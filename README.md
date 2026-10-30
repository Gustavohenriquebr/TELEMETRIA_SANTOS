# Telemetria Elétrica e Engenharia Reversa da Rede de Distribuição — Santos/SP
## Alimentador STO33 | Subestação STO (Santos 1) | CPFL Piratininga | Pesquisa COBRIC 2026

[![Python](https://img.shields.io/badge/Python-3.9%2B-blue.svg)](https://www.python.org/)
[![OpenDSS](https://img.shields.io/badge/Simulation-OpenDSS%20QSTS-green.svg)](https://www.epri.com/pages/sa/opendss)
[![BDGD](https://img.shields.io/badge/Data-ANEEL%20BDGD%202025-orange.svg)](https://dadosabertos.aneel.gov.br/)
[![COBRIC](https://img.shields.io/badge/Pesquisa-COBRIC%202026-purple.svg)]()
[![Status](https://img.shields.io/badge/Auditoria%20e%20Pipeline-100%25%20Auditado-brightgreen.svg)]()

Base técnica, analítica e de modelagem computacional para a reconstrução topológica, telemetria elétrica, auditoria técnica e diagnóstico da rede de média tensão do alimentador **STO33** da **CPFL Piratininga** (Subestação Santos 1).

O projeto integra dados georreferenciados oficiais da **ANEEL (BDGD 2025, INDGER, MMGD, Curvas CTR)** e do **DETRAN-SP**, modelagem elétrica quase-estática temporal (QSTS de 24h) em **OpenDSS**, grafos em **NetworkX**, mapas interativos em **Folium/Leaflet**, e o estudo comparativo de **5 cenários para o Congresso Brasileiro de Iniciação Científica (COBRIC 2026)** avaliando a integração de sistemas fotovoltaicos (FV), veículos elétricos (VE), sistemas de armazenamento de energia em baterias (BESS centralizado e distribuído) e reforço físico de rede.

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

## 3. Pesquisa COBRIC 2026: Avaliação dos 5 Cenários Técnicos

Para o artigo do COBRIC 2026, foram implementados e simulados 5 cenários operacionais para comparar os impactos técnicos da expansão de recursos energéticos distribuídos (REDs) e soluções de flexibilidade:

1. **Cenário A (Baseline):** Rede atual com 195,89 kW de geração fotovoltaica cadastrada na ANEEL.
2. **Cenário B (Expansão FV + VE):** Dobra a capacidade solar (+195,89 kW) e adiciona **100 veículos elétricos (740 kW de carregadores residenciais de 7,4 kW)** alocados nos 20 maiores transformadores residenciais, com pico de recarga noturno sincronizado (curva `PerfilVE`).
3. **Cenário C (BESS Centralizado na Subestação):** Adiciona sistema BESS de **1,5 MW / 3,0 MWh** na barra de 13,8 kV da Subestação Santos 1, com despacho programado para corte de ponta (peak shaving de 1.000 kW entre 19h e 21h) e recarga no vale noturno e pico solar.
4. **Cenário D (BESS Distribuído nos Transformadores Críticos):** Instalação de **3 BESS de 50 kW / 150 kWh (total 150 kW / 450 kWh)** conectados na baixa tensão (0,22 kV) dos transformadores mais sobrecarregados (`TR_1193581`, `TR_1193923`, `TR_1193996`).
5. **Cenário E (Reforço Físico Convencional — *Wires Alternative*):** Substituição dos 3 transformadores críticos por unidades de 300 kVA e recondutoramento de 4,5 km do tronco primário para cabos de 575 A (menor resistência ôhmica $R_1$).

### Matriz de Indicadores Comparativos Simulada no OpenDSS

| Indicador Técnico / Econômico | Cenário A (Baseline) | Cenário B (FV + VE) | Cenário C (BESS Centralizado) | Cenário D (BESS Distribuído) | Cenário E (Reforço Físico) |
|:---|:---:|:---:|:---:|:---:|:---:|
| **Demanda de Pico na Subestação (kW)** | 2.982,50 | 3.724,90 | **2.724,82** | 3.580,86 | 3.722,92 |
| **Hora do Pico Diário** | 20h | 20h | 20h | 20h | 20h |
| **Alívio de Ponta vs Cenário B (kW / %)** | — | Referência | **-1.000,08 kW (-26,8%)** | -144,04 kW (-3,9%) | -1,98 kW (-0,05%) |
| **Demanda Mínima (kW)** | 658,14 | 665,61 | 665,57 | 665,68 | 665,53 |
| **Energia Diária Injetada (kWh/dia)** | 29.413,03 | 32.823,09 | 31.622,81 | 32.714,18 | 32.814,77 |
| **Perdas Técnicas Diárias (kWh/dia)** | 1.539,15 | 1.664,31 | 1.664,00 | **1.645,01** | 1.655,72 |
| **Percentual de Perdas Diárias (%)** | 5,23% | 5,07% | 5,26% | **5,03%** | 5,05% |
| **Tensão Mínima ($V_{\min}$, pu)** | 0,9744 | 0,9523 | 0,9531 | **0,9540** | 0,9525 |
| **Tensão Máxima ($V_{\max}$, pu)** | 1,0288 | 1,0288 | 1,0288 | 1,0288 | 1,0288 |
| **CAPEX Estimado da Solução (R$)** | R$ 0,00 | R$ 0,00 | R$ 4.950.000,00 | R$ 945.000,00 | **R$ 553.500,00** |

### Conclusões Técnicas da Comparação:
- **Impacto do Cenário B (Crescimento de VEs):** A penetração não coordenada de 100 veículos elétricos eleva a demanda de pico em **+742,40 kW (+24,9%)**, rebaixa a tensão mínima para 0,9523 pu e acentua o risco térmico nos transformadores secundários.
- **BESS Centralizado (Cenário C):** É a solução mais eficaz para **corte de ponta na subestação** (-1.000 kW), permitindo postergar ampliações de subestação e transformadores de força AT/MT, porém exige alto investimento (R$ 4,95M) e não reduz as perdas ao longo dos 19,37 km do alimentador.
- **BESS Distribuído (Cenário D):** Apresenta o **melhor desempenho técnico local**, alcançando as **menores perdas elétricas da rede (1.645 kWh/dia, alívio de 19,3 kWh/dia)** e o melhor suporte de tensão ($V_{\min} = 0,9540\text{ pu}$), aliviando diretamente os transformadores críticos por injetar energia diretamente na baixa tensão com 1/5 do custo do BESS centralizado.
- **Reforço Físico (Cenário E):** Representa a alternativa convencional mais barata (R$ 553,5k), eliminando o gargalo térmico dos transformadores, mas sem oferecer flexibilidade operacional, armazenamento de energia ou capacidade de absorção de excedentes solares.

---

## 4. Auditoria Técnica e Qualidade dos Dados

Em conformidade com as diretrizes de integridade científica do COBRIC 2026, o repositório passou por uma auditoria técnica completa documentada em:
- Relatório de Auditoria: [`08_RELATORIOS/AUDITORIA_TECNICA_STO33.md`](08_RELATORIOS/AUDITORIA_TECNICA_STO33.md)
- Matriz de Divergências: [`08_RELATORIOS/MATRIZ_DIVERGENCIAS_STO33.csv`](08_RELATORIOS/MATRIZ_DIVERGENCIAS_STO33.csv)
- Resultados Consolidados: [`08_RELATORIOS/RESULTADOS_CENARIOS_COBRIC.csv`](08_RELATORIOS/RESULTADOS_CENARIOS_COBRIC.csv)
- Séries Horárias de 24h: [`08_RELATORIOS/CURVAS_HORARIAS_CENARIOS_COBRIC.csv`](08_RELATORIOS/CURVAS_HORARIAS_CENARIOS_COBRIC.csv)

### Principais Correções e Rastreabilidade Comprovada:
1. **Potência Fotovoltaica:** Corrigida de 44,35 kW para **195,89 kW**, após cruzamento individual de cada código `CEG_GD` com o cadastro oficial da ANEEL MMGD.
2. **Capacidade dos Transformadores:** Corrigida para **14.318,0 kVA** (105 transformadores), eliminando a omissão anterior do transformador de 25 kVA (`1194151`).
3. **Extensão Física de MT:** Ajustada para **19,37 km** (soma auditada do campo `COMP` da BDGD).
4. **Reativos da Subestação:** Retificados para **434,89 kvar** ($FP = 0,989$ indutivo) com capacitores em operação.
5. **Contexto Geográfico:** Comprovado documentalmente que, embora a subestação pertença ao cadastro de Santos, 98,1% da rede física atende a área conurbada do Jardim Casqueiro (Cubatão/SP).

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
│   │   ├── pvsystems_opendss.csv            <- 23 usinas solares com 195,89 kW auditados
│   │   └── resultado_fluxo_diario_sto33.csv <- Curva diária de 24h simulada no alimentador
│   ├── modelo_parcial/                      <- Scripts e componentes OpenDSS do STO33
│   │   ├── Master.dss                       <- Script mestre com EnergyMeter e Monitores
│   │   ├── Circuit.dss                      <- Barra infinita e equivalente de curto-circuito
│   │   ├── LineCodes.dss                    <- Parâmetros de sequência (R1, X1, R0, X0)
│   │   ├── Lines.dss                        <- 792 trechos (730 condutores + 62 chaves fechadas)
│   │   ├── Transformers.dss                 <- 105 transformadores (14.318 kVA totais)
│   │   ├── Loads.dss                        <- 94 cargas trifásicas agregadas
│   │   ├── PVSystems.dss                    <- 23 usinas solares fotovoltaicas (195,89 kW)
│   │   ├── Capacitors.dss                   <- 2 bancos de capacitores (2x 300 kvar)
│   │   ├── LoadShapes.dss                   <- Curvas diárias normalizadas (24 horas)
│   │   └── simulate_sto33.py                <- Simulação Snapshot e QSTS diária (24h)
│   ├── cenarios_cobric/                     <- Cenários de Estudo para o Artigo do COBRIC 2026
│   │   ├── Cenario_A_Baseline.dss           <- Modelo da rede atual
│   │   ├── Cenario_B_Expansao_FV_VE.dss     <- Expansão FV (+100%) + 100 VEs (740 kW)
│   │   ├── Cenario_C_BESS_Centralizado.dss  <- BESS 1,5 MW / 3,0 MWh na Subestação
│   │   ├── Cenario_D_BESS_Distribuido.dss   <- 3 BESS de 50 kW / 150 kWh nos trafos críticos
│   │   ├── Cenario_E_Reforco_Fisico.dss     <- Upgrades de trafos para 300 kVA e cabos 575 A
│   │   └── run_cenarios_cobric.py           <- Executor QSTS automatizado dos 5 cenários
│   └── README_modelagem.md                  <- Documentação técnica da modelagem
│
├── 08_RELATORIOS/                           <- Relatórios Executivos, Técnicos e Auditoria
│   ├── AUDITORIA_TECNICA_STO33.md           <- Relatório completo de auditoria do alimentador
│   ├── MATRIZ_DIVERGENCIAS_STO33.csv        <- Matriz estruturada de inconsistências e ações
│   ├── RESULTADOS_CENARIOS_COBRIC.csv       <- Tabela comparativa dos 5 cenários COBRIC 2026
│   ├── CURVAS_HORARIAS_CENARIOS_COBRIC.csv  <- Séries temporais horárias (24h) dos 5 cenários
│   ├── relatorio_executivo.md               <- Respostas formais às 20 perguntas do projeto
│   ├── relatorio_tecnico_completo.md        <- Relatório de engenharia detalhado em 12 seções
│   ├── metodologia_e_limitacoes.md          <- Metodologia de engenharia reversa e níveis A-G
│   └── fontes_e_referencias.md              <- Catálogo bibliográfico e normas vigentes
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

### 6.2. Executar a Simulação do Alimentador STO33 (Snapshot + 24h QSTS)
Para simular o fluxo de carga estático e a curva diária horária:
```bash
python3 07_OPENDSS/modelo_parcial/simulate_sto33.py
```

### 6.3. Executar a Simulação Comparativa dos 5 Cenários do COBRIC 2026
Para rodar a simulação temporal horária de 24 horas dos 5 cenários e gerar as tabelas comparativas:
```bash
python3 07_OPENDSS/cenarios_cobric/run_cenarios_cobric.py
```
Os resultados são consolidados automaticamente em:
- `08_RELATORIOS/RESULTADOS_CENARIOS_COBRIC.csv`
- `08_RELATORIOS/CURVAS_HORARIAS_CENARIOS_COBRIC.csv`

---

## 7. Visualização do Mapa Interativo

Abra o arquivo [`04_TOPOLOGIA/mapa_sto33.html`](04_TOPOLOGIA/mapa_sto33.html) em qualquer navegador web para explorar:
- **Subestação STO (Santos 1):** Ponto de injeção de potência.
- **Rede de Média Tensão (730 trechos):** Representados com espessura proporcional à ampacidade do condutor.
- **Transformadores de Distribuição (105 trafos):** Cores indicativas de nível de potência instalada.
- **Equipamentos de Manobra (86 chaves):** Diferenciadas entre Normalmente Fechadas (verde) e Normalmente Abertas (vermelho).
- **Usinas Solares (23 sistemas PV):** Marcadas com detalhes de potência de pico e classe de tensão.

---

## 8. Autoria e Licença

Desenvolvido no âmbito do projeto de modelagem elétrica, telemetria e estudos de redes inteligentes da distribuição da Baixada Santista para submissão ao **COBRIC 2026**.
Dados de origem pública conforme Lei de Acesso à Informação (Lei 12.527/2011) e normas regulatórias da ANEEL.
