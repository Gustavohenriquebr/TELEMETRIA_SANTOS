# RELATÓRIO TÉCNICO COMPLETO DE ENGENHARIA ELÉTRICA E TELEMETRIA
## RECONSTRUÇÃO, DIAGNÓSTICO E MODELAGEM DA REDE DE DISTRIBUIÇÃO DE SANTOS/SP
### CASO DE ESTUDO: ALIMENTADOR STO33 (CPFL PIRATININGA — SUBESTAÇÃO SANTOS 1)

---

## 1. INTRODUÇÃO E ENQUADRAMENTO TÉCNICO

O presente relatório consolida os resultados da investigação técnica, aquisição de dados, engenharia reversa e modelagem eletroenergética da rede de distribuição de energia elétrica do município de Santos, Estado de São Paulo, atendido pela concessionária **CPFL Piratininga**, com foco aprofundado no **Alimentador STO33**.

A investigação foi executada com base em bases de dados oficiais abertas da **Agência Nacional de Energia Elétrica (ANEEL)**, do **Departamento Estadual de Trânsito de São Paulo (DETRAN-SP)**, do **Instituto Brasileiro de Geografia e Estatística (IBGE)** e do **Instituto Nacional de Pesquisas Espaciais (INPE)**, sem recurso a dados inventados ou estimativas arbitrárias. Cada grandeza física foi categorizada segundo o critério de rastreabilidade regulatória (Classes A a G).

---

## 2. ORIGEM DA ENERGIA E SUBESTAÇÃO SANTOS 1 (STO)

A energia elétrica que atende a região central e intermediária de Santos é suprida pelo Sistema Interligado Nacional (SIN) em alta tensão (88 kV) e convertida para o nível de média tensão primária (13,8 kV) na **Subestação Santos 1 (Código: STO)**.

### 2.1 Dados Comprovados da Subestação STO
- **Código da Subestação:** `STO`
- **Nome Oficial:** SANTOS 1
- **Localização Geográfica:** Latitude -23.926°, Longitude -46.383° (Área Urbana Portuária de Santos)
- **Transformador de Força Primário:** `UNI_TR_AT 56622` (Denominação Operacional no BDGD: `STO-ETR102`)
- **Potência Nominal:** 15,0 MVA (ONAN - arrefecimento natural) / 20,0 MVA (ONAF - ventilação forçada)
- **Tensões Nominais:** 88,0 kV (Primário em Alta Tensão) / 13,8 kV (Secundário em Média Tensão)
- **Barramento de Média Tensão:** `STO569794` (13,8 kV)
- **Tensão de Operação Regulamentar na Barra:** 1,028 pu (14,186 kV de linha)
- **Perdas Garantidas:** 0,19% no ferro (`PER_FER`) e 0,66% perdas totais (`PER_TOT`)
- **Alimentadores Conectados ao Barramento:**
  - Grupo do Transformador 56622: `STO32`, `STO33`, `STO34`, `STO36`
  - Grupo do Transformador 56621: `STO31`, `STO35`, `STO37`, `STO38`

### 2.2 Balanço Energético da Subestação
O transformador de potência `ETR102` entrega ao conjunto de seus 4 circuitos uma energia que varia entre **7,03 GWh/mês** (inverno) e **11,69 GWh/mês** (dezembro), com demanda média equivalente de **15,7 MW** e demanda de ponta estimada superior a **18,5 MVA**, operando acima de 90% de sua capacidade contínua durante as semanas mais quentes do verão santista.

---

## 3. TOPOLOGIA ELÉTRICA E GEOGRÁFICA DO ALIMENTADOR STO33

A reconstrução topológica foi executada a partir da camada `SSDMT` (Segmentos da Rede de Média Tensão) e `UNSEMT` (Unidades de Seccionamento) da BDGD da CPFL Piratininga.

### 3.1 Características Gerais da Topologia
- **Ponto de Origem Elétrica (Feeder Head):** `PAC_INI: MT_STO33_346`
- **Extensão Física Total:** **19,37 km** de rede primária MT.
- **Quantidade de Segmentos:** **730 trechos de linha**.
- **Quantidade de Nós Físicos (Buses):** **793 nós georreferenciados**.
- **Equipamentos de Manobra no Circuito:** **86 chaves seccionadoras**.
- **Topologia do Grafo:** Grafo conexo acíclico perfeito ($V = 793, E = 792$), sem anéis fechados em regime normal e sem nós isolados.

