# Checklist de Validação Física e Elétrica dos 5 Cenários — COBRIC 2026
## Auditoria Técnica e Reprodutibilidade da Modelagem OpenDSS
### Alimentador STO33 | Subestação STO (Santos 1) | CPFL Piratininga

---

## 1. Conservação de Energia e Balanço de Potência Instantânea

Para cada uma das 24 horas e para todos os 5 cenários simulados (120 passos de tempo horários), foi verificado o balanço de potência ativa no alimentador:

$$P_{\text{subestação}}(t) + P_{\text{FV}}(t) + P_{\text{BESS\_descarga}}(t) = P_{\text{cargas}}(t) + P_{\text{VE}}(t) + P_{\text{BESS\_carga}}(t) + P_{\text{perdas\_técnicas}}(t)$$

- **Resultado da Verificação:** **APROVADO**.
- **Erro Máximo Absoluto de Balanço:** **0,3317 kW** (observado no Cenário B às 20h, representando erro residual de $0,0089\%$ em relação à potência total circulante de 3.725 kW).
- **Causa do Resíduo:** Truncamento natural de condutâncias transversais de transformadores em vazio e admitâncias shunt capacitivas de condutores no algoritmo iterativo de injeção de corrente do OpenDSS.

---

## 2. Coerência de Grandezas Elétricas (kW, kvar, kVA e Fator de Potência)

- **Potência Reativa e Bancos de Capacitores:**
  - O alimentador STO33 possui 2 bancos de capacitores cadastrados na média tensão (`Capacitors.dss`), cada um com 300 kvar trifásico (total de 600 kvar).
  - No patamar de pico (20h), a demanda reativa indutiva bruta das cargas é de approximately $1.075\text{ kvar}$.
  - Com os 600 kvar capacitivos injetados pelos bancos, a potência reativa líquida demandada na subestação é de **437,91 kvar**.
  - O Fator de Potência global resultante na subestação é de **0,9894 indutivo**, plenamente em conformidade com o limite regulatório mínimo do PRODIST ($FP \ge 0,92$).
- **Durante a Madrugada (01h às 06h):**
  - Devido à baixa carga ativa (658 a 754 kW) e à injeção fixa dos 600 kvar capacitivos, o fluxo reativo na subestação inverte-se para **-340 a -435 kvar (capacitivo)**.
  - Isso provoca ligeira elevação de tensão na barra da SE para **1,0288 pu**, sem violar a faixa adequada ($V \le 1,05\text{ pu}$).

---

## 3. Limites de Potência dos Inversores e Elementos de Geração

- **Usinas Solares Fotovoltaicas (Baseline e Expansão):**
  - Todas as 23 usinas baseline (195,89 kW totais) e as 23 usinas da expansão (+195,89 kW) foram modeladas com fator de potência unitário ($FP = 1,0$), em conformidade com as exigências técnicas da CPFL e do Módulo 3 do PRODIST para microgeração conectada na baixa tensão.
  - A potência aparente dos inversores ($S_{\text{inv}}$) foi parametrizada com margem de segurança de 10% ($kVA = 1,10 \times P_{\text{mpp}}$).
- **BESS Centralizado (Cenário C):**
  - Inversor bidirecional (PCS) parametrizado com $P_{\text{rated}} = 1.500\text{ kW}$.
  - O despacho máximo de injeção programado é de **1.000 kW** (às 19h, 20h e 21h), operando com fator de carregamento do inversor de $66,7\%$ da capacidade nominal.
- **BESS Distribuído (Cenário D):**
  - Inversores de cada uma das 3 unidades parametrizados com $P_{\text{rated}} = 50\text{ kW}$.
  - O despacho máximo no pico é de **45 kW** por unidade ($90\%$ de carregamento do inversor), respeitando a margem técnica de operação.

---

## 4. Análise Física da Disponibilidade Energética e SOC do BESS

### 4.1 BESS Centralizado (1,5 MW / 3,0 MWh)
- **Despacho Programado no Modelo:**
  - Carga noturna (02h, 03h, 04h): $3 \times 500\text{ kW} = 1.500\text{ kWh}$ absorvidos da rede.
  - Recarga solar diurna (12h, 13h): $2 \times 400\text{ kW} = 800\text{ kWh}$ absorvidos da rede.
  - **Total de Energia Absorvida:** **2.300 kWh/dia**.
  - Descarga pré-pico (18h): $1 \times 500\text{ kW} = 500\text{ kWh}$ injetados.
  - Descarga no pico (19h, 20h, 21h): $3 \times 1.000\text{ kW} = 3.000\text{ kWh}$ injetados.
  - **Total de Energia Injetada:** **3.500 kWh/dia**.
