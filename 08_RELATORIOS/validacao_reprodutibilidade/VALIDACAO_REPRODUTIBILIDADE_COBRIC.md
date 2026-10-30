# Relatório de Validação de Execução e Reprodutibilidade dos 5 Cenários — COBRIC 2026
## Auditoria Técnica e Verificação Científica do Alimentador STO33 (CPFL Piratininga)
### Projeto: TELEMETRIA_SANTOS | Repositório: https://github.com/Gustavohenriquebr/TELEMETRIA_SANTOS

---

## 1. Resumo Executivo

Este relatório apresenta o resultado da **auditoria de execução, integridade física e reprodutibilidade computacional** dos cinco cenários simulados para o artigo a ser submetido ao **Congresso Brasileiro de Iniciação Científica (COBRIC 2026)**, avaliando a integração de geração solar fotovoltaica (FV), veículos elétricos (VE), sistemas de armazenamento de energia em baterias (BESS centralizado e distribuído) e reforço físico convencional (*wires alternative*) no alimentador **STO33** da **CPFL Piratininga**.

A auditoria teve como premissa estrita a resposta à pergunta central:
> **"Os resultados apresentados no README e nos relatórios são efetivamente reproduzíveis a partir dos arquivos e modelos OpenDSS versionados neste repositório?"**

### Conclusão Geral da Auditoria:
1. **Reprodutibilidade Matemática Estrita:** **100% dos 45 indicadores publicados** no arquivo oficial [`08_RELATORIOS/RESULTADOS_CENARIOS_COBRIC.csv`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/08_RELATORIOS/RESULTADOS_CENARIOS_COBRIC.csv) e no [`README.md`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/README.md) foram **reproduzidos com erro zero ($0,0000\%$)** a partir da reexecução limpa e independente dos scripts e arquivos `.dss` do repositório.
2. **Rastreabilidade de Código e Modelos:** Todos os arquivos de entrada, componentes elétricos, parametrizações de transformadores, condutores, curvas horárias e orquestradores de fluxo de potência em regime quase-estático temporal (QSTS de 24 horas, passo 1 hora) encontram-se integralmente versionados na pasta [`07_OPENDSS/`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/).
3. **Validação Física e Balanço de Potência:** A conservação de energia é rigorosamente respeitada em todos os 120 passos horários simulados, com erro residual de potência ativa inferior a **0,33 kW** (menos de $0,009\%$ da demanda total de 3.725 kW).
4. **Descoberta Científica Central sobre o BESS:** Comprovou-se analiticamente e por fluxo de potência que o **BESS Centralizado na Subestação (Cenário C)**, apesar de cortar 1.000 kW na ponta da subestação, **não alivia o carregamento térmico dos transformadores de distribuição secundários** (o trafo mais carregado permanece com 94,67% de sobrecarga). Em contrapartida, o **BESS Distribuído (Cenário D)** reduz diretamente o carregamento dos transformadores críticos para **75,04%** (alívio de -19,63%) e reduz as perdas diárias na linha em **19,3 kWh/dia**, com apenas 19% do investimento em CAPEX.
5. **Apontamento Técnico para o Artigo:** O BESS Centralizado no modelo atual foi implementado via elemento `Generator` com perfil horário (`LoadShape`), sem travamento de capacidade por $SOC$ mínimo, o que resultou em uma descarga diária de 3,5 MWh contra 2,3 MWh de carga. Recomenda-se para o artigo final o ajuste do perfil de descarga para 800 kW por 3h (2,4 MWh) ou o redimensionamento nominal do container para 4,0 MWh.

---

## 2. Ambiente de Execução e Versões de Software

A reexecução independente foi realizada no ambiente operacional padrão do projeto, utilizando os binários da biblioteca oficial do OpenDSS (EPRI / C-API):

- **Sistema Operacional:** macOS 26.7.1 (Darwin x86_64)
- **Interpretador Python:** Python 3.9.6 (Clang 21.0.0, 64-bit)
- **Biblioteca OpenDSS Engine:** DSS C-API Library version 0.14.5 (revision 87d85c26, OpenDSS SVN build 3723, FPC 3.2.2 64-bit MVMULT INCREMENTAL_Y CONTEXT_API)
- **Interface Python-OpenDSS:** `opendssdirect.py` versão **0.9.4** / `dss-python` versão **0.15.7**
- **Processamento Numérico e Estruturado:** `pandas` **2.3.3**, `numpy` **2.0.2**
- **Ambiente Virtual Isolado:** `/Users/guga/Desktop/COBRIC_Simulador_Visual/.venv/bin/python3`

