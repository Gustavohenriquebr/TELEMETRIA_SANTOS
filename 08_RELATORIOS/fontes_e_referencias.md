# FONTES OFICIAIS, ENDPOINTS E REFERÊNCIAS REGULATÓRIAS
## BASE DE DADOS DO PROJETO COBRIC — SANTOS/SP

---

## 1. REPOSITÓRIOS OFICIAIS CONSULTADOS E BAIXADOS

### 1.1 Agência Nacional de Energia Elétrica (ANEEL)
- **Portal de Dados Abertos da ANEEL:**  
  URL: https://dadosabertos.aneel.gov.br/
- **Base de Dados Geográfica da Distribuidora (BDGD) — CPFL Piratininga:**  
  *Dataset:* `base-de-dados-geografica-da-distribuidora-bdgd`  
  *Arquivo Oficial:* `CPFL_Piratininga_2937_2025-12-31_V11_20260823-1826.gdb.zip` (Tamanho: 419,3 MB)  
  *Endpoint de Download Direto:* https://www.arcgis.com/sharing/rest/content/items/75863ef447904bbdacc1435e7f47d43a/data  
  *Data de Referência:* 31/12/2025 (Submissão regulatória homologada em agosto de 2026).
- **Indicadores Gerenciais da Distribuição (INDGER):**  
  *Dataset:* `indger-indicadores-gerenciais-da-distribuicao`  
  *Recurso:* `indger-dados-tecnicos-alimentadores.parquet` (15,55 MB)  
  *Endpoint:* https://dadosabertos.aneel.gov.br/dataset/7cacb2c4-b165-4591-a793-9ed20d1f167d/resource/8c05a6d2-c6d4-48d6-9cc9-f805652eea2e/download/indger-dados-tecnicos-alimentadores.parquet
- **Relação de Empreendimentos de Geração Distribuída:**  
  *Dataset:* `relacao-de-empreendimentos-de-geracao-distribuida`  
  *Recurso:* `empreendimento-geracao-distribuida.parquet` (102,18 MB)  
  *Endpoint:* https://dadosabertos.aneel.gov.br/dataset/5e0fafd2-21b9-4d5b-b622-40438d40aba2/resource/cd29f6eb-e08d-4db7-b6fb-ed6e3b682d27/download/empreendimento-geracao-distribuida.parquet
- **Sistema de Curvas de Carga (CTR — Campanha de Medição):**  
  *Dataset:* `ctr-curva-de-carga`  
  *Recurso:* `ctr-curvas-carga-consumidor-tipo.parquet` (32,19 MB)  
  *Endpoint:* https://dadosabertos.aneel.gov.br/dataset/2594ebad-1306-49d1-9f30-ded144acca87/resource/fae0d52c-5f02-4e84-a11b-d03db87e990d/download/ctr-curvas-carga-consumidor-tipo.parquet

### 1.2 Departamento Estadual de Trânsito de São Paulo (DETRAN-SP)
- **Portal de Dados Abertos do Estado de São Paulo — Frota Ativa:**  
  *Dataset:* `frota-ativa`  
  *Recurso Utilizado:* `frota_ativa_sp.csv` (Referência: Setembro de 2026)  
  *Endpoint de Streaming:* https://0010068detrandadoabprdsa.blob.core.windows.net/detran-public/dados-abertos/frota/Sep-2026/30/frota_ativa_sp.csv  
  *Filtro Aplicado:* Município de Santos (20.752 registros consolidados).

### 1.3 Instituto Brasileiro de Geografia e Estatística (IBGE)
- **API de Serviços e Localidades — Município de Santos (Código 3548500):**  
  *Endpoint:* https://servicodados.ibge.gov.br/api/v1/localidades/municipios/3548500
- **Censo Demográfico 2022 (População e Domicílios):**  
  *Endpoint:* https://servicodados.ibge.gov.br/api/v3/agregados/4714/periodos/2022/variaveis/93?localidades=N6[3548500]

### 1.4 Instituto Nacional de Pesquisas Espaciais (INPE)
- **Atlas Brasileiro de Energia Solar (2ª Edição):**  
  *Laboratório de Modelagem e Estudos de Recursos Renováveis de Energia (LABREN/CCST/INPE).*  
  *Acesso:* http://labren.ccst.inpe.br/atlas_2017.html

---

## 2. NORMAS TÉCNICAS E REGULAMENTAÇÕES CONSULTADAS

1. **ANEEL Resolução Normativa nº 956/2021:** Procedimentos de Distribuição de Energia Elétrica no Sistema Elétrico Nacional (PRODIST).
   - *Módulo 8:* Qualidade da Energia Elétrica.
   - *Módulo 10:* Sistema de Informação Geográfica Regulatório (SIG-R / BDGD).
2. **ANEEL Resolução Normativa nº 1.000/2021:** Consolidação das Regras de Prestação do Serviço Público de Distribuição de Energia Elétrica.
3. **Lei nº 14.300/2022:** Marco Legal da Microgeração e Minigeração Distribuída.
4. **IEEE Std 1547-2018:** IEEE Standard for Interconnection and Interoperability of Distributed Energy Resources with Associated Electric Power Systems Interfaces.
5. **EPRI (Electric Power Research Institute):** OpenDSS (Open Distribution System Simulator) Reference Guide, Version 9.x.
