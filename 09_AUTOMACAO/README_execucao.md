# GUIA DE EXECUÇÃO E AUTOMAÇÃO — PIPELINE COBRIC SANTOS

Este diretório contém os scripts Python para execução automatizada de todo o fluxo de engenharia reversa, download, processamento, modelagem topológica, diagnóstico e simulação no OpenDSS da rede de distribuição de Santos/SP (Alimentador STO33).

---

## 1. AMBIENTE E INSTALAÇÃO

Certifique-se de estar com o ambiente virtual ativado:

```bash
# Na raiz do projeto (/Users/guga/Desktop/COBRIC_Simulador_Visual):
source .venv/bin/activate
pip install -r COBRIC_TELEMETRIA_SANTOS/09_AUTOMACAO/requirements.txt
```

---

## 2. EXECUÇÃO COMPLETA (END-TO-END)

Para executar todas as 8 etapas em sequência automatizada:

```bash
python COBRIC_TELEMETRIA_SANTOS/09_AUTOMACAO/scripts/run_all_pipeline.py
```

---

## 3. DESCRIÇÃO INDIVIDUAL DOS SCRIPTS

| Script | Etapa | Descrição Técnica |
| :--- | :---: | :--- |
| `01_download_dados_aneel.py` | B | Download automatizado via TLS 1.2 dos arquivos da ANEEL (INDGER, MMGD, CTR e Dicionários de Dados em PDF). |
| `02_download_frota_detran_santos.py` | B | Streaming e filtro da base de 355 MB do DETRAN-SP, extraindo a frota ativa e eletrificada de Santos/SP. |
| `03_processar_dados_tratados.py` | C | Processamento das 43 camadas da BDGD e geração dos 10 datasets estruturados em `03_DADOS_TRATADOS/`. |
| `04_construir_topologia_mapas.py` | D | Construção do grafo em NetworkX, exportação de `topologia_sto33.graphml`, `topologia_sto33.geojson` e mapa interativo `mapa_sto33.html`. |
| `05_gerar_diagnostico_critico.py` | E | Classificação dos pontos críticos (Classes 1 a 5), análise de carregamento e mapeamento de lacunas operacionais. |
| `06_estruturar_telemetria.py` | F | Estruturação do catálogo de telemetria, matriz de disponibilidade e identificação de pontos de controle/BESS. |
| `07_construir_modelo_opendss.py` | G | Geração dos arquivos `.dss` (`Master.dss`, `Lines.dss`, `Transformers.dss`, `Loads.dss`, `PVSystems.dss`, `Capacitors.dss`, `LoadShapes.dss`). |
| `08_gerar_catalogo_fontes.py` | A | Consolidação do catálogo oficial de fontes e metadados regulatórios em `01_FONTES_E_DOCUMENTACAO/`. |
| `simulate_sto33.py` | G | Execução do fluxo de potência no OpenDSS via `opendssdirect` e extração de perdas e tensões nodais. |

---

## 4. VISUALIZAÇÃO DO MAPA INTERATIVO

O mapa completo da rede pode ser aberto diretamente em qualquer navegador web:

```bash
open COBRIC_TELEMETRIA_SANTOS/04_TOPOLOGIA/mapa_sto33.html
```