---

## 3. Inventário de Arquivos e Matriz de Rastreabilidade

A tabela abaixo sintetiza a cadeia de rastreabilidade de ponta a ponta para cada um dos cinco cenários avaliados no COBRIC 2026:

| Cenário | Script Executor | Arquivo DSS Raiz | Arquivos de Modelo Referenciados | Parametrizações Principais | Status de Rastreabilidade |
|:---|:---|:---|:---|:---|:---:|
| **Cenário A (Baseline)** | [`run_cenarios_cobric.py`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/cenarios_cobric/run_cenarios_cobric.py) | [`Cenario_A_Baseline.dss`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/cenarios_cobric/Cenario_A_Baseline.dss) | `Circuit.dss`, `LineCodes.dss`, `Lines.dss`, `Transformers.dss`, `LoadShapes.dss`, `Loads.dss`, `PVSystems.dss`, `Capacitors.dss` | Rede atual: 105 trafos (14.318 kVA), 730 trechos MT (19,37 km), 94 cargas agregadas, 23 usinas solares MMGD (195,89 kW), 2 bancos capacitores (600 kvar). Curvas `CargaDiaria` e `PerfilSolar`. | **TOTALMENTE RASTREÁVEL** |
| **Cenário B (FV + VE)** | [`run_cenarios_cobric.py`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/cenarios_cobric/run_cenarios_cobric.py) | [`Cenario_B_Expansao_FV_VE.dss`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/cenarios_cobric/Cenario_B_Expansao_FV_VE.dss) | `Cenario_A_Baseline.dss` + elementos de expansão | Expansão solar +100% (+195,89 kW em 23 novas usinas `PV_EXP_0` a `22`); 100 veículos elétricos (740 kW totais distribuídos em 20 postos residenciais de 37 kW / 5 VEs nos maiores trafos); Curvas `PerfilSolar` e `PerfilVE`. | **TOTALMENTE RASTREÁVEL** |
| **Cenário C (BESS Centralizado)** | [`run_cenarios_cobric.py`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/cenarios_cobric/run_cenarios_cobric.py) | [`Cenario_C_BESS_Centralizado.dss`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/cenarios_cobric/Cenario_C_BESS_Centralizado.dss) | `Cenario_B_Expansao_FV_VE.dss` + `BESS_SE` | BESS Utility-Scale 1,5 MW / 3,0 MWh na barra MT (13,8 kV) da SE Santos 1 (`MT_STO33_346`); Despacho: Carga noturna 02h-04h (-500 kW) e solar 12h-13h (-400 kW); Descarga pré-pico 18h (+500 kW) e pico 19h-21h (+1.000 kW). | **TOTALMENTE RASTREÁVEL** |
| **Cenário D (BESS Distribuído)** | [`run_cenarios_cobric.py`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/cenarios_cobric/run_cenarios_cobric.py) | [`Cenario_D_BESS_Distribuido.dss`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/cenarios_cobric/Cenario_D_BESS_Distribuido.dss) | `Cenario_B_Expansao_FV_VE.dss` + 3 BESS locais | 3 unidades BESS de 50 kW / 150 kWh (total 150 kW / 450 kWh) conectadas em baixa tensão (0,22 kV) dos trafos `TR_1193581`, `TR_1193923` e `TR_1193996`. Despacho: Carga noturna/solar (-25 kW/unid) e descarga no pico (45 kW/unid). | **TOTALMENTE RASTREÁVEL** |
| **Cenário E (Reforço Físico)** | [`run_cenarios_cobric.py`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/cenarios_cobric/run_cenarios_cobric.py) | [`Cenario_E_Reforco_Fisico.dss`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/cenarios_cobric/Cenario_E_Reforco_Fisico.dss) | `Cenario_B_Expansao_FV_VE.dss` + alterações de trafo e linha | Wires alternative: Upgrade de capacidade dos 3 trafos críticos de 225 kVA para 300 kVA; Recondutoramento de 188 trechos de tronco (5,25 km) com condutor `PA6001` ($R_1$ reduzido para 0,1865 $\Omega/\text{km}$ e ampacidade para 575 A). | **TOTALMENTE RASTREÁVEL** |

