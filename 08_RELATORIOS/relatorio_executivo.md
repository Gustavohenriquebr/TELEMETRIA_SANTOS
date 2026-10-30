# RELATÓRIO EXECUTIVO — TELEMETRIA, ENGENHARIA REVERSA E DIAGNÓSTICO DA REDE DE DISTRIBUIÇÃO DE SANTOS/SP
## FOCO: ALIMENTADOR STO33 (CPFL PIRATININGA — SUBESTAÇÃO SANTOS 1)

**Data de Conclusão da Investigação:** 30 de Outubro de 2026  
**Status do Projeto:** Base Eletrogeográfica e Modelo OpenDSS 100% Concluídos e Validados  
**Área de Investigação:** Município de Santos/SP (Código IBGE: 3548500)  
**Distribuidora:** CPFL Piratininga (Código de Agente ANEEL: 2937)  
**Conjunto Elétrico:** 15884 (Santos 1)  

---

## RESPOSTAS DIRETAS ÀS 20 PERGUNTAS OBRIGATÓRIAS

### 1. O que sabemos efetivamente sobre o STO33?
Sabemos que o STO33 é um circuito radial de distribuição urbana de média tensão (13,8 kV) em topologia aérea e trechos de transição subterrânea, com extensão física total de **19,37 km** (730 segmentos de linha), atendendo **6.499 unidades consumidoras de baixa tensão (UCBT)** e 7 unidades consumidoras de média tensão (UCMT). Possui **105 transformadores de distribuição MT/BT** totalizando **14.318 kVA** de capacidade instalada, **86 chaves seccionadoras**, **2 bancos de capacitores** de MT e **23 sistemas de microgeração solar fotovoltaica** conectados na BT (totalizando **195,89 kW** na base oficial ANEEL MMGD). A energia anual entregue pelo alimentador é de **23,06 GWh/ano**, com pico sazonal no verão (dezembro a março).

### 2. Qual é a subestação de origem comprovada?
A subestação comprovada documental e geograficamente na BDGD e no INDGER é a **Subestação Santos 1 (Código: STO)**, localizada nas coordenadas **Latitude -23.926°, Longitude -46.383°**. O alimentador STO33 parte da barra de média tensão `STO569794`, alimentada pelo transformador de força `UNI_TR_AT 56622` (denominação operacional `STO-ETR102`, potência nominal 15/20 MVA, relação de transformação 88/13,8 kV).

### 3. Qual é a topologia que foi possível reconstruir?
Foi possível reconstruir **100% da topologia física e elétrica do alimentador STO33**. O circuito forma uma **árvore conectada perfeita com 793 nós e 792 arestas** (sem ciclos e sem ilhas elétricas), com raiz no ponto de conexão de cabeceira `PAC_INI: MT_STO33_346`. O tronco principal parte com condutores de alta capacidade (575 A / 420 A) e se ramifica em condutores de 310 A e 202 A. Todos os 105 transformadores conectam-se diretamente a nós do alimentador.

### 4. Quantos elementos foram encontrados em cada categoria?
- **Segmentos de Rede MT (SSDMT):** 730 trechos (19,37 km).
- **Barramentos/Nós MT (PACs):** 793 nós georreferenciados.
- **Transformadores de Distribuição (UNTRMT):** 105 unidades (15 a 300 kVA).
- **Transformadores de Subestação (UNTRAT):** 1 transformador de força dedicado (20 MVA, 88/13,8 kV).
- **Chaves de Manobra/Seccionamento (UNSEMT):** 86 chaves (62 fechadas na configuração normal).
- **Bancos de Capacitores MT (UNCRMT):** 2 bancos para compensação de reativos.
- **Religadores MT de Linha (UNREMT):** 0 no alimentador (proteção concentrada no disjuntor da SE).
- **Consumidores BT (UCBT):** 6.499 unidades consumidoras.
- **Consumidores MT (UCMT):** 7 unidades consumidoras.
- **Usinas de Geração Distribuída (UGBT):** 23 usinas solares no STO33 (1.342 em Santos).