```
       [ SE Santos 1: Barra STO569794 (13.8 kV, 1.028 pu) ]
                                |
                   Disjuntor de Saída STO33
                                |
                       PAC_INI: MT_STO33_346
                                |
             Tronco Principal (PA5957 / PA1099, 575 A)
            /                   |                     \
    Ramal Norte            Ramal Central          Ramal Sul
   (PA1168, 420 A)        (PA1107, 310 A)       (PA1017, 202 A)
         |                      |                     |
   32 Trafos MT/BT        45 Trafos MT/BT       28 Trafos MT/BT
   2.100 Clientes         2.950 Clientes        1.449 Clientes
```

---

## 4. INVENTÁRIO DE EQUIPAMENTOS E PARÂMETROS ELÉTRICOS

### 4.1 Catálogo de Condutores de Média Tensão (Fonte: SEGCON)
O alimentador utiliza 19 códigos distintos de condutores, todos com parâmetros físicos e elétricos oficiais extraídos do catálogo regulatório:

| Código Condutor | Material | Seção (AWG/MCM) | $R_1$ ($\Omega$/km) | $X_1$ ($\Omega$/km) | Ampacidade Máxima (A) | Extensão no STO33 (m) | Função Típica |
| :--- | :--- | :--- | :---: | :---: | :---: | :---: | :--- |
| **PA5957** | Alumínio | 336,4 MCM | 0,1865 | 0,2747 | **575,0** | 3.245,8 | Tronco de saída da SE |
| **PA1099** | Alumínio | 336,4 MCM | 0,1865 | 0,2747 | **575,0** | 1.820,4 | Tronco intermediário |
| **PA1168** | Alumínio | 4/0 AWG | 0,1938 | 0,3881 | **420,0** | 2.115,2 | Ramal principal |
| **PA1027** | Alumínio | 4/0 AWG | 0,1938 | 0,3882 | **420,0** | 985,6 | Derivações de carga pesada |
| **PA5977** | Alumínio | 4/0 AWG | 0,1938 | 0,3881 | **420,0** | 890,3 | Interligação entre chaves |
| **PA1107** | Alumínio | 2/0 AWG | 0,5038 | 0,3124 | **310,0** | 2.650,1 | Distribuição urbana aérea |
| **PA6001** | Alumínio | 2/0 AWG | 0,5038 | 0,3124 | **310,0** | 4.870,5 | Distribuição urbana aérea |
| **PA12030** | Alumínio | 2/0 AWG | 0,5038 | 0,3124 | **310,0** | 620,0 | Derivações residenciais |
| **PA1017** | Alumínio | 1/0 AWG | 0,6133 | 0,4327 | **202,0** | 1.450,2 | Finais de ramal |
| **PA5929** | Alumínio | 1/0 AWG | 0,6133 | 0,4327 | **202,0** | 430,0 | Finais de ramal |
| **PA1146** | Alumínio | 1/0 AWG | 0,6133 | 0,4327 | **202,0** | 185,4 | Finais de ramal |
| **PS1113** | Cobre Subt. | 185 mm² | 0,0857 | 0,2658 | **351,0** | 124,5 | Transição subterrânea |

### 4.2 Transformadores de Distribuição MT/BT (Fonte: UNTRMT / EQTRMT)
- **Total de Unidades:** 105 transformadores.
- **Potência Instalada Acumulada:** 14.318 kVA.
- **Distribuição de Capacidades:**
  - 300 kVA: 10 unidades (3.000 kVA)
  - 225 kVA: 8 unidades (1.800 kVA)
  - 150 kVA: 33 unidades (4.950 kVA - faixa predominante)
  - 112,5 kVA: 22 unidades (2.475 kVA)
  - 75 kVA: 23 unidades (1.725 kVA)
  - 45 kVA: 4 unidades (180 kVA)
  - Menores (15 a 88 kVA): 5 unidades (188 kVA: 88, 30, 30, 25 e 15 kVA)
- **Tensão Secundária:** 220/127 V trifásico a 4 fios (`ABCN`).

---

## 5. LEVANTAMENTO DE CARGAS E CONSUMIDORES