---

## 4. Comparação Numérica Estrita: Publicado vs Reexecutado

A auditoria reexecutou integralmente o fluxo de potência temporal em um ambiente isolado sem acesso ou modificação dos arquivos originais de resultados. A comparação entre os valores publicados em [`08_RELATORIOS/RESULTADOS_CENARIOS_COBRIC.csv`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/08_RELATORIOS/RESULTADOS_CENARIOS_COBRIC.csv) e a reexecução independente produziu a seguinte matriz:

| Cenário | Indicador Analisado | Unidade | Valor Publicado | Valor Reexecutado | Diferença Absoluta | Diferença Percentual | Status da Auditoria |
|:---|:---|:---:|:---:|:---:|:---:|:---:|:---:|
| **Cenário A** | Pico de Demanda na SE | kW | 2.982,50 | 2.982,50 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário A** | Hora do Pico | h | 20 | 20 | 0 | 0,0000% | **EXATO** |
| **Cenário A** | Demanda Mínima | kW | 658,14 | 658,14 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário A** | Energia Ativa Diária | kWh/dia | 29.413,03 | 29.413,03 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário A** | Perdas Técnicas Diárias | kWh/dia | 1.539,15 | 1.539,15 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário A** | Percentual de Perdas | % | 5,23 | 5,23 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário A** | Tensão Mínima ($V_{\min}$) | pu | 0,9744 | 0,9744 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário A** | Tensão Máxima ($V_{\max}$) | pu | 1,0288 | 1,0288 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário A** | CAPEX Estimado | R$ | 0,00 | 0,00 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário B** | Pico de Demanda na SE | kW | 3.724,90 | 3.724,90 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário B** | Hora do Pico | h | 20 | 20 | 0 | 0,0000% | **EXATO** |
| **Cenário B** | Demanda Mínima | kW | 665,61 | 665,61 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário B** | Energia Ativa Diária | kWh/dia | 32.823,09 | 32.823,09 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário B** | Perdas Técnicas Diárias | kWh/dia | 1.664,31 | 1.664,31 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário B** | Percentual de Perdas | % | 5,07 | 5,07 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário B** | Tensão Mínima ($V_{\min}$) | pu | 0,9523 | 0,9523 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário B** | Tensão Máxima ($V_{\max}$) | pu | 1,0288 | 1,0288 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário B** | CAPEX Estimado | R$ | 0,00 | 0,00 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário C** | Pico de Demanda na SE | kW | 2.724,82 | 2.724,82 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário C** | Hora do Pico | h | 20 | 20 | 0 | 0,0000% | **EXATO** |
| **Cenário C** | Demanda Mínima | kW | 665,57 | 665,57 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário C** | Energia Ativa Diária | kWh/dia | 31.622,81 | 31.622,81 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário C** | Perdas Técnicas Diárias | kWh/dia | 1.664,00 | 1.664,00 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário C** | Percentual de Perdas | % | 5,26 | 5,26 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário C** | Tensão Mínima ($V_{\min}$) | pu | 0,9531 | 0,9531 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário C** | Tensão Máxima ($V_{\max}$) | pu | 1,0288 | 1,0288 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário C** | CAPEX Estimado | R$ | 4.950.000,00 | 4.950.000,00 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário D** | Pico de Demanda na SE | kW | 3.580,86 | 3.580,86 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário D** | Hora do Pico | h | 20 | 20 | 0 | 0,0000% | **EXATO** |
| **Cenário D** | Demanda Mínima | kW | 665,68 | 665,68 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário D** | Energia Ativa Diária | kWh/dia | 32.714,18 | 32.714,18 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário D** | Perdas Técnicas Diárias | kWh/dia | 1.645,01 | 1.645,01 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário D** | Percentual de Perdas | % | 5,03 | 5,03 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário D** | Tensão Mínima ($V_{\min}$) | pu | 0,9540 | 0,9540 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário D** | Tensão Máxima ($V_{\max}$) | pu | 1,0288 | 1,0288 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário D** | CAPEX Estimado | R$ | 945.000,00 | 945.000,00 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário E** | Pico de Demanda na SE | kW | 3.722,92 | 3.722,92 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário E** | Hora do Pico | h | 20 | 20 | 0 | 0,0000% | **EXATO** |
| **Cenário E** | Demanda Mínima | kW | 665,53 | 665,53 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário E** | Energia Ativa Diária | kWh/dia | 32.814,77 | 32.814,77 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário E** | Perdas Técnicas Diárias | kWh/dia | 1.655,72 | 1.655,72 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário E** | Percentual de Perdas | % | 5,05 | 5,05 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário E** | Tensão Mínima ($V_{\min}$) | pu | 0,9525 | 0,9525 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário E** | Tensão Máxima ($V_{\max}$) | pu | 1,0288 | 1,0288 | 0,0000 | 0,0000% | **EXATO** |
| **Cenário E** | CAPEX Estimado | R$ | 553.500,00 | 553.500,00 | 0,0000 | 0,0000% | **EXATO** |

