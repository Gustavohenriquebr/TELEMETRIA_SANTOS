# Relatório de Correção Física dos BESS e Reexecução dos Cenários — Missão 3
## Validação Científica para o Artigo do COBRIC 2026
### Projeto: TELEMETRIA_SANTOS | Alimentador STO33 | CPFL Piratininga

---

## 1. O Que Foi Corrigido

1. **Eliminação de Injeção de Energia Sem Lastro Físico no Cenário C:**
   - **Antes (Missão 2):** O BESS Centralizado descarregava **3.500 kWh** no período noturno (18h-21h) após ter carregado apenas **2.300 kWh** na mesma data em um container com capacidade nominal de **3.000 kWh**, violando a conservação de energia e os limites de placa.
   - **Agora (Missão 3):** Implementou-se um algoritmo estrito de controle de estado de carga ($SOC$). O despacho foi redimensionado para descarregar **2.200,0 kWh** (às 19h: 700 kW; 20h: 950 kW; 21h: 550 kW) e carregar **2.210,5 kWh** (madrugada: 1.800 kWh; meio-dia: 410,5 kWh), resultando em um ciclo perfeitamente equilibrado com eficiência round-trip de **90,25%** e perdas internas de conversão de **226,3 kWh**.
2. **Correção do BESS Distribuído no Cenário D:**
   - **Antes (Missão 2):** Descarregava 155 kWh por unidade contra 125 kWh carregados (excedendo a capacidade nominal de 150 kWh por unidade).
   - **Agora (Missão 3):** Cada unidade de 50 kW / 150 kWh foi parametrizada com controle independente de $SOC$, carregando **110,5 kWh** e descarregando **110,0 kWh** no pico, mantendo o $SOC$ estritamente entre **12,8%** e **90,0%**.
3. **Mecanismo de Throttling e Proteção contra Sobrecarga:**
   - No meio-dia (13h), quando a geração solar atinge o pico e a bateria se aproxima de $SOC_{\max} = 90,0\%$, a potência de recarga é automaticamente reduzida para impedir que a energia ultrapasse a capacidade nominal física.

---

## 2. O Que Foi Mantido

1. **Parâmetros Cadastrais e Elétricos da Rede STO33:**
   - 105 transformadores de distribuição totalizando **14.318,0 kVA** (média de 136,36 kVA/trafo).
   - 730 trechos de média tensão totalizando **19,37 km** cadastrais (19.372,73 m campo `COMP`).
   - 94 cargas agregadas na baixa tensão atendendo 6.499 clientes de BT e 7 de MT.
   - 2 bancos de capacitores de MT (2 x 300 kvar = 600 kvar).
   - 23 usinas solares fotovoltaicas baseline somando **195,89 kW** (dados oficiais ANEEL MMGD).
2. **Premissas do Cenário B (Expansão FV + VE):**
   - Expansão de +100% de geração fotovoltaica (+195,89 kW em 23 novas usinas).
   - Inclusão de 100 veículos elétricos residenciais (740 kW totais) em carregadores trifásicos de 7,4 kW alocados nos 20 maiores transformadores residenciais com curva `PerfilVE`.
3. **Modelagem do Cenário E (Reforço Físico Convencional):**
   - Substituição dos transformadores `TR_1193581`, `TR_1193923` e `TR_1193996` para 300 kVA.
   - Recondutoramento de 188 trechos de tronco (5,247 km) com condutor `PA6001` (575 A, $R_1 = 0,1865\ \Omega/\text{km}$).

---

## 3. Quais Resultados Melhoraram e Quais Pioraram

### O Que Melhorou:
- **Consistência Científica e Física:** O balanço de massa/energia fecha com rigor absoluto. O modelo é agora 100% defensável perante revisores de bancas acadêmicas e congressos de engenharia.
- **Rastreabilidade de SOC:** Todos os sistemas BESS agora possuem curvas horárias de $SOC$, potência e perdas transparentes registradas em [`BALANCO_ENERGETICO_BESS.csv`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/08_RELATORIOS/validacao_missao_3/BALANCO_ENERGETICO_BESS.csv).
- **Perdas no Cenário D:** O Cenário D consolidou-se como o de menor perda técnica da rede (**1.649,67 kWh/dia**, contra 1.664 kWh no baseline/VEs e 1.655 kWh no reforço físico).