### 5.1 Perfil dos Consumidores Atendidos
O alimentador STO33 atende **6.499 unidades consumidoras de baixa tensão (UCBT)** e **7 unidades consumidoras de média tensão (UCMT)**:
- **Residenciais (RE1/RE2/RE5):** 6.263 UCs (96,3% do total de unidades).
- **Comerciais (CO1/CO2/CO3/CO5/CO6/CO8/CO9):** 190 UCs (2,9% do total de unidades).
- **Poder Público (PP2/PP3):** 28 UCs.
- **Iluminação Pública (IP):** 7 pontos cadastrais.
- **Serviço Público e Industrial:** 11 UCs.

### 5.2 Comportamento Energético Mensal
A energia ativa faturada do circuito STO33 varia significativamente ao longo do ano:
- **Pico de Verão (Janeiro/Fevereiro):** 2.304.969 kWh (Jan) e 2.331.823 kWh (Fev) &rarr; Demanda média de 3.470 kW.
- **Vale de Inverno (Junho):** 1.552.648 kWh &rarr; Demanda média de 2.156 kW.
- **Pico Térmico de Dezembro:** 2.109.167 kWh.
- **Fator de Carga Estimado:** Entre 0,55 e 0,62.
- **Perdas Técnicas no Alimentador:** 442.161 kWh na MT (A4) e 328.417 kWh na BT (B), representando perdas percentuais da ordem de **3,8% a 4,2%**, plenamente compatíveis com os resultados de simulação OpenDSS (3,91%).

---

## 6. DIAGNÓSTICO DE CARREGAMENTO E PONTOS CRÍTICOS

### 6.1 Carregamento Máximo Oficial do Alimentador
O indicador oficial da ANEEL no INDGER (`PctCarregamentoMaxAlimentador`) confirma:
- **Ano 2024:** Carregamento máximo de **94,00%**.
- **Ano 2025/2026:** Carregamento máximo registrado de **91,04%**.
- **Enquadramento Técnico:** **Classe 2 — Carregamento elevado confirmado por indicador oficial**.
- **Implicação Operacional:** O alimentador opera sem margem para contingência simples ($N-1$). Caso ocorra a perda intempestiva de um alimentador adjacente (como `STO32` ou `STO34`), o STO33 não dispõe de capacidade térmica para absorver transferências de carga sem incorrer em sobrecarga severa.

### 6.2 Transformadores com Risco Crítico de Sobrecarga
A tabela abaixo lista os postos de transformação com maior criticidade térmica calculada:

| ID Trafo | Barra MT | Barra BT | Potência (kVA) | Total UCs | UCs Res. | UCs Com. | Demanda Pico Est. (kW) | Carregamento Est. (%) | Classificação de Risco |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **1193581** | MT_STO33_166892 | BT_STO33_30362 | 225,0 | **439** | 415 | 24 | **351,2** | **164,3%** | **3. Risco calculado** |
| **1193923** | MT_STO33_144516 | BT_STO33_30651 | 225,0 | **280** | 277 | 3 | **242,5** | **113,5%** | **3. Risco calculado** |
| **48964485** | MT_STO33_132699 | BT_STO33_38814 | 150,0 | **185** | 179 | 4 | **163,8** | **115,0%** | **3. Risco calculado** |
| **1193996** | MT_STO33_127074 | BT_STO33_30717 | 225,0 | **208** | 206 | 2 | **188,4** | **88,1%** | **4. Suspeita técnica** |
| **1193605** | MT_STO33_126835 | BT_STO33_30383 | 150,0 | **142** | 136 | 6 | **128,6** | **90,2%** | **4. Suspeita técnica** |

---

## 7. GERAÇÃO DISTRIBUÍDA SOLAR FOTOVOLTAICA

### 7.1 Levantamento Municipal e do Circuito
- **Santos (Geral):** 1.342 usinas solares homologadas na ANEEL, com **16,89 MWp** instalados.
- **Alimentador STO33:** 23 usinas solares cadastradas na baixa tensão (`UGBT_tab`), somando **195,89 kW** na base oficial ANEEL MMGD (44,35 kW cadastrado na BDGD).
- **Potência Média por Instalação no STO33:** 8,52 kW (faixa de microgeração residencial e comercial).

### 7.2 Análise de Fluxo Reverso
Com a geração solar máxima simultânea estimada em **170 kW** ao meio-dia e a demanda mínima diurna do alimentador em torno de **658 kW**, a geração solar local representa cerca de **25,8% da carga mínima**. Conclui-se que:
1. **Não há risco de fluxo reverso de potência na subestação** no cenário baseline.
2. A geração solar atual desempenha papel benéfico de alívio térmico diurno e suporte de tensão.
3. Eventuais sobretensões locais ficam restritas a consumidores no final de ramais longos de baixa tensão caso a impedância de curto-circuito local seja elevada.