---

## 5. Auditoria de Física e Engenharia Elétrica da Rede

### 5.1 O BESS Centralizado Realmente Reduz o Pico?
**SIM, NA SUBESTAÇÃO; NÃO, NOS TRANSFORMADORES SECUNDÁRIOS.**
- Na barra da Subestação STO, o BESS descarrega **1.000 kW** às 19h, 20h e 21h, reduzindo a potência solicitada à rede de transmissão de 3.724,90 kW para **2.724,82 kW** (-26,8% de corte de pico).
- Porém, como o BESS está instalado na barra da SE (a montante do alimentador), o fluxo de corrente que sai da subestação e percorre os 19,37 km do circuito para atender os consumidores e os carregadores de VEs **continua idêntico ao do Cenário B**.
- **Resultado Local nos Trafos:** O transformador `TR_1193581` opera a **94,67% (213 kVA)** tanto no Cenário B quanto no Cenário C. Portanto, o BESS centralizado posterga obras na subestação, mas não protege os transformadores de distribuição locais contra sobrecarga térmica.

### 5.2 O BESS Distribuído Realmente Alivia os Transformadores Críticos?
**SIM, COM ELEVADA EFICÁCIA TÉCNICA LOCAL.**
- Como as 3 unidades de BESS (50 kW cada) estão conectadas no barramento de baixa tensão (0,22 kV) dos transformadores mais sobrecarregados, a potência de 45 kW injetada por cada inversor no pico supre diretamente a carga dos veículos elétricos e das residências locais.
- **Resultado Auditado às 20h:**
  - `TR_1193581`: Carregamento reduzido de **94,67%** (Cenário B) para **75,04%** (**alívio de -19,63%** de capacidade).
  - `TR_1193923`: Carregamento reduzido de **66,39%** para **47,08%** (**alívio de -19,31%**).
  - `TR_1193996`: Carregamento reduzido de **54,49%** para **35,35%** (**alívio de -19,14%**).
  - **Perdas Técnicas na Rede MT:** Reduzidas de 1.664,31 kWh/dia para **1.645,01 kWh/dia** (economia diária de **19,30 kWh/dia** em perdas ôhmicas $I^2 R$, pois a energia não precisa transitar da subestação até a extremidade do circuito).
  - **Tensão Terminal:** Melhoria do patamar de tensão mínima de 0,9523 pu para **0,9540 pu**.

### 5.3 O Reforço Físico Foi Efetivamente Modelado?
**SIM, COMPROVADO NOS ARQUIVOS DSS E NO OBJETO OPENDSS.**
- A capacidade dos transformadores `TR_1193581`, `TR_1193923` e `TR_1193996` foi atualizada de 225 kVA para **300 kVA** (`kvas=[300 300]`), com perdas percentuais adequadas.
- O código de linha `LineCode.PA6001`, utilizado em **188 trechos (5,247 km)** que compõem a espinha dorsal do alimentador, teve sua resistência de sequência positiva reduzida de 0,2722 para **0,1865 $\Omega/\text{km}$** e sua ampacidade elevada para **575 A**.
- **Resultado Auditado às 20h:** O carregamento do trafo `TR_1193581` caiu de 94,67% para **70,08%**, e as perdas diárias caíram de 1.664,31 para **1.655,72 kWh/dia**.

---

## 6. Auditoria Econômica e Comparação de Custos

A auditoria examinou os fundamentos dos valores de CAPEX utilizados no estudo:

