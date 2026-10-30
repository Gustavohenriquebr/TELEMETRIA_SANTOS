# Telemetria Elétrica e Engenharia Reversa da Rede de Distribuição — Santos/SP
## Alimentador STO33 | Subestação STO (Santos 1) | CPFL Piratininga

[![Python](https://img.shields.io/badge/Python-3.9%2B-blue.svg)](https://www.python.org/)
[![OpenDSS](https://img.shields.io/badge/Simulation-OpenDSS-green.svg)](https://www.epri.com/pages/sa/opendss)
[![BDGD](https://img.shields.io/badge/Data-ANEEL%20BDGD%202025-orange.svg)](https://dadosabertos.aneel.gov.br/)
[![Status](https://img.shields.io/badge/Pipeline-100%25%20Concluído-brightgreen.svg)]()

Base técnica, analítica e de modelagem computacional para a reconstrução topológica, telemetria elétrica e diagnóstico da rede de média tensão do alimentador **STO33** da **CPFL Piratininga** no município de **Santos/SP**.

O projeto integra dados georreferenciados oficiais da **ANEEL (BDGD 2025, INDGER, MMGD, Curvas CTR)** e do **DETRAN-SP**, modelagem elétrica em **OpenDSS**, grafos em **NetworkX**, mapas interativos em **Folium/Leaflet**, e arquitetura de **telemetria para suporte a BESS, veículos elétricos e automação FLISR**.

---

## 1. Visão Geral e Indicadores Técnicos do Alimentador STO33

| Parâmetro Técnico | Valor / Especificação | Fonte / Método |
|:---|:---|:---|
| **Concessionária** | CPFL Piratininga (Distribuição) | Cadastro Oficial ANEEL |
| **Subestação de Origem** | SE STO (Santos 1) — 88/13.8 kV | BDGD ANEEL 2025 (`SUB`) |
| **Conjunto Elétrico** | Código 15884 | INDGER ANEEL 2025 (`CONJ`) |
| **Tensão Nominal Primária** | 13,8 kV (Trifásico) | BDGD ANEEL / Linhas de MT |
| **Extensão Total de MT** | 22,86 km (730 segmentos de linha) | BDGD camada `SSDMT` |
| **Nós Topológicos** | 793 nós georreferenciados | Topologia NetworkX / Dijkstra |
| **Conectividade Topológica** | Árvore estritamente radial (793 nós, 792 ramos) | Análise de Grafos / BDGD |
| **Transformadores de MT/BT** | 105 transformadores (13.8 / 0.22 kV) | BDGD camada `UNTRMT` |
| **Potência Instalada de Transformação** | 11.235 kVA total (média 107 kVA/trafo) | BDGD ANEEL 2025 |
| **Unidades Consumidoras Atendidas** | 6.499 clientes (classe residencial predominante) | BDGD camadas `UCBT` e `UCMT` |
| **Geração Distribuída Solar no STO33** | 23 usinas fotovoltaicas ativas (348,6 kWp instalados) | ANEEL MMGD 2026 |
| **Geração Distribuída Total em Santos** | 1.342 usinas ativas (18,74 MWp instalados) | ANEEL MMGD 2026 |
| **Frota de Veículos Elétricos (Santos)** | 1.284 veículos eletrificados (243 BEV, 627 PHEV, 414 HEV) | DETRAN-SP Set/2026 |
| **Equipamentos de Manobra (Chaves)** | 86 chaves (62 normalmente fechadas, 24 normalmente abertas) | BDGD camada `UNSEMT` |
| **Compensação Reativa** | 2 bancos de capacitores em MT (2 × 300 kvar = 600 kvar) | BDGD camada `BCMT` |

---

## 2. Resultados da Simulação de Fluxo de Potência (OpenDSS)

O alimentador STO33 foi modelado e simulado em regime permanente utilizando o motor oficial **OpenDSS** (via `opendssdirect`):

- **Status da Simulação:** CONVERGIU com sucesso em 4 iterações.
- **Potência Ativa Injetada na Subestação:** **2.936,64 kW (2,937 MW)**
- **Potência Reativa Injetada:** **642,81 kvar** (Fator de Potência = 0,977 indutivo)
- **Perdas Técnicas Totais:** **114,83 kW (3,91%)** da potência ativa injetada.
- **Tensão Máxima:** **1,0248 pu** (Barra da Subestação STO).
- **Tensão Mínima:** **0,9747 pu** (Extremidade sul do circuito, nó P11738).
- **Conformidade Regulatória (PRODIST Módulo 8):** **100% dos nós em Faixa Adequada (0,93 pu a 1,05 pu)**.
- **Risco de Fluxo Reverso:** Inexistente no patamar de pico (penetração de GD solar de 11,8% da potência de ponta).

---

## 3. Estrutura Completa do Repositório

```bash
TELEMETRIA_SANTOS/
│
├── README.md                                <- Este documento de referência técnica
├── .gitignore                               <- Exclusão de binários pesados (>100MB)
│
├── 01_FONTES_E_DOCUMENTACAO/                <- Metadados, dicionários de dados e normas
│   ├── catalogo_fontes.csv                  <- Catálogo detalhado das 10 fontes oficiais
│   ├── dicionarios_de_dados/                <- 6 PDFs com manuais da ANEEL (BDGD, INDGER, GD, CTR)
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
│   ├── transformadores.csv                  <- 105 transformadores com cálculo de carregamento
│   ├── equipamentos_controle.csv            <- 86 chaves seccionadoras + 2 bancos capacitores
│   ├── cargas.csv                           <- 105 pontos de carga agregada (kW e kvar)
│   ├── geracao_distribuida.csv              <- 23 usinas solares no alimentador STO33
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
│   ├── modelo_parcial/                      <- Scripts e componentes OpenDSS do STO33
│   │   ├── Master.dss                       <- Script mestre de simulação
│   │   ├── Circuit.dss                      <- Barra infinita e equivalente de curto-circuito
│   │   ├── LineCodes.dss                    <- Parâmetros de sequência (R1, X1, R0, X0)
│   │   ├── Lines.dss                        <- 792 trechos (730 condutores + 62 chaves fechadas)
│   │   ├── Transformers.dss                 <- 105 transformadores (13.8/0.22 kV, perdas %)
│   │   ├── Loads.dss                        <- 94 cargas trifásicas agregadas
│   │   ├── PVSystems.dss                    <- 23 usinas solares fotovoltaicas
│   │   ├── Capacitors.dss                   <- 2 bancos de capacitores (2x 300 kvar)
│   │   ├── LoadShapes.dss                   <- Curvas diárias normalizadas (24 horas)
│   │   └── simulate_sto33.py                <- Script de execução via opendssdirect
│   └── README_modelagem.md                  <- Documentação técnica da modelagem
│
├── 08_RELATORIOS/                           <- Relatórios Executivos e Técnicos
│   ├── relatorio_executivo.md               <- Respostas formais às 20 perguntas do projeto
│   ├── relatorio_tecnico_completo.md        <- Relatório de engenharia detalhado em 12 seções
│   ├── metodologia_e_limitacoes.md          <- Metodologia de engenharia reversa e níveis A-G
│   └── fontes_e_referencias.md              <- Catálogo bibliográfico e normas vigentes
│
└── 09_AUTOMACAO/                            <- Pipeline Automatizado em Python
    ├── requirements.txt                     <- Dependências do projeto (pyogrio, opendssdirect, etc.)
    ├── README_execucao.md                   <- Guia detalhado de execução
    └── scripts/                             <- 9 scripts modulares e orquestrador
        ├── 01_download_dados_aneel.py       <- Download oficial ANEEL com adaptador TLS 1.2
        ├── 02_download_frota_detran_santos.py <- Download e filtro da frota do DETRAN-SP
        ├── 03_processar_dados_tratados.py   <- Extração da BDGD e geração dos 10 CSVs
        ├── 04_construir_topologia_mapas.py  <- Construção do grafo NetworkX e mapa Folium
        ├── 05_gerar_diagnostico_critico.py  <- Diagnóstico de sobrecargas e pontos críticos
        ├── 06_estruturar_telemetria.py      <- Dicionário e arquitetura de telemetria
        ├── 07_construir_modelo_opendss.py   <- Geração automática dos scripts OpenDSS (.dss)
        ├── 08_gerar_catalogo_fontes.py      <- Criação do catálogo de fontes
        └── run_all_pipeline.py              <- Master runner que executa os 9 estágios
```

---

## 4. Como Executar

### 4.1. Pré-requisitos
- Python 3.9+ instalado no sistema operacional.
- Dependências listadas em `09_AUTOMACAO/requirements.txt`:
```bash
pip install -r 09_AUTOMACAO/requirements.txt
```

### 4.2. Executar a Simulação em OpenDSS
Para simular o fluxo de potência do alimentador STO33 e verificar tensões, perdas e carregamento:
```bash
python3 07_OPENDSS/modelo_parcial/simulate_sto33.py
```

### 4.3. Executar o Pipeline Completo de Engenharia Reversa
Para reprocessar todos os dados tratados, reconstruir a topologia, gerar os mapas, diagnósticos e modelos OpenDSS:
```bash
python3 09_AUTOMACAO/scripts/run_all_pipeline.py
```

---

## 5. Visualização do Mapa Interativo

Abra o arquivo [`04_TOPOLOGIA/mapa_sto33.html`](04_TOPOLOGIA/mapa_sto33.html) em qualquer navegador web para explorar:
- **Subestação STO (Santos 1):** Ponto de injeção de potência.
- **Rede de Média Tensão (730 trechos):** Representados com espessura proporcional à ampacidade do condutor.
- **Transformadores de Distribuição (105 trafos):** Cores indicativas de nível de potência instalada.
- **Equipamentos de Manobra (86 chaves):** Diferenciadas entre Normalmente Fechadas (verde) e Normalmente Abertas (vermelho).
- **Usinas Solares (23 sistemas PV):** Marcadas com detalhes de potência de pico e classe de tensão.

---

## 6. Autoria e Licença

Desenvolvido no âmbito do projeto de modelagem elétrica, telemetria e estudos de redes inteligentes da distribuição da Baixada Santista.
Dados de origem pública conforme Lei de Acesso à Informação (Lei 12.527/2011) e normas da ANEEL.