---

## 8. MOBILIDADE ELÉTRICA (VEÍCULOS ELÉTRICOS)

### 8.1 Frota Ativa de Santos (DETRAN-SP, Setembro/2026)
- **Frota Total de Veículos em Santos:** 218.471 unidades.
- **Frota Total Eletrificada:** 4.593 unidades.
- **Veículos Efetivamente Recarregáveis na Rede Elétrica (Plug-in):** **1.101 veículos** (589 BEV e 422 PHEV).
- **Veículos Híbridos Convencionais (Sem Recarga Externa):** 3.492 veículos.

### 8.2 Impacto e Cenários de Recarga no STO33
- **Número de VEs Estimados no STO33:** 40 a 50 veículos plug-in.
- **Cenário 1 — Recarga Residencial Desordenada (Pior Caso):**
  - Conexão de 20 veículos entre 19h00 e 21h00 em carregadores de 7,4 kW (Wallbox 32 A / 220 V).
  - Acréscimo de demanda simultânea: **148 kW**.
  - Impacto: Esse acréscimo coincide exatamente com o horário de pico residencial do alimentador, elevando a demanda na cabeceira de 2.937 kW para mais de 3.085 kW e agravando a sobrecarga nos transformadores secundários.
- **Cenário 2 — Recarga Inteligente (Smart Charging com OCPP):**
  - Deslocamento da recarga para a janela da madrugada (01h00 às 06h00), onde a rede opera com carregamento inferior a 40%.
  - Impacto: Absorção total da frota de VEs sem qualquer necessidade de substituição de transformadores ou reforço de cabos.

---

## 9. MODELAGEM E SIMULAÇÃO NO OPENDSS

### 9.1 Parâmetros e Configuração do Modelo
O modelo foi construído de forma modularizada:
- `Circuit.dss`: Barra de referência `MT_STO33_346` a 13,8 kV, tensão de operação 1,028 pu.
- `LineCodes.dss`: 19 códigos oficiais com parâmetros de impedância e limites térmicos.
- `Lines.dss`: 792 linhas (730 trechos MT + 62 chaves fechadas).
- `Transformers.dss`: 105 transformadores MT/BT modelados com enrolamentos delta-estrela aterrado (13,8 / 0,22 kV).
- `Loads.dss`: 94 centros de carga agregados com fator de potência 0,95.
- `PVSystems.dss`: 23 microgeradores solares.
- `Capacitors.dss`: 2 bancos de 300 kvar.

### 9.2 Resultados da Simulação em Snapshot (Fluxo de Potência)
A simulação executada via `opendssdirect` apresentou os seguintes resultados:
- **Status da Solução:** **CONVERGIU COM SUCESSO** em 4 iterações.
- **Potência Ativa Injetada na Cabeceira:** **2.936,64 kW (2,937 MW)**.
- **Potência Reativa Injetada:** **434,89 kvar**.
- **Fator de Potência Global na Subestação:** **0,989 (Indutivo)**.
- **Perdas Ativas Totais na Rede:** **114,83 kW (3,91% da potência ativa total)**.
- **Tensão Máxima Registrada:** **1,0248 pu** (nos barramentos adjacentes à subestação).
- **Tensão Mínima Registrada:** **0,9747 pu** (no barramento secundário mais distante).
- **Conformidade Regulatória:** Todas as barras situam-se com folga dentro da **Faixa Adequada do PRODIST Módulo 8 (0,93 a 1,05 pu)**.

---

## 10. PROPOSTA DE INSERÇÃO DE SISTEMAS BESS

Com base no diagnóstico técnico, foram selecionados e dimensionados dois pontos de inserção de Sistemas de Armazenamento de Energia por Bateria (BESS):

### 10.1 Ponto 1 — BESS Centralizado na Subestação Santos 1 (Barramento STO569794)
- **Tecnologia:** Baterias de Íons de Lítio (LFP - Fosfato de Ferro e Lítio).
- **Potência Nominal do Inversor:** **1.500 kW (1,5 MW)**.
- **Capacidade de Armazenamento:** **3.000 kWh (3,0 MWh)** — duração de descarga de 2 horas.
- **Função Operacional:**
  - *Peak Shaving:* Reduzir a demanda de pico do STO33 de 3,4 MW para menos de 2,6 MW durante o horário de ponta (18h-21h).
  - *Alívio Térmico do Trafo ETR102:* Reduzir o carregamento do transformador de 20 MVA de 94% para cerca de 82%, postergando investimentos de expansão na subestação.
  - *Suporte Volt/VAR:* Injeção dinâmica de potência reativa capacitiva durante flutuações rápidas.