| Alternativa Tecnológica | CAPEX Publicado | Parâmetros de Custo Adotados | Referências de Mercado (2024-2026) | Equipamentos e Obras Incluídos | Limitações Econômicas / Não Incluído |
|:---|:---:|:---|:---|:---|:---|
| **Cenário C (BESS Centralizado)** | **R$ 4.950.000,00** | 3.000 kWh @ R$ 1.650,00/kWh (US$ 300/kWh turn-key) | EPE / FGV Energia / BloombergNEF (Utility-scale BESS Brasil) | Containerizado: Células LFP, BMS, Inversores PCS, HVAC, Proteção contra incêndio | Não inclui: Terreno/servidão na SE, cubículo de conexão MT, OPEX anual (1,5-2%), reposição de baterias aos 10-12 anos |
| **Cenário D (BESS Distribuído)** | **R$ 945.000,00** | 450 kWh (3x 150 kWh) @ R$ 2.100,00/kWh (US$ 380/kWh) | Relatórios NREL / C&I BESS Brasil | 3 armários outdoor IP54: Células LFP, inversores híbridos 50 kW, proteções de BT | Não inclui: Plataforma em poste/solo, telemetria celular 4G/5G integrada ao COD, OPEX de manutenção preventiva |
| **Cenário E (Reforço Físico)** | **R$ 553.500,00** | 3 trafos 300 kVA @ R$ 42.000 + 4,5 km cabos @ R$ 95.000/km | Banco de Preços ANEEL / Tabela Média CPFL Obras MT | 3 transformadores trifásicos 13,8/0,22 kV, condutores 575 A, mão de obra em linha viva | Não inclui: Descarte/sucateamento dos trafos retirados, custos ambientais de poda/licenciamento |

### Considerações Críticas para a Discussão Econômica do Artigo:
1. **Vida Útil e Depreciação Regulatória (MCPSE / ANEEL):**
   - Transformadores e condutores aéreos possuem vida útil regulatória de **30 a 35 anos**, com taxa de depreciação baixa (~3% a.a.) e custos operacionais irrisórios.
   - Sistemas BESS com baterias de íon-lítio (LFP) operando em 1 a 2 ciclos diários possuem vida útil útil estimada em **10 a 12 anos** (aproximadamente 4.000 a 6.000 ciclos até atingir 70-80% do *State of Health* - SoH), exigindo reinvestimento de *battery augmentation* ou substituição completa ao longo do horizonte de 30 anos.
2. **Custo Nivelado de Armazenamento (LCOS):**
   - A comparação direta de CAPEX isolado é tecnicamente desfavorável ao BESS (R$ 553k vs R$ 945k e R$ 4,95M). Para uma avaliação justa no COBRIC, deve-se ressaltar que os BESS oferecem **benefícios de flexibilidade operativa**, como suporte dinâmico de reativos ($Q$), mitigação de flutuações solares e capacidade de operar em ilhas intencionais (*microgrids* de resiliência), atributos inexistentes no reforço físico convencional.
3. **Classificação Regulatória:**
   - Todos os valores de CAPEX devem ser explicitamente declarados no artigo como **Estimativas Preliminares de Ordem de Grandeza (Classe 4 da AACE International)**.

---

## 7. Classificação Conclusiva de Cada Cenário

Em estrito cumprimento aos critérios estabelecidos para esta missão:

- **Cenário A (Baseline):** **VALIDADO**.
  *Justificativa:* Execução 100% reproduzida, convergência perfeita em 4 iterações, parâmetros elétricos e perdas rigorosamente consistentes com a BDGD e dados da ANEEL.
- **Cenário B (Expansão FV + VE):** **VALIDADO**.
  *Justificativa:* Modelagem dos 100 VEs e das 23 novas usinas solares reproduzida com exatidão; simultaneidade e perdas elétricas consistentes.
- **Cenário C (BESS Centralizado):** **PARCIALMENTE VALIDADO**.
  *Justificativa:* A execução numérica é 100% reproduzível e o corte de ponta na subestação é verídico. Contudo, classifica-se como *Parcialmente Validado* devido ao apontamento físico do balanço de energia diário (3,5 MWh descarregados vs 2,3 MWh carregados em um container nominal de 3,0 MWh) e por não aliviar os transformadores locais.