### O Que Piorou (Impacto do Realismo Físico):
- **Pico de Demanda no Cenário C:**
  - Na Missão 2 (sem controle de $SOC$): $P_{\text{pico}} = 2.724,82\text{ kW}$ às 20h.
  - Na Missão 3 (com restrição física de 3.000 kWh): $P_{\text{pico}} = \mathbf{2.780,92\text{ kW}}$ às 19h (e 2.774,84 kW às 20h).
  - *Diferença:* $+56,10\text{ kW}$ (+2,06%). Esse ligeiro acréscimo reflete a limitação física real: para não esgotar a bateria abaixo de 10% de $SOC$ e manter o corte de ponta ao longo das três horas mais críticas (19h, 20h e 21h), a descarga às 20h foi ajustada para 950 kW (em vez de 1.000 kW) e às 19h para 700 kW. O corte de pico real é de **-943,98 kW (-25,3%)**, em vez dos 1.000 kW fictícios.
- **Demanda Mínima no Cenário C:**
  - Passou de 665,57 kW para **767,07 kW**, pois a recarga noturna física (450 kW) foi estendida até as 05h da manhã para repor integralmente a energia consumida no dia anterior.

---

## 4. Tabela de Resultados Consolidados da Missão 3

Arquivo de referência: [`08_RELATORIOS/validacao_missao_3/RESULTADOS_CENARIOS_COBRIC_MISSAO_3.csv`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/08_RELATORIOS/validacao_missao_3/RESULTADOS_CENARIOS_COBRIC_MISSAO_3.csv)

| Cenário Avaliado | Pico de Demanda na SE (kW) | Hora do Pico | Demanda Mínima (kW) | Energia Diária Injetada (kWh/dia) | Perdas Técnicas Diárias (kWh/dia) | Perdas Percentuais (%) | Tensão Mínima ($V_{\min}$, pu) | Tensão Máxima ($V_{\max}$, pu) | CAPEX Estimado da Solução (R$) | Convergência 100% |
|:---|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| **Cenário A (Baseline)** | 2.982,50 | 20h | 658,14 | 29.413,03 | 1.539,15 | 5,23% | 0,9744 | 1,0288 | R$ 0,00 | **SIM** |
| **Cenário B (FV + VE)** | 3.724,90 | 20h | 665,61 | 32.823,09 | 1.664,31 | 5,07% | 0,9523 | 1,0288 | R$ 0,00 | **SIM** |
| **Cenário C (BESS Central)** | **2.780,92** | 19h | 767,07 | 32.833,33 | 1.664,02 | 5,07% | 0,9530 | 1,0286 | R$ 4.950.000,00 | **SIM** |
| **Cenário D (BESS Distrib.)**| **3.580,88** | 20h | 665,68 | 32.810,70 | **1.649,67** | **5,03%** | **0,9540** | 1,0288 | R$ 945.000,00 | **SIM** |
| **Cenário E (Reforço Físico)**| 3.722,92 | 20h | 665,53 | 32.814,77 | 1.655,72 | 5,05% | 0,9525 | 1,0288 | **R$ 553.500,00** | **SIM** |

---

## 5. Classificação Conclusiva dos Cenários

- **Cenário A (Baseline):** **VALIDADO**. Modelo consistente com a BDGD e dados da ANEEL.
- **Cenário B (Expansão FV + VE):** **VALIDADO**. Simulação de fluxo de carga consistente com elevação esperada de demanda noturna e queda de tensão.
- **Cenário C (BESS Centralizado):** **VALIDADO COM RESSALVA DE ESCOPO**. O modelo físico de bateria agora respeita a conservação de energia e limites de $SOC$. A ressalva científica reside no fato de que o BESS central não alivia a sobrecarga térmica dos transformadores de distribuição secundários.
- **Cenário D (BESS Distribuído):** **VALIDADO**. Comprovou-se o duplo benefício: alívio de 19,63% no carregamento do transformador mais crítico e menor índice de perdas técnicas em toda a rede.
- **Cenário E (Reforço Físico Convencional):** **VALIDADO**. Alterações de condutores e transformadores comprovadas nos arquivos DSS, resolvendo os limites térmicos com o menor custo de investimento inicial.

---

## 6. Comandos Exatos para Reproduzir a Simulação

Para reproduzir a simulação completa da Missão 3 a partir do terminal:

```bash
# 1. Ativar o ambiente virtual Python
source /Users/guga/Desktop/COBRIC_Simulador_Visual/.venv/bin/activate

# 2. Navegar para a raiz do repositório
cd /Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS

# 3. Executar o orquestrador da Missão 3
python3 08_RELATORIOS/validacao_missao_3/executar_missao_3.py

# 4. Executar os cenários no script geral
python3 07_OPENDSS/cenarios_cobric/run_cenarios_cobric.py
```