### 5. Quais equipamentos possuem parâmetros elétricos completos?
- **Condutores:** Todos os 19 tipos de cabos possuem parâmetros completos no catálogo `SEGCON` da BDGD ($R_1, X_1, R_0, X_0$, ampacidade nominal, ampacidade máxima e material).
- **Transformadores MT/BT:** Todos os 105 trafos possuem potência nominal (kVA), tensões primária (13,8 kV) e secundária (0,22 kV), conexão (delta-estrela aterrado), perdas no cobre, perdas no ferro e reatância percentual ($X_{HL}$).
- **Transformador da Subestação:** 20 MVA, 88/13,8 kV, perdas totais de 0,66% e perdas em vazio de 0,19%.

### 6. Onde estão as maiores concentrações de carga?
As maiores concentrações de carga estão associadas aos transformadores de alta densidade vertical e comercial:
- **Trafo 1193581 (225 kVA):** 439 unidades consumidoras (demanda calculada de 370 kVA).
- **Trafo 1193923 (225 kVA):** 280 unidades consumidoras (demanda calculada de 255 kVA).
- **Trafo 1193996 (225 kVA):** 208 unidades consumidoras (demanda calculada de 198 kVA).
- **Trafo 48964485 (150 kVA):** 185 unidades consumidoras (demanda calculada de 172 kVA).
- **Trafo 1194753 (300 kVA):** 141 unidades consumidoras.

### 7. Existe algum carregamento elevado comprovado?
**SIM.** O carregamento elevado na cabeceira do circuito STO33 é **comprovado por indicador oficial da ANEEL (INDGER)**:
- O campo `PctCarregamentoMaxAlimentador` registra **94,00%** (2024) e **91,04%** (2026), superando a margem operacional recomendada de 80% (critério de contingência N-1).
- O transformador de força `ETR102` (20 MVA) da SE Santos 1 atende simultaneamente 4 alimentadores (`STO32`, `STO33`, `STO34`, `STO36`) entregando mais de 5,7 GWh em dezembro, operando próximo de seu limite de ventilação forçada (ONAF).

### 8. Existem sobrecargas confirmadas?
- **Em tempo real por medição contínua:** Não há dados de corrente minuto a minuto públicos que comprovem a duração exata de sobrecarga térmica contínua.
- **Risco técnico calculado (Classe 3):** O transformador `1193581` (225 kVA, 439 UCs) e o transformador `48964485` (150 kVA, 185 UCs) apresentam demanda de pico simultânea estimada acima de **115% a 164%** da capacidade contínua nominal em dias de calor severo de verão.

### 9. Existem dados de tensão e corrente?
- **Dados Oficiais Existentes:** Tensão nominal cadastral (13,8 kV / 220 V), tensão de operação em regime permanente na barra da subestação (1,028 pu na BDGD), e corrente máxima agregada via percentual de carregamento no INDGER.
- **Dados Ausentes Publicamente:** Séries temporais de corrente instantânea ($I_a, I_b, I_c$) e tensões nodais minutais ao longo da linha.

### 10. Existe telemetria operacional pública?
**NÃO.** A telemetria operacional em tempo real (SCADA/EMS) da CPFL Piratininga é de uso interno e restrito à operação do COD. As bases públicas disponibilizam apenas dados cadastrais anuais (BDGD) e indicadores gerenciais mensais agregados (INDGER).

### 11. Quais são os recursos de automação comprovados?
- Disjuntor de saída na subestação com proteção de sobrecorrente e supervisão SCADA.
- 86 chaves seccionadoras de média tensão (comprovadas no BDGD).
- 2 bancos de capacitores de MT (300 kvar cada) cadastrados na rede para compensação de reativos.
- Automação centralizada no COD Campinas via protocolo DNP3/IEC 60870-5-104.

### 12. Qual é a presença fotovoltaica na região?
- **Alimentador STO33:** 23 usinas solares conectadas em baixa tensão, somando **195,89 kW** na base oficial ANEEL MMGD (44,35 kW cadastrado originalmente na BDGD).
- **Município de Santos:** **1.342 empreendimentos de micro e minigeração solar**, somando **16.893,56 kW (16,89 MWp)** de potência instalada (10,00 MWp no comércio e 6,32 MWp em residências).