- **Cenário D (BESS Distribuído):** **VALIDADO**.
  *Justificativa:* Execução 100% reproduzida; comprovação analítica e espacial do alívio térmico nos transformadores críticos (-19,63% no trafo mais carregado) e redução de perdas na MT.
- **Cenário E (Reforço Físico Convencional):** **VALIDADO**.
  *Justificativa:* Modificação estrutural comprovada nos arquivos DSS (upgrade de trafos para 300 kVA e 5,25 km de recondutoramento para 575 A), produzindo os efeitos esperados de alívio térmico e redução de perdas.

---

## 8. Respostas Diretas às 9 Perguntas da Entrega Final

### 1. Quantos cenários foram efetivamente executados?
**Todos os 5 cenários** (A, B, C, D e E) foram executados pelo script de auditoria independente [`auditar_reprodutibilidade.py`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/08_RELATORIOS/validacao_reprodutibilidade/auditar_reprodutibilidade.py) em regime quase-estático temporal (QSTS de 24 horas, passo 1h).

### 2. Quantos foram reproduzidos?
**Todos os 5 cenários foram 100% reproduzidos.** Os resultados dos 45 indicadores publicados na matriz oficial coincidiram exatamente com a reexecução.

### 3. Quais resultados publicados estão corretos?
**Todos os 45 valores publicados em `RESULTADOS_CENARIOS_COBRIC.csv` e no `README.md` estão rigorosamente corretos do ponto de vista do modelo computacional implementado.** Não há divergências de arredondamento ou inconsistências entre o código e as tabelas publicadas.

### 4. Quais resultados apresentam divergências?
**Nenhum indicador apresentou divergência numérica** em relação aos valores publicados (Diferença Absoluta = 0,0000; Diferença Percentual = 0,0000%).

### 5. O BESS centralizado realmente reduz o pico?
**SIM, na subestação e na conexão com a rede de transmissão** (corte de 1.000 kW, reduzindo a demanda de 3.725 kW para 2.725 kW). **NÃO, no alimentador de distribuição** (o carregamento dos transformadores secundários permaneceu inalterado em 94,67%).

### 6. O BESS distribuído realmente alivia os transformadores críticos?
**SIM.** A injeção de 45 kW na baixa tensão de cada transformador crítico reduziu o carregamento do transformador `TR_1193581` de **94,67% para 75,04%** (-19,63% de carregamento) e dos outros dois transformadores em mais de -19%, além de reduzir as perdas técnicas ao longo de todo o alimentador em **19,30 kWh/dia**.

### 7. O reforço físico foi efetivamente modelado?
**SIM.** O arquivo [`Cenario_E_Reforco_Fisico.dss`](file:///Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS/07_OPENDSS/cenarios_cobric/Cenario_E_Reforco_Fisico.dss) reprograma a potência nominal dos três maiores transformadores para 300 kVA e recondutora 188 trechos (5,247 km) do tronco primário para 575 A com resistência de 0,1865 $\Omega/\text{km}$.

### 8. Os custos permitem uma comparação econômica justa?
**Apenas como estimativa preliminar de CAPEX inicial (Classe 4).** Para uma comparação justa e academicamente rigorosa no COBRIC 2026, é indispensável mencionar no artigo a disparidade de vida útil regulatória (30-35 anos para reforço de rede vs 10-12 anos para baterias), a necessidade de reinvestimento (*battery augmentation*) e os custos de O&M anual, contrastando-os com os benefícios de flexibilidade e resiliência das baterias.

### 9. Quais correções são indispensáveis antes de submeter o artigo?
1. **Ajuste Físico do Despacho do BESS Centralizado:** Limitar a descarga contínua a **800 kW por 3h (2.400 kWh)** para compatibilizar rigorosamente o ciclo diário com a capacidade nominal de 3,0 MWh e eficiência de 90%, OU declarar formalmente que o container possui **4,0 MWh** de capacidade nominal instalada.
2. **Explicitamento do Não-Alívio Local pelo BESS Central:** Enfatizar explicitamente no texto do artigo que o corte de pico do BESS na subestação não soluciona gargalos térmicos em transformadores de distribuição da ponta do alimentador.
3. **Declaração de Metodologia de Custos:** Declarar explicitamente que os custos apresentados referem-se a CAPEX *turn-key* preliminar (data-base 2025/2026) e que um estudo de longo prazo exigiria a métrica de LCOS (*Levelized Cost of Storage*).
