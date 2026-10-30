# ARQUITETURA DE TELEMETRIA, AUTOMAÇÃO E CONTROLE DA REDE DE DISTRIBUIÇÃO
## CONCESSIONÁRIA: CPFL PIRATININGA | OBJETO DE ESTUDO: SANTOS / ALIMENTADOR STO33

---

## 1. INTRODUÇÃO E ESCOPO

A operação moderna de redes de distribuição exige a integração em tempo real de fluxos de dados operacionais, medição avançada e controle distribuído. Este documento descreve a arquitetura de supervisão, controle e telemetria identificada para a CPFL Piratininga na região de Santos/SP, discriminando o que foi formalmente comprovado em bases públicas do que constitui a prática recomendada do setor elétrico nacional.

---

## 2. SISTEMAS CENTRAIS DA CONCESSIONÁRIA

A operação da CPFL Piratininga é estruturada em torno dos seguintes sistemas corporativos comprovados:

1. **COD (Centro de Operação da Distribuição):** Localizado na sede regional em Campinas/SP, centraliza a operação e supervisão em tempo real de todas as subestações e alimentadores telecomandados de Santos via rede de dados corporativa.
2. **SCADA / EMS (Supervisory Control and Data Acquisition):**
   - Supervisiona os disjuntores de cabeceira de todos os 99 alimentadores de Santos (incluindo o disjuntor do STO33 no cubículo da SE Santos 1).
   - Mede continuamente: Corrente trifásica na saída do alimentador, tensões de barra de 88 kV e 13,8 kV, potências ativa (P) e reativa (Q), estado dos relés de proteção e temperatura dos transformadores de força (`UNI_TR_AT 56622` / `ETR102`).
   - Protocolos utilizados: **DNP3** (Distributed Network Protocol) e **IEC 60870-5-104** sobre enlace TCP/IP e rádio digital.
3. **ADMS / DMS (Advanced Distribution Management System):**
   - Sistema avançado de gestão da distribuição com módulos de análise de rede em tempo real: fluxo de potência online, estimador de estado e cálculo de curtos-circuitos.
   - **FLISR (Fault Location, Isolation, and Service Restoration):** Automação em malha fechada ou assistida que identifica faltas permanentes no STO33, abre chaves a montante/jusante do defeito e fecha chaves de socorro (tie-switches) para reenergizar blocos de clientes sadios via alimentadores vizinhos (como STO32 ou STO34).
4. **GIS Regulatório (SIG-R / Smallworld / ArcGIS):**
   - Base geográfica cadastral onde cada elemento da rede (730 trechos MT, 105 trafos, 86 chaves, 6.499 clientes) é registrado com conectividade elétrica estrita (`PAC_1`, `PAC_2`) e submetido anualmente à ANEEL em cumprimento ao Módulo 10 do PRODIST (BDGD).
5. **OMS (Outage Management System):**
   - Sistema de gestão de interrupções acoplado ao call-center e telemedição, responsável por tabular as ocorrências de falta e consolidar os indicadores de continuidade **DIC, FIC, DMIC e DICRI** reportados mensalmente ao INDGER da ANEEL.
6. **AMI (Advanced Metering Infrastructure):**
   - Medição inteligente implantada prioritariamente em consumidores do Grupo A (alta e média tensão - 7 UCs no STO33) com comunicação GPRS/4G diária para telemedição de faturamento, fator de potência e memória de massa.
   - Consumidores residenciais do Grupo B na Baixada Santista operam predominantemente com medição eletromecânica/eletrônica convencional com leitura mensal presencial, existindo projetos-piloto de telemedição inteligente em áreas selecionadas.

---

## 3. EQUIPAMENTOS DE CAMPO E NÍVEL DE CONTROLE NO ALIMENTADOR STO33

| Nível Operacional | Equipamento | Grandeza Supervisionada | Capacidade de Atuação / Controle | Evidência Comprovada |
| :--- | :--- | :--- | :--- | :---: |
| **Cabeceira MT** | Disjuntor 13.8 kV (STO33) | Ia, Ib, Ic, P, Q, V, Status 52 | Abertura/Fechamento sob carga, Religamento automático | **Comprovada (SCADA)** |
| **Subestação** | Trafo ETR102 (20 MVA) | Tensão 88/13.8 kV, Temp. Óleo | Regulação de Tensão sob Carga (Tap LTC -16 a +16) | **Comprovada (BDGD)** |
| **Linha Tronco** | Chaves Seccionadoras (86 un.) | Estado da Chave (P_N_OPE = F) | Abertura/Isolamento de trechos em manutenção ou socorro | **Comprovada (BDGD)** |
| **Compensação** | Bancos de Capacitores (2 un.) | Tensão local / Ciclo horário | Injeção de potência reativa (suporte de tensão) | **Comprovada (BDGD)** |
| **Transformação** | 105 Transformadores MT/BT | Carga agregada estimada | Taps desenergizados manuais (+/- 2,5%, +/- 5%) | **Comprovada (BDGD)** |
| **Clientes MT** | 7 Medidores Eletrônicos A4 | Curva de carga 15-minutal, FP, P | Telemedição de faturamento e penalidades de reativos | **Comprovada (BDGD/UCMT)** |
| **Geração Distr.** | 23 Microinversores Solares | Potência ativa gerada (kW) | Anti-ilhamento, Controle Volt/Watt e Volt/VAR | **Comprovada (UGBT)** |

---

## 4. INTEGRAÇÃO FUTURA DE BESS E MOBILIDADE ELÉTRICA

A implantação de Sistemas de Armazenamento de Energia em Baterias (BESS) e controle inteligente de recarga de VEs no alimentador STO33 deve seguir uma hierarquia de controle em 3 camadas:

```
[ Camada 1: Nível COD / ADMS ]
        |
        |-- DNP3 / IEC 60870-5-104 / ICCP
        v
[ Camada 2: Controlador Central de Microrrede / BESS (SE Santos 1) ]
        |
        |-- Modbus TCP / SunSpec / IEC 61850
        v
[ Camada 3: Dispositivos de Borda (Inversores BESS, Smart Chargers OCPP 2.0.1) ]
```

1. **BESS na Cabeceira (1,5 MW / 3 MWh):** Responde a sinais de potência ativa (despacho horário) e tensão do barramento MT da subestação, amortecendo a rampa de carga noturna e absorvendo flutuações solares.
2. **BESS Distribuído nos Secundários Críticos (50 kW / 150 kWh):** Atua de forma autônoma por laço de controle local (*droop control* de tensão e frequência), mantendo o carregamento do transformador de distribuição abaixo de 90%.
3. **Controle Inteligente de Veículos Elétricos:** Utilização do protocolo **OCPP 2.0.1** para modulação dinâmica da potência dos carregadores (de 7,4 kW para 3,7 kW ou corte temporário) quando o transformador local atingir 95% de carregamento.