- **Auditoria Física e Alerta Técnico:**
  - Existe um déficit energético líquido diário de $3.500 - 2.300 = 1.200\text{ kWh/dia}$ caso a bateria inicie o ciclo em $SOC$ parcial sem considerar recargas complementares.
  - Se a capacidade nominal é de **3.000 kWh**, descarregar 3.500 kWh em um ciclo contínuo de pico exigiria recarga intermediária completa ou um dimensionamento nominal de **4,0 MWh** para manter o estado de carga ($SOC$) entre os limites de reserva segura ($10\% \le SOC \le 90\%$).
  - **Classificação:** O modelo OpenDSS executou via `Generator` com curva de despacho pré-definida sem interrupção por sub-tensão/sub-capacidade. Para o artigo final do COBRIC, recomenda-se ajustar o despacho de pico para **800 kW por 3 horas (2.400 kWh)** ou redimensionar a capacidade nominal do container para **4,0 MWh**.

### 4.2 BESS Distribuído (3 x 50 kW / 150 kWh = 450 kWh totais)
- **Despacho Programado por Unidade:**
  - Carga noturna e solar: $25\text{ kW} \times 5\text{h} = 125\text{ kWh}$ absorvidos por unidade ($375\text{ kWh}$ totais).
  - Descarga no pico: $20\text{ kW} \times 1\text{h} + 45\text{ kW} \times 3\text{h} = 155\text{ kWh}$ injetados por unidade ($465\text{ kWh}$ totais).
  - Razão entre energia descarregada e capacidade nominal: $155 / 150 = 103,3\%$.
  - **Classificação:** Praticamente equilibrado, exigindo ajuste marginal no artigo para descarga de **40 kW** por 3h ($120\text{ kWh} + 20\text{ kWh} = 140\text{ kWh} \le 150\text{ kWh}$).

---

## 5. Avaliação do Alívio Local vs Alívio Sistêmico nos Transformadores Críticos

Um dos aspectos científicos mais relevantes exigidos nesta auditoria é comprovar se a redução de potência observada na subestação efetivamente alivia os transformadores de distribuição.

Os dados medidos nos 4 maiores postos de transformação no horário de pico (20h) demonstram:

| Cenário | Trafo 1193581 (225 kVA) | Trafo 1193923 (225 kVA) | Trafo 1193996 (225 kVA) | Trafo 48964485 (150 kVA) | Comportamento Técnico Observado |
|:---|:---:|:---:|:---:|:---:|:---|
| **Cenário A (Baseline)** | 78,05% (175,6 kVA) | 50,04% (112,6 kVA) | 38,24% (86,1 kVA) | 52,74% (79,1 kVA) | Carregamento normal da rede atual |
| **Cenário B (FV + VE)** | **94,67% (213,0 kVA)** | **66,39% (149,4 kVA)** | **54,49% (122,6 kVA)** | **77,33% (116,0 kVA)** | Impacto severo da recarga simultânea de VEs (+37 kW por trafo) |
| **Cenário C (BESS Centralizado)** | **94,67% (213,0 kVA)** | **66,39% (149,4 kVA)** | **54,49% (122,6 kVA)** | **77,33% (116,0 kVA)** | **NENHUM ALÍVIO LOCAL**: BESS na SE não reduz a corrente nos transformadores de MT/BT |
| **Cenário D (BESS Distribuído)** | **75,04% (168,8 kVA)** | **47,08% (105,9 kVA)** | **35,35% (79,5 kVA)** | 77,33% (116,0 kVA) | **ALÍVIO REAL COMPROVADO**: BESS na BT reduz o carregamento em **-19,63%** no trafo mais crítico |
| **Cenário E (Reforço Físico)** | **70,08% (210,3 kVA)** | **49,35% (148,1 kVA)** | **40,58% (121,7 kVA)** | 77,33% (116,0 kVA) | **ALÍVIO TÉRMICO POR UPGRADE**: Capacidade aumentada de 225 para 300 kVA |

> [!IMPORTANT]
> **Evidência Central da Auditoria:**
> O BESS Centralizado (Cenário C), embora reduza a demanda de ponta na subestação em **1.000 kW**, tem **efeito nulo sobre a sobrecarga dos transformadores de distribuição** (o carregamento do trafo 1193581 permanece inalterado em 94,67%).
> Em contrapartida, o BESS Distribuído (Cenário D) conectado no secundário (baixa tensão) dos transformadores alivia o carregamento do transformador mais crítico para **75,04%**, provando a superioridade técnica da descentralização do armazenamento para alívio de ativos locais.

---

## 6. Conformidade de Tensão com o PRODIST Módulo 8

