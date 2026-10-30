# 02 — Bases de Dados Brutas (Raw Data)

Este diretório armazena os dados brutos e conjuntos regulatórios oficiais obtidos diretamente dos portais da **ANEEL** e do **DETRAN-SP**.

---

## 1. Arquivos no Diretório

| Arquivo | Origem | Tamanho | Status no Git | Como Obter / Reprocessar |
|:---|:---|:---|:---|:---|
| `frota_ativa_santos_setembro_2026.csv` | DETRAN-SP | ~2.3 MB | Versionado | Baixado e filtrado via `02_download_frota_detran_santos.py` |
| `CPFL_Piratininga_2025.gdb.zip` | ANEEL / ArcGIS BDGD | ~419 MB | Ignorado (>100MB) | `python3 09_AUTOMACAO/scripts/01_download_dados_aneel.py` |
| `CPFL_Piratininga_2937_...gdb/` | ANEEL BDGD (descompactado) | ~400+ MB | Ignorado (>100MB) | Descompactado automaticamente no pipeline |
| `empreendimento-geracao-distribuida.parquet` | ANEEL Dados Abertos | ~102 MB | Ignorado (>100MB) | `python3 09_AUTOMACAO/scripts/01_download_dados_aneel.py` |
| `ctr-curvas-carga-consumidor-tipo.parquet` | ANEEL Dados Abertos | ~32 MB | Ignorado | `python3 09_AUTOMACAO/scripts/01_download_dados_aneel.py` |
| `ctr-curvas-carga-redes-tipo.parquet` | ANEEL Dados Abertos | ~17 MB | Ignorado | `python3 09_AUTOMACAO/scripts/01_download_dados_aneel.py` |
| `indger-dados-tecnicos-alimentadores.parquet` | ANEEL INDGER | ~16 MB | Ignorado | `python3 09_AUTOMACAO/scripts/01_download_dados_aneel.py` |
| `indger-dados-comerciais.parquet` | ANEEL INDGER | ~14 MB | Ignorado | `python3 09_AUTOMACAO/scripts/01_download_dados_aneel.py` |

> [!NOTE]
> Os arquivos em formato `.gdb` e `.parquet` com tamanhos superiores a 50 MB / 100 MB são ignorados pelo `.gitignore` para respeitar as políticas de armazenamento do GitHub e evitar lentidão na clonagem do repositório.
> 
> **Todos os dados tratados e filtrados para o município de Santos e para o alimentador STO33 já se encontram prontos e versionados no diretório [`03_DADOS_TRATADOS/`](../03_DADOS_TRATADOS/).**

---

## 2. Como Baixar os Dados Brutos Novamente

Para realizar o download automático de todas as bases regulatórias brutas diretamente dos servidores da ANEEL e do DETRAN-SP (com adaptação TLS 1.2 para contorno de limitações do portal da ANEEL), execute:

```bash
python3 09_AUTOMACAO/scripts/01_download_dados_aneel.py
python3 09_AUTOMACAO/scripts/02_download_frota_detran_santos.py
```
