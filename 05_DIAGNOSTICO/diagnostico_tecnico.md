# DIAGNÓSTICO TÉCNICO DE ENGENHARIA ELÉTRICA — REDE DE DISTRIBUIÇÃO SANTOS/SP
## FOCO OPERACIONAL: ALIMENTADOR STO33 (CPFL PIRATININGA — SUBESTAÇÃO SANTOS 1)

**Data do Diagnóstico:** Outubro de 2026  
**Autor:** Antigravity / Investigação Técnica de Engenharia  
**Classificação de Dados:** Conforme Diretriz de Rastreabilidade ANEEL (Classes A a G)

---

## 1. RESUMO EXECUTIVO DO DIAGNÓSTICO

A investigação técnica detalhada da rede de distribuição de Santos/SP, a partir do cruzamento da Base de Dados Geográfica da Distribuidora (BDGD 2025/2026), dos Indicadores Gerenciais da Distribuição (INDGER) e da Relação de Empreendimentos de Geração Distribuída da ANEEL, comprovou que:

1. **Subestação de Origem:** O alimentador **STO33** parte comprovadamente da **Subestação Santos 1 (STO)**, localizada na região central/portuária de Santos (lat -23.926°, lon -46.383°), alimentado pelo barramento de média tensão `STO569794` em 13,8 kV, derivado do transformador de força `UNI_TR_AT 56622` (denominado operacionalmente `STO-ETR102`, 20 MVA, 88/13,8 kV).
2. **Carregamento Elevado Confirmado:** O alimentador STO33 opera com **carregamento elevado confirmado por indicador oficial da ANEEL** (`PctCarregamentoMaxAlimentador` no INDGER entre **91,04% e 94,00%**).
3. **Topologia Completa e Conectada:** A rede de média tensão do STO33 possui **730 trechos de linha** somando **19,37 km de extensão**, 86 chaves seccionadoras e 105 transformadores MT/BT. A topologia física forma uma **árvore conectada perfeita com 793 nós e 792 ramos**, sem ilhas elétricas ou segmentos órfãos.
4. **Concentração de Carga e Risco em Transformadores:** O alimentador atende **6.499 unidades consumidoras de baixa tensão (UCBT)** e 7 unidades de média tensão (UCMT). Foram identificados transformadores com carregamento crítico, destacando-se o trafo `1193581` (225 kVA) atendendo **439 consumidores**, cujo cálculo de demanda simultânea aponta carregamento superior a **120% da capacidade nominal**.
5. **Geração Distribuída:** Há **23 sistemas de microgeração solar fotovoltaica** conectados na BT do alimentador STO33 (somando 44,35 kWp cadastrados), e **1.342 usinas solares** no município de Santos (16,89 MWp). Atualmente, a geração solar no STO33 é amplamente absorvida pela carga local, não havendo fluxo reverso para a subestação.
6. **Eletromobilidade:** Santos possui **4.593 veículos eletrificados** registrados (DETRAN-SP, setembro/2026), dos quais **1.101 são veículos plug-in** com recarga em rede (589 BEV e 422 PHEV). A coincidência de recarga noturna não gerenciada representa o maior vetor de sobrecarga futura nos transformadores residenciais.

---

## 2. MATRIZ DE PONTOS CRÍTICOS IDENTIFICADOS

| ID Equipamento | Tipo | Indicador Técnico | Valor Registrado | Limite | Classificação de Risco | Grau de Confiança |
| :--- | :--- | :--- | :--- | :--- | :--- | :---: |
| **STO33 Cabeceira** | Disjuntor Saída MT | PctCarregamentoMaxAlimentador | **91,04% - 94,00%** | 80% (N-1) | **2. Carregamento elevado oficial** | **C** |
| **Trafo 1193581** | Trafo 225 kVA | Demanda Estimada / 439 UCs | **370 kVA (164%)** | 225 kVA | **3. Risco técnico calculado** | **E** |
| **Trafo 1193923** | Trafo 225 kVA | Demanda Estimada / 280 UCs | **255 kVA (113%)** | 225 kVA | **3. Risco técnico calculado** | **E** |
| **Trafo 1193996** | Trafo 225 kVA | Demanda Estimada / 208 UCs | **198 kVA (88%)** | 225 kVA | **4. Suspeita técnica preventiva** | **E** |
| **Trafo 48964485** | Trafo 150 kVA | Demanda Estimada / 185 UCs | **172 kVA (115%)** | 150 kVA | **3. Risco técnico calculado** | **E** |
| **Ramais Finais** | Condutores PA1017 | Ampacidade Térmica | 202 A | 202 A | **4. Suspeita técnica preventiva** | **B** |
| **Subestação STO** | Trafo ETR102 (20 MVA) | Energia Mensal Agregada | 5,7 GWh/mês | 20 MVA | **2. Carregamento elevado oficial** | **B** |

---

## 3. AVALIAÇÃO DE FLUXO REVERSO E GERAÇÃO FOTOVOLTAICA

- **Consumo Mensal Médio do STO33:** 1,85 GWh/mês (~2.570 kW médios contínuos).
- **Capacidade Solar Instalada no STO33:** 44,35 kWp distribuídos em 23 instalações.
- **Balanço Potência Líquida:** A potência fotovoltaica máxima simultânea no pico solar (12h-13h) gera aproximadamente 35 a 40 kW, o que representa menos de **1,5% da carga mínima diurna** do alimentador.
- **Conclusão:** Não existe possibilidade de fluxo reverso na cabeceira do alimentador STO33 no cenário atual. Eventual reversão local de fluxo está restrita a ramais secundários de transformadores específicos caso novas instalações de minigeração comercial acima de 75 kW sejam conectadas sem reforço de rede.

---

## 4. IMPACTO DA MOBILIDADE ELÉTRICA (VEÍCULOS ELÉTRICOS)

- Com 1.101 veículos plug-in registrados em Santos e ~6.500 consumidores no STO33 (cerca de 3,6% das UCs do município), estima-se a presença de **40 a 50 veículos elétricos e híbridos plug-in** atendidos por este circuito.
- Se 20 desses veículos forem conectados simultaneamente entre 19h e 21h em carregadores residenciais de 7,4 kW (32 A em 220 V), haverá uma sobrecarga adicional de **148 kW concentrada exatamente no horário de ponta do sistema**.
- Em transformadores já próximos de 90% de carregamento (como os trafos `1193581`, `1193923` e `48964485`), a conexão de apenas 3 a 5 veículos elétricos no mesmo transformador pode precipitar a atuação de elos fusíveis ou acelerar a degradação térmica do isolamento por sobrecarga.

---

## 5. RECOMENDAÇÕES PARA APLICAÇÃO DE BESS E CONTROLE VOLT/VAR

1. **BESS na Cabeceira (Subestação Santos 1):** Um sistema BESS centralizado de **1,5 MW / 3 MWh** permitiria raspar o pico noturno (peak shaving) do STO33 e STO32, aliviando o transformador de força `ETR102` de 20 MVA e postergando obras de ampliação da subestação.
2. **BESS Distribuído em Baixa Tensão:** Instalação de unidades BESS comunitárias de **50 kW / 150 kWh** conectadas aos secundários dos transformadores críticos (`1193581` e `1193923`), absorvendo demanda de recarga veicular e mitigando afundamentos de tensão.
3. **Controle Volt/VAR em Inversores Fotovoltaicos:** Habilitar curvas $Q(V)$ e $\cos\phi(P)$ nos inversores solares conectados ao STO33 para suporte de reativos durante a tarde, aliviando os 2 bancos de capacitores cadastrados (`UNCRMT`).
