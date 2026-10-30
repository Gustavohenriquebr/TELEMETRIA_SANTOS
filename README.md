# Telemetria Elétrica e Engenharia Reversa da Rede de Distribuição — Santos/SP
## Alimentador STO33 | Subestação STO (Santos 1) | CPFL Piratininga | Pesquisa COBRIC 2026

[![Python](https://img.shields.io/badge/Python-3.9%2B-blue.svg)](https://www.python.org/)
[![OpenDSS](https://img.shields.io/badge/Simulation-OpenDSS%20QSTS-green.svg)](https://www.epri.com/pages/sa/opendss)
[![BDGD](https://img.shields.io/badge/Data-ANEEL%20BDGD%202025-orange.svg)](https://dadosabertos.aneel.gov.br/)
[![COBRIC](https://img.shields.io/badge/Pesquisa-COBRIC%202026-purple.svg)]()
[![Status](https://img.shields.io/badge/Auditoria%20e%20Pipeline-Missão%203%20Fisicamente%20Validada-brightgreen.svg)]()

Base técnica, analítica e de modelagem computacional para a reconstrução topológica, telemetria elétrica, auditoria técnica e diagnóstico da rede de média tensão do alimentador **STO33** da **CPFL Piratininga** (Subestação Santos 1).

O projeto integra dados georreferenciados oficiais da **ANEEL (BDGD 2025, INDGER, MMGD, Curvas CTR)** e do **DETRAN-SP**, modelagem elétrica quase-estática temporal (QSTS de 24h) em **OpenDSS**, grafos em **NetworkX**, mapas interativos em **Folium/Leaflet**, e o estudo comparativo de **5 cenários para o Congresso Brasileiro de Iniciação Científica (COBRIC 2026)** avaliando a integração de sistemas fotovoltaicos (FV), veículos elétricos (VE), sistemas de armazenamento de energia em baterias (BESS centralizado e distribuído com controle físico estrito de $SOC$) e reforço físico de rede.

---

## 1. Visão Geral e Indicadores Técnicos do Alimentador STO33

| Parâmetro Técnico | Valor / Especificação | Fonte / Método |
|:---|:---|:---|
| **Concessionária** | CPFL Piratininga (Distribuição) | Cadastro Oficial ANEEL |
| **Subestação de Origem** | SE STO (Santos 1) — 88/13.8 kV | BDGD ANEEL 2025 (`SUB`) |
| **Conjunto Elétrico** | Código 15884 | INDGER ANEEL 2025 (`CONJ`) |
| **Tensão Nominal Primária** | 13,8 kV (Trifásico) | BDGD ANEEL / Linhas de MT |
| **Extensão Total de MT** | **19,37 km** cadastrais (19.372,73 m campo `COMP`; 18,91 km geodésicos em 730 segmentos) | BDGD camada `SSDMT` |
| **Nós Topológicos** | 793 nós georreferenciados | Topologia NetworkX / Dijkstra |
| **Conectividade Topológica** | Árvore estritamente radial (793 nós, 792 arestas, sem ilhas) | Análise de Grafos / BDGD |
| **Transformadores de MT/BT** | 105 transformadores (13.8 / 0.22 kV) | BDGD camada `UNTRMT` |
| **Potência Instalada de Transformação** | **14.318,0 kVA** total (média 136,36 kVA/trafo) | BDGD ANEEL 2025 (`EQTRMT`) |
| **Unidades Consumidoras Atendidas** | 6.499 clientes de BT (6.263 residenciais) e 7 de MT | BDGD camadas `UCBT` e `UCMT` |
| **Geração Distribuída Solar no STO33** | **23 usinas fotovoltaicas ativas (195,89 kW)** | ANEEL MMGD 2026 (Cruzamento `CEG_GD`) |
| **Geração Distribuída Total em Santos** | 1.342 usinas ativas (16,89 MWp instalados) | ANEEL MMGD 2026 |
| **Frota de Veículos Elétricos (Santos)** | 4.593 eletrificados (1.101 plug-in: 589 BEV, 512 PHEV) | DETRAN-SP Set/2026 |
| **Equipamentos de Manobra (Chaves)** | 86 chaves (62 normalmente fechadas, 24 normalmente abertas) | BDGD camada `UNSEMT` |
| **Compensação Reativa** | 2 bancos de capacitores em MT (2 × 300 kvar = 600 kvar) | BDGD camada `BCMT` |
| **Enquadramento Territorial** | Circuito Intermunicipal: Subestação em Santos (IBGE 3548500), atendimento em conurbação com Cubatão (IBGE 3513504, 103 trafos no Jardim Casqueiro/Ilha Caraguatá) | Geocodificação BDGD / IBGE |

---

## 2. Resultados da Simulação no OpenDSS

O alimentador STO33 foi modelado e simulado em regime permanente e em simulação quase-estática temporal (QSTS de 24 horas, passo 1h) via `opendssdirect`:

### 2.1. Simulação Estática (Snapshot no Pico)
- **Status da Simulação:** CONVERGIU com sucesso em 4 iterações.
- **Potência Ativa Injetada na Subestação:** **2.982,62 kW (2,98 MW)**
- **Potência Reativa Injetada:** **437,91 kvar** (Fator de Potência = **0,989 indutivo**, com os 600 kvar de capacitores ativos).
- **Perdas Técnicas Totais no Pico:** **116,45 kW (3,90%)** da potência ativa injetada.
- **Tensão Máxima:** **1,0280 pu** (Barra da Subestação STO).
- **Tensão Mínima:** **0,9743 pu** (Extremidade do alimentador, nó `MT_STO33_166892`).
- **Conformidade Regulatória (PRODIST Módulo 8):** **100% dos nós em Faixa Adequada (0,93 pu a 1,05 pu)**.

### 2.2. Simulação Temporal Diária (QSTS 24 Horas)
- **Energia Ativa Total Injetada no Dia:** **29.413,03 kWh/dia (29,41 MWh/dia)**.
- **Perdas Técnicas Diárias Acumuladas:** **1.539,15 kWh/dia (5,23%)**.
- **Demanda Máxima de Ponta (Hora 20h):** **2.982,50 kW** ($V_{\min} = 0,9744\text{ pu}$).
- **Demanda Mínima de Madrugada (Hora 05h):** **658,14 kW** ($V_{\max} = 1,0288\text{ pu}$).
- **Risco de Fluxo Reverso na Subestação:** Inexistente no baseline (geração solar máxima de 170 kW representa 25,8% da carga mínima diurna de 658 kW).

---

## 3. Pesquisa COBRIC 2026: Avaliação dos 5 Cenários Técnicos (Resultados Atualizados da Missão 3)

Na **Missão 3**, os modelos de BESS foram reformulados com controle rigoroso de estado de carga ($10\% \le SOC \le 90\%$) e eficiência round-trip de 90,25%, garantindo fechamento perfeito de balanço de energia sem déficit diário:

1. **Cenário A (Baseline):** Rede atual com 195,89 kW de geração fotovoltaica cadastrada na ANEEL.
2. **Cenário B (Expansão FV + VE):** Dobra a capacidade solar (+195,89 kW) e adiciona **100 veículos elétricos (740 kW de carregadores residenciais de 7,4 kW)** alocados nos 20 maiores transformadores residenciais, com pico de recarga noturno sincronizado (curva `PerfilVE`).
3. **Cenário C (BESS Centralizado na Subestação):** BESS de **1,5 MW / 3,0 MWh** na barra de 13,8 kV da Subestação Santos 1, com despacho fisicamente controlado para corte de ponta (descarrega 2.200 kWh entre 19h e 21h; recarrega 2.210 kWh no vale noturno e pico solar).
4. **Cenário D (BESS Distribuído nos Transformadores Críticos):** **3 unidades BESS de 50 kW / 150 kWh (total 150 kW / 450 kWh)** conectadas na baixa tensão (0,22 kV) dos transformadores mais sobrecarregados (`TR_1193581`, `TR_1193923`, `TR_1193996`), com controle de $SOC$ independente por unidade.
5. **Cenário E (Reforço Físico Convencional — *Wires Alternative*):** Substituição dos 3 transformadores críticos por unidades de 300 kVA e recondutoramento de 188 trechos de tronco (5,25 km) para condutores de 575 A (código `PA6001` com menor resistência ôhmica $R_1$).

### Matriz de Indicadores Comparativos Simulada no OpenDSS (Missão 3)

| Indicador Técnico / Econômico | Cenário A (Baseline) | Cenário B (FV + VE) | Cenário C (BESS Centralizado) | Cenário D (BESS Distribuído) | Cenário E (Reforço Físico) |
|:---|:---:|:---:|:---:|:---:|:---:|
| **Demanda de Pico na Subestação (kW)** | 2.982,50 | 3.724,90 | **2.780,92** | 3.580,88 | 3.722,92 |
| **Hora do Pico Diário** | 20h | 20h | **19h** (20h: 2.774,84 kW) | 20h | 20h |
| **Alívio de Ponta vs Cenário B (kW / %)** | — | Referência | **-943,98 kW (-25,3%)** | -144,02 kW (-3,9%) | -1,98 kW (-0,05%) |
| **Demanda Mínima (kW)** | 658,14 | 665,61 | 767,07 | 665,68 | 665,53 |
| **Energia Diária Injetada (kWh/dia)** | 29.413,03 | 32.823,09 | 32.833,33 | 32.810,70 | 32.814,77 |
| **Perdas Técnicas Diárias (kWh/dia)** | 1.539,15 | 1.664,31 | 1.664,02 | **1.649,67** | 1.655,72 |
| **Percentual de Perdas Diárias (%)** | 5,23% | 5,07% | 5,07% | **5,03%** | 5,05% |
| **Tensão Mínima ($V_{\min}$, pu)** | 0,9744 | 0,9523 | 0,9530 | **0,9540** | 0,9525 |
| **Tensão Máxima ($V_{\max}$, pu)** | 1,0288 | 1,0288 | 1,0286 | 1,0288 | 1,0288 |
| **CAPEX Estimado da Solução (R$)** | R$ 0,00 | R$ 0,00 | R$ 4.950.000,00 | R$ 945.000,00 | **R$ 553.500,00** |
| **Convergência Numérica (100% passos)** | **SIM** | **SIM** | **SIM** | **SIM** | **SIM** |

### Conclusões Técnicas Centrais da Auditoria Física:
1. **BESS Centralizado (Cenário C):** É altamente eficiente para **corte de ponta sistêmico na subestação** (-944 kW), permitindo postergar obras no transformador de força AT/MT. Contudo, **NÃO reduz a sobrecarga nos transformadores de distribuição** (o carregamento do trafo `TR_1193581` permanece inalterado em 94,67%), pois a corrente continua fluindo pela rede secundária.
2. **BESS Distribuído (Cenário D):** É a alternativa mais equilibrada e eficaz para a rede local, aliviando o transformador mais crítico de **94,67% para 75,04%** (-19,63% de carregamento), alcançando as **menores perdas técnicas da rede (1.649,67 kWh/dia)** e o melhor perfil de tensão ($V_{\min} = 0,9540\text{ pu}$), com cerca de 19% do custo de investimento do BESS centralizado.
3. **Reforço Físico (Cenário E):** Elimina a sobrecarga dos transformadores por aumento de capacidade nominal (carregamento cai para 70,08%) com o menor CAPEX inicial (R$ 553,5k), porém possui ativo passivo sem flexibilidade operativa ou capacidade de armazenamento de excedentes solares.

---

## 4. Auditoria Técnica, Reprodutibilidade e Qualidade dos Dados

O repositório passou por três ciclos rigorosos de auditoria técnica documentados em `08_RELATORIOS/`:

### 4.1. Auditoria Técnica Geral e Rastreabilidade
- Relatório de Auditoria: [`08_RELATORIOS/AUDITORIA_TECNICA_STO33.md`](08_RELATORIOS/AUDITORIA_TECNICA_STO33.md)
- Matriz de Divergências: [`08_RELATORIOS/MATRIZ_DIVERGENCIAS_STO33.csv`](08_RELATORIOS/MATRIZ_DIVERGENCIAS_STO33.csv)
- Validação de Reprodutibilidade (Missão 2): [`08_RELATORIOS/validacao_reprodutibilidade/VALIDACAO_REPRODUTIBILIDADE_COBRIC.md`](08_RELATORIOS/validacao_reprodutibilidade/VALIDACAO_REPRODUTIBILIDADE_COBRIC.md)
- Comparação Matemática (Missão 2): [`08_RELATORIOS/validacao_reprodutibilidade/COMPARACAO_RESULTADOS_PUBLICADOS.csv`](08_RELATORIOS/validacao_reprodutibilidade/COMPARACAO_RESULTADOS_PUBLICADOS.csv)

### 4.2. Auditoria e Correção Física dos BESS (Missão 3 — Outubro/2026)
- Relatório Executivo da Missão 3: [`08_RELATORIOS/validacao_missao_3/RELATORIO_CORRECAO_BESS_COBRIC.md`](08_RELATORIOS/validacao_missao_3/RELATORIO_CORRECAO_BESS_COBRIC.md)
- Balanço Energético Detalhado dos BESS: [`08_RELATORIOS/validacao_missao_3/BALANCO_ENERGETICO_BESS.csv`](08_RELATORIOS/validacao_missao_3/BALANCO_ENERGETICO_BESS.csv)
- Comparação Antes vs Depois: [`08_RELATORIOS/validacao_missao_3/COMPARACAO_ANTES_DEPOIS.csv`](08_RELATORIOS/validacao_missao_3/COMPARACAO_ANTES_DEPOIS.csv)
- Validação Física e Elétrica: [`08_RELATORIOS/validacao_missao_3/VALIDACAO_FISICA_ELETRICA.md`](08_RELATORIOS/validacao_missao_3/VALIDACAO_FISICA_ELETRICA.md)
- Análise Econômica Atualizada: [`08_RELATORIOS/validacao_missao_3/ANALISE_ECONOMICA_ATUALIZADA.md`](08_RELATORIOS/validacao_missao_3/ANALISE_ECONOMICA_ATUALIZADA.md)

---

## 5. Estrutura Completa do Repositório

```bash
TELEMETRIA_SANTOS/
│
├── README.md                                <- Este documento de referência técnica auditada
├── .gitignore                               <- Exclusão de binários temporários
│
├── 01_FONTES_E_DOCUMENTACAO/                <- Metadados, dicionários de dados e normas
│   ├── catalogo_fontes.csv                  <- Catálogo detalhado das 10 fontes oficiais
│   ├── dicionarios_de_dados/                <- Manuais da ANEEL (BDGD, INDGER, GD, CTR)
│   └── documentos_tecnicos/                 <- Normas regulatórias e metodológicas
│
├── 02_DADOS_BRUTOS/                         <- Dados regulatórios de entrada
│   ├── README.md                            <- Guia de obtenção e download das bases ANEEL/DETRAN
│   └── frota_ativa_santos_setembro_2026.csv <- 20.752 veículos do DETRAN-SP para Santos
│
├── 03_DADOS_TRATADOS/                       <- Bases tratadas e filtradas para Santos e STO33
│   ├── alimentadores.csv                    <- 99 alimentadores de Santos (8 subestações)
│   ├── segmentos_rede.csv                   <- 730 trechos de MT com R1, X1, bitola e ampacidade
│   ├── nos_rede.csv                         <- 791 nós georreferenciados (Lon/Lat)
│   ├── transformadores.csv                  <- 105 transformadores (14.318 kVA auditados)
│   ├── equipamentos_controle.csv            <- 86 chaves seccionadoras + 2 bancos capacitores
│   ├── cargas.csv                           <- 105 pontos de carga agregada (kW e kvar)
│   ├── geracao_distribuida.csv              <- 23 usinas solares (195,89 kW auditados na ANEEL)
│   ├── geracao_distribuida_santos_total.csv <- 1.342 usinas solares em todo o município
│   ├── veiculos_eletricos.csv               <- Distribuição de veículos elétricos em Santos
│   ├── curvas_carga.csv                     <- 432 perfis horários normalizados da CPFL
│   └── indicadores_indger.csv               <- 58 meses de histórico de carregamento do STO33
│
├── 04_TOPOLOGIA/                            <- Grafos e Geometrias
│   ├── topologia_sto33.graphml              <- Grafo topológico completo (NetworkX, 793 nós)
│   ├── topologia_sto33.geojson              <- 924 feições georreferenciadas (linhas, nós, trafos)
│   ├── inventario_topologico.csv            <- Distâncias elétricas à subestação (Dijkstra)
│   └── mapa_sto33.html                      <- Mapa interativo Leaflet com camadas operacionais
│
├── 05_DIAGNOSTICO/                          <- Análise de Gargalos e Pontos Críticos
│   ├── pontos_criticos.csv                  <- Classificação regulatória de riscos (Classes 1 a 5)
│   ├── carregamento_alimentadores.csv       <- Análise comparativa dos 99 alimentadores
│   ├── lacunas_operacionais.csv             <- 5 lacunas cadastrais e minutas de ofício à CPFL
│   └── diagnostico_tecnico.md               <- Relatório técnico de diagnóstico e sobrecargas
│
├── 06_TELEMETRIA/                           <- Arquitetura SCADA / AMI / BESS
│   ├── dicionario_telemetria.csv            <- 26 variáveis elétricas e operacionais padronizadas
│   ├── matriz_disponibilidade.csv           <- Matriz de disponibilidade de dados (medido/cadastral)
│   ├── pontos_controle.csv                  <- 21 pontos candidatos para instalação de BESS e FLISR
│   └── arquitetura_telemetria.md            <- Arquitetura SCADA, ADMS, FLISR, AMI e BESS
│
├── 07_OPENDSS/                              <- Modelagem Computacional Elétrica
│   ├── dados_modelo/                        <- Tabelas tratadas formatadas para o OpenDSS
│   ├── modelo_parcial/                      <- Scripts base do alimentador STO33
│   └── cenarios_cobric/                     <- 5 Cenários Oficiais COBRIC 2026 (Modelos DSS)
│       ├── Cenario_A_Baseline.dss           <- Modelo da rede atual
│       ├── Cenario_B_Expansao_FV_VE.dss     <- Expansão FV (+100%) + 100 VEs (740 kW)
│       ├── Cenario_C_BESS_Centralizado.dss  <- BESS 1,5 MW / 3,0 MWh com controle físico de SOC
│       ├── Cenario_D_BESS_Distribuido.dss   <- 3 BESS de 50 kW / 150 kWh com SOC independente
│       ├── Cenario_E_Reforco_Fisico.dss     <- Upgrades de trafos para 300 kVA e cabos 575 A
│       └── run_cenarios_cobric.py           <- Executor QSTS automatizado dos 5 cenários
│
├── 08_RELATORIOS/                           <- Relatórios Técnicos, Executivos e Auditorias
│   ├── AUDITORIA_TECNICA_STO33.md           <- Relatório completo de auditoria do alimentador
│   ├── MATRIZ_DIVERGENCIAS_STO33.csv        <- Matriz estruturada de inconsistências e ações
│   ├── RESULTADOS_CENARIOS_COBRIC.csv       <- Tabela comparativa oficial dos 5 cenários
│   ├── CURVAS_HORARIAS_CENARIOS_COBRIC.csv  <- Séries temporais horárias (24h) dos 5 cenários
│   ├── validacao_reprodutibilidade/         <- Auditoria da Missão 2 (Reprodutibilidade Estrita)
│   │   ├── VALIDACAO_REPRODUTIBILIDADE_COBRIC.md
│   │   ├── COMPARACAO_RESULTADOS_PUBLICADOS.csv
│   │   ├── MATRIZ_RASTREABILIDADE_CENARIOS.csv
│   │   └── CHECKLIST_VALIDACAO_ELETRICA.md
│   └── validacao_missao_3/                  <- Auditoria da Missão 3 (Modelos Físicos de BESS)
│       ├── RELATORIO_CORRECAO_BESS_COBRIC.md
│       ├── RESULTADOS_CENARIOS_COBRIC_MISSAO_3.csv
│       ├── CURVAS_HORARIAS_COBRIC_MISSAO_3.csv
│       ├── BALANCO_ENERGETICO_BESS.csv
│       ├── COMPARACAO_ANTES_DEPOIS.csv
│       ├── VALIDACAO_FISICA_ELETRICA.md
│       ├── ANALISE_ECONOMICA_ATUALIZADA.md
│       └── README_MISSAO_3.md
│
└── 09_AUTOMACAO/                            <- Pipeline Automatizado em Python
    ├── requirements.txt                     <- Dependências do projeto (pyogrio, opendssdirect, etc.)
    ├── README_execucao.md                   <- Guia detalhado de execução
    └── scripts/                             <- Scripts modulares e orquestrador
```

---

## 6. Como Executar

### 6.1. Pré-requisitos
- Python 3.9+ instalado no sistema operacional.
- Dependências listadas em `09_AUTOMACAO/requirements.txt`:
```bash
pip install -r 09_AUTOMACAO/requirements.txt
```

### 6.2. Executar a Simulação dos 5 Cenários (Resultados Físicos Consolidados)
```bash
python3 07_OPENDSS/cenarios_cobric/run_cenarios_cobric.py
```

### 6.3. Executar o Orquestrador da Missão 3 (Validação Física e Balanço de SOC)
```bash
python3 08_RELATORIOS/validacao_missao_3/executar_missao_3.py
```

---

## 7. Autoria e Licença

Desenvolvido no âmbito do projeto de modelagem elétrica, telemetria e estudos de redes inteligentes da distribuição da Baixada Santista para submissão ao **COBRIC 2026**.
Dados de origem pública conforme Lei de Acesso à Informação (Lei 12.527/2011) e normas regulatórias da ANEEL.
