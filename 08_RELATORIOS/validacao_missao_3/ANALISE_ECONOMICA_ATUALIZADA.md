# Análise Econômica Atualizada e Comparativa — Missão 3
## Avaliação de Custos, Vida Útil, Reposição e Limitações Metodológicas
### Projeto COBRIC 2026 — Alimentador STO33 (CPFL Piratininga)

---

## 1. Contexto e Diretriz de Rigor Científico

Na literatura técnica e em estudos de planejamento da distribuição, é comum encontrar comparações superficiais que contrastam apenas o **CAPEX inicial** de obras convencionais contra o de sistemas de baterias, afirmando precipitadamente a superioridade de uma alternativa sobre a outra.

Em estrito atendimento às diretrizes do **COBRIC 2026**, esta análise estabelece uma separação formal entre:
1. Custos de capital inicial (CAPEX turn-key).
2. Custos operacionais e de manutenção contínua (OPEX).
3. Vida útil regulatória e depreciação patrimonial.
4. Ciclos de reposição de baterias (*battery augmentation* / substituição de células).
5. Eficiência energética e custo das perdas elétricas.
6. Horizonte temporal de planejamento e avaliação em bases equivalentes.

---

## 2. Comparativo Estruturado de Parâmetros Econômicos

| Parâmetro de Avaliação | Cenário C: BESS Centralizado (SE) | Cenário D: BESS Distribuído (BT) | Cenário E: Reforço Físico Convencional |
|:---|:---|:---|:---|
| **Escopo Físico** | 1 container outdoor utility-scale (1,5 MW / 3,0 MWh) | 3 armários outdoor pad-mounted (50 kW / 150 kWh cada = 450 kWh) | Upgrade de 3 transformadores para 300 kVA + 4,5 km de recondutoramento (575 A) |
| **CAPEX Inicial Estimado** | **R$ 4.950.000,00** | **R$ 945.000,00** | **R$ 553.500,00** |
| **Custo Unitário Adotado** | US$ 300/kWh turn-key (R$ 1.650/kWh @ R$ 5,50/US$) | US$ 380/kWh turn-key (R$ 2.100/kWh @ R$ 5,50/US$) | R$ 42.000/trafo + R$ 95.000/km de rede primária |
| **Data-Base e Referência** | EPE (Nota Técnica DEA 01/2024), FGV Energia, BloombergNEF | NREL Utility/C&I Storage Benchmark (2024-2025) | Tabela Média de Preços CPFL / Banco de Preços ANEEL |
| **Vida Útil Operacional** | **10 a 12 anos** (4.000 a 5.000 ciclos até 70% SoH) | **10 a 12 anos** (4.000 a 5.000 ciclos até 70% SoH) | **30 a 35 anos** (conforme MCPSE / ANEEL) |
| **Necessidade de Reposição** | Reinvestimento de 40% a 50% do CAPEX no 11º ano | Reinvestimento de 40% a 50% do CAPEX no 11º ano | Nenhuma reposição estrutural em 30 anos |
| **OPEX Anual Estimado** | 1,5% a 2,0% do CAPEX inicial (R$ 74k a R$ 99k/ano) | 2,0% a 2,5% do CAPEX inicial (R$ 19k a R$ 24k/ano) | 0,5% a 1,0% do CAPEX inicial (R$ 3k a R$ 5k/ano) |
| **Custos de O&M Inclusos** | Manutenção de HVAC, recertificação de combate a incêndio | Manutenção preventiva de inversores e inspeção anual | Poda de árvores e inspeção termográfica periódica |
| **Eficiência Round-Trip ($\eta_{\text{RT}}$)** | **90,25%** (perda interna diária de 226,3 kWh) | **90,25%** (perda interna diária de 33,9 kWh) | **100%** (sem perdas eletroquímicas; apenas redução de perdas Joule) |
| **Custo Diário das Perdas no BESS** | R$ 181,00/dia (a R$ 0,80/kWh de energia no atacado) | R$ 27,10/dia (a R$ 0,80/kWh) | — (Economia de 8,59 kWh/dia de perdas Joule na linha) |

---

## 3. Análise Crítica dos Resultados e Vieses Comparativos

### 3.1 A Ilusão do CAPEX Imediato
Se a avaliação se limitar ao custo de desembolso no ano zero ($t=0$):
- O Reforço Físico (Cenário E) desponta como **8,9 vezes mais barato** que o BESS Centralizado e **1,7 vezes mais barato** que o BESS Distribuído.
- Contudo, essa conclusão ignora que o reforço físico é um ativo puramente passivo e unidirecional, enquanto o BESS é um recurso de flexibilidade ativa.

### 3.2 O Impacto da Vida Útil e da Reposição de Células
Em um horizonte de planejamento regulatório de **30 anos** (padrão de concessão de distribuição):
- O Reforço Físico Convencional opera durante todo o período sem necessidade de substituição patrimonial, com custos de manutenção ínfimos.
- Os sistemas BESS (Cenários C e D) exigirão **duas substituições parciais ou totais de células de bateria** (ao redor do ano 11 e do ano 22), elevando significativamente o custo acumulado no ciclo de vida (*Life-Cycle Cost*).

### 3.3 A Valoração dos Serviços Ancilares e Flexibilidade Operacional
Para que a comparação econômica seja justa e academicamente sólida, o artigo do COBRIC 2026 deve ressaltar que os sistemas de armazenamento oferecem benefícios que não são capturados pelo reforço físico:
1. **Corte de Demanda na Ponta Sistêmica:** O BESS Centralizado posterga a ampliação do transformador de força da subestação (de 20 MVA para 25/30 MVA) e o reforço da linha de subtransmissão de 88 kV.
2. **Suporte Rápido de Reativos ($Q$):** Os inversores bidirecionais a quatro quadrantes permitem regulação dinâmica de tensão em milissegundos sem desgaste mecânico (ao contrário de reguladores de tensão sob carga e bancos de capacitores chaveados).
3. **Absorção de Geração Solar Excedente:** O BESS armazena o excedente solar do meio-dia para injeção noturna, reduzindo o *curtailment* em cenários futuros de penetração massiva de GD.
4. **Resiliência e Microgrids:** O BESS Distribuído permite a operação em ilha intencional de blocos de consumidores críticos durante contingências graves na linha tronco.

---

## 4. Classificação Metodológica dos Custos

Conforme os padrões internacionais de engenharia de custos da **AACE International (American Association of Cost Engineering)**:
- Os valores adotados classificam-se como **Estimativas de Ordem de Grandeza (Classe 4 / Classe 5)**, com margem de incerteza típica de $-20\%$ a $+30\%$.
- A ausência de cotações de fornecedores locais para o alimentador STO33 e a volatilidade do câmbio (R$/US$) e dos preços de lítio (LFP) no mercado internacional impedem declarar esses valores como preços firmes de contratação.
- **Declaração Científica:** A comparação econômica presente neste estudo tem finalidade exclusivamente comparativa e ilustrativa de cenários, devendo ser classificada como **preliminar e inconclusiva para fins de decisão de investimento regulatório definitivo**.