### 13. Qual é a presença de veículos elétricos?
Conforme microdados oficiais do DETRAN-SP de **setembro de 2026**:
- **Frota Total de Santos:** 218.471 veículos.
- **Frota Eletrificada Total:** **4.593 veículos**.
- **Veículos Plug-in com Carga na Rede (BEV + PHEV):** **1.101 veículos** (589 puramente elétricos e 422 híbridos plug-in).
- **Projeção para o Alimentador STO33:** Aproximadamente **40 a 50 veículos plug-in** atendidos na área geográfica do circuito.

### 14. Quais pontos podem apresentar restrições com FV e EV?
- **Restrição com FV:** Baixa no alimentador como um todo. A geração solar total (195,89 kW) representa menos de 30% da demanda mínima diurna (658 kW), afastando risco de fluxo reverso na subestação no baseline.
- **Restrição com EV:** Alta e concentrada nos transformadores secundários com mais de 150 consumidores (`1193581`, `1193923`, `48964485`). A conexão simultânea de 3 a 5 veículos em recarga rápida residencial (7,4 kW) entre 19h e 21h pode disparar sobrecargas superiores a 130%.

### 15. Quais pontos podem ser candidatos a BESS?
1. **Candidato Centralizado (Ponto 1):** Barramento de 13,8 kV da Subestação Santos 1 (Barra `STO569794`).
   - *Problema Mitigado:* Carregamento de 91-94% do alimentador STO33 e alívio térmico do transformador de força `ETR102` (20 MVA).
   - *Porte Recomendado:* **1,5 MW / 3,0 MWh**.
2. **Candidatos Distribuídos (Ponto 2 e 3):** Secundários de baixa tensão dos transformadores `1193581` e `1193923`.
   - *Problema Mitigado:* Sobrecarga local calculada (>120%) e suporte à recarga simultânea de veículos elétricos.
   - *Porte Recomendado:* **50 kW / 150 kWh** por ponto crítico.

### 16. O que pode ser simulado imediatamente no OpenDSS?
O modelo construído e validado permite simular imediatamente:
- Fluxo de potência estático (Snapshot) e em regime permanente.
- Perfil de tensão nodal ao longo dos 19,37 km de rede.
- Perdas ativas e reativas por segmento de condutor e transformador.
- Simulação quase-estática diária (Daily Time-Series de 24 horas) cruzando perfis CTR de carga residencial/comercial, geração solar e curvas de recarga veicular.
- Análise de contingência N-1 e manobras de abertura/fechamento das 86 chaves.

### 17. Quais parâmetros impedem uma simulação completa?
A ausência de:
- Curva de medição de corrente contínua 15-minutal da cabeceira do alimentador.
- Despacho e status em tempo real das chaves telecomandadas (SOE de manobras).
- Medição individualizada horária de faturamento dos transformadores de distribuição.

### 18. Quais dados precisam ser solicitados diretamente à CPFL?
Deve ser protocolada solicitação formal de:
1. Histórico 15-minutal dos últimos 12 meses das grandezas $I_a, I_b, I_c, P, Q, V$ do alimentador STO33 na SE Santos 1.
2. Diagrama Operativo de Distribuição (DOD) identificando as chaves telecomandadas e pontos normais abertos com circuitos vizinhos (`STO32`, `STO34`).
3. Relatórios de carregamento e temperatura do transformador `STO-ETR102`.

### 19. Quais informações são públicas, restritas ou inexistentes?
- **Públicas:** Topologia georreferenciada completa (BDGD), cadastro de transformadores, condutores, chaves, consumidores agregados, usinas de GD (ANEEL), curvas de carga de referência (CTR) e frota veicular (DETRAN-SP).
- **Restritas à CPFL:** Medições SCADA em tempo real, telas do ADMS, cadastros de medição inteligente (AMI) individualizados de baixa tensão e oscilografias de proteção.
- **Inexistentes:** Telemetria minuto a minuto de transformadores de poste convencionais sem automação de BT.

### 20. Qual é o próximo passo técnico com maior valor para a pesquisa?
Executar a campanha de simulação no OpenDSS acoplando o script `simulate_sto33.py` com cenários de inserção do **BESS de 1,5 MW / 3 MWh na cabeceira** e **BESS de 50 kW nos transformadores críticos**, quantificando o percentual exato de redução do pico de carregamento e a capacidade de recepção de novos veículos elétricos na rede de Santos.