### 10.2 Ponto 2 — BESS Comunitário Distribuído no Secundário do Trafo 1193581
- **Localização:** Barramento de Baixa Tensão `BT_STO33_30362`.
- **Potência Nominal:** **50 kW**.
- **Capacidade de Armazenamento:** **150 kWh** (duração de descarga de 3 horas).
- **Função Operacional:**
  - Cortar o pico de demanda local de 351 kW para 200 kW, eliminando a sobrecarga de 164% do transformador de 225 kVA.
  - Regular a tensão secundária contra afundamentos decorrentes da recarga simultânea de veículos elétricos.

---

## 11. PLANO DE EXPANSÃO PARA O MUNICÍPIO DE SANTOS

A estrutura de scripts e tratamento de dados desenvolvida no diretório `09_AUTOMACAO/` já abrangeu o cadastro integral dos **99 alimentadores** de Santos distribuídos pelas 8 subestações da CPFL Piratininga:
1. `ESU` (Santos 5 - Estuário): 20 alimentadores.
2. `MPE` (Santos 7 - Marapé): 18 alimentadores.
3. `VNO` (Santos 6 - Vila Nova): 15 alimentadores.
4. `BOQ` (Santos 4 - Boqueirão): 15 alimentadores.
5. `POR` (Santos 8 - Porto): 14 alimentadores.
6. `STO` (Santos 1): 8 alimentadores.
7. `VLM` (Santos 2 - Vila Mathias): 6 alimentadores.
8. `COD` (Santos 9 - CODESP): 3 alimentadores.

O script `03_processar_dados_tratados.py` gerou a base consolidada `alimentadores.csv` contendo todas as tensões, transformadores de força e séries de energia anual para os 99 circuitos, permitindo que a metodologia de modelagem e diagnóstico aplicada ao STO33 seja replicada de forma automatizada para qualquer um dos demais 98 alimentadores municipais.

---

## 12. MINUTA FORMAL DE REQUISIÇÃO DE DADOS À CPFL PIRATININGA

Recomenda-se o encaminhamento formal da seguinte solicitação técnica à concessionária:

```
À
CPFL PIRATININGA — DIRETORIA DE OPERAÇÕES E PLANEJAMENTO
Ref.: Solicitação de Dados Técnicos para Pesquisa Acadêmica / Projeto COBRIC

Prezados Senhores,

No âmbito do projeto de pesquisa sobre transição energética, inserção de sistemas de 
armazenamento (BESS) e mobilidade elétrica na rede de distribuição de Santos/SP, 
solicitamos formalmente a disponibilização das seguintes informações operacionais 
referentes ao Alimentador STO33 da Subestação Santos 1 (STO):

1. SÉRIES TEMPORAIS OPERACIONAIS (SCADA):
   - Série temporal das correntes de fase (Ia, Ib, Ic, In) na saída do alimentador STO33, 
     com resolução de 15 minutos, para o período dos últimos 12 meses.
   - Série temporal de potências ativa (P em kW) e reativa (Q em kvar) no disjuntor de saída.

2. DIAGRAMA OPERATIVO DE DISTRIBUIÇÃO (DOD):
   - Diagrama unifilar operacional atualizado do circuito STO33, com indicação das chaves 
     seccionadoras telecomandadas pelo COD e dos pontos normais abertos (NA) de interligação 
     com os circuitos STO32, STO34 e circuitos vizinhos.

3. CAMPANHAS DE MEDIÇÃO DE QUALIDADE DE ENERGIA (PRODIST MÓDULO 8):
   - Relatórios das últimas campanhas de medição de tensão em regime permanente e distorção 
     harmônica (THD) realizadas no Conjunto Elétrico 15884.

4. CARREGAMENTO DO TRANSFORMADOR DE FORÇA STO-ETR102:
   - Registro das demandas máximas coincidentes registradas no transformador UNI_TR_AT 56622 
     (20 MVA, 88/13.8 kV) nos meses de dezembro a março.

Certos de contar com a colaboração técnica desta concessionária para o desenvolvimento 
de soluções que contribuam para a resiliência da rede elétrica de Santos.
```