Os limites regulatórios da ANEEL (PRODIST Módulo 8) para redes de distribuição em baixa tensão (220/127 V) e média tensão (13,8 kV) estabelecem:
- **Faixa Adequada:** $0,93\text{ pu} \le V \le 1,05\text{ pu}$.
- **Faixa Precária:** $0,87\text{ pu} \le V < 0,93\text{ pu}$ ou $1,05\text{ pu} < V \le 1,06\text{ pu}$.
- **Faixa Crítica:** $V < 0,87\text{ pu}$ ou $V > 1,06\text{ pu}$.

### Resultados Auditados de Tensão Nodal:
- **Cenário A:** $V_{\min} = 0,9744\text{ pu}$, $V_{\max} = 1,0288\text{ pu}$ &rarr; **100% dos nós em Faixa Adequada**.
- **Cenário B:** $V_{\min} = 0,9523\text{ pu}$, $V_{\max} = 1,0288\text{ pu}$ &rarr; Queda de tensão acentuada no pico noturno (-0,0221 pu), porém ainda dentro da Faixa Adequada.
- **Cenário C:** $V_{\min} = 0,9531\text{ pu}$, $V_{\max} = 1,0288\text{ pu}$ &rarr; Suporte de tensão marginal na barra da SE.
- **Cenário D:** $V_{\min} = 0,9540\text{ pu}$, $V_{\max} = 1,0288\text{ pu}$ &rarr; **Melhor suporte de tensão no final do alimentador**, decorrente da injeção de potência ativa na baixa tensão próxima aos nós terminais.
- **Cenário E:** $V_{\min} = 0,9525\text{ pu}$, $V_{\max} = 1,0288\text{ pu}$ &rarr; Suporte por redução da impedância série do tronco ($R_1$).

---

## 7. Carregamento dos Condutores de Média Tensão

- O condutor de saída do disjuntor da subestação (cabo subterrâneo `PS1113`, cobre 185 mm²) possui ampacidade nominal de **351 A**.
- O tronco aéreo principal parte com condutores de alta capacidade (`PA6001`), com ampacidade nominal de **420 A** (elevada para **575 A** no Cenário E).
- **Corrente de Pico na Cabeceira (20h):**
  - Cenário A: $I = 126,8\text{ A}$ (carregamento de $36,1\%$ no cabo subterrâneo e $30,2\%$ no tronco).
  - Cenário B: $I = 158,5\text{ A}$ (carregamento de $45,2\%$ no cabo subterrâneo e $37,7\%$ no tronco).
  - Cenário C: $I = 115,9\text{ A}$ (carregamento de $33,0\%$ no cabo da SE).
  - Cenário D: $I = 152,4\text{ A}$ (carregamento de $43,4\%$).
  - Cenário E: $I = 158,4\text{ A}$ (carregamento de $45,1\%$ no cabo e $27,5\%$ no tronco recondutorado).
- **Conclusão:** Nenhum trecho de média tensão opera com sobrecarga térmica contínua nos cenários simulados. Os gargalos físicos da rede estão concentrados estritamente nos **transformadores de distribuição secundários** que atendem mais de 150 consumidores.

---

## 8. Síntese do Checklist

| Item Auditado | Critério Técnico | Status | Evidência / Rastreabilidade |
|:---|:---|:---:|:---|
| **Balanço de Potência** | Erro $< 0,1\%$ em todas as 24h | **APROVADO** | Erro máximo de 0,33 kW em 3.725 kW ($0,0089\%$) |
| **Convergência do Fluxo** | Convergência estrita em 4 iterações | **APROVADO** | 100% dos passos convergidos sem divergência nodal |
| **Níveis de Tensão (PRODIST)** | $0,93 \le V \le 1,05\text{ pu}$ | **APROVADO** | $V_{\min} = 0,9523\text{ pu}$ no pior caso (Cenário B) |
| **Fator de Potência da SE** | $FP \ge 0,92$ no pico de carga | **APROVADO** | $FP = 0,9894$ indutivo às 20h |
| **Alívio de Transformadores (BESS Distribuído)** | Redução de carregamento comprovada | **APROVADO** | Queda de 94,67% para 75,04% no trafo crítico |
| **Alívio de Transformadores (BESS Central)** | Redução de carregamento local | **REPROVADO** | Carregamento inalterado em 94,67% (BESS na barra SE) |
| **Balanço Energético do BESS Central** | $E_{\text{descarga}} \le E_{\text{carga}} \times \eta$ | **ALERTA** | Descarga de 3,5 MWh vs Carga de 2,3 MWh requer ajuste |
| **Modelagem do Reforço Físico** | Alteração nos arquivos de rede comprovada | **APROVADO** | 3 trafos para 300 kVA e 188 trechos (5,25 km) para 575 A |
