# RELATÓRIO FINAL TÉCNICO — COBRIC 2026
## Simulação de Armazenamento de Energia e Geração Distribuída no Alimentador STO33

**Projeto:** TELEMETRIA_SANTOS — COBRIC 2026  
**Alimentador:** STO33 (CPFL Piratininga)  
**Data:** 30 de outubro de 2026  
**Branch:** `fix/auditoria-bess-m4`

---

## 1. Objetivo

Avaliar o impacto técnico e econômico da inserção de sistemas de armazenamento de energia por baterias (BESS) e geração distribuída fotovoltaica no alimentador de distribuição STO33, operado pela CPFL Piratininga, utilizando simulação em OpenDSS com dados regulatórios reais da BDGD/ANEEL.

---

## 2. Área de Estudo e Limitações Geográficas

- **Alimentador:** STO33, subestação STO (CPFL Piratininga).
- **Área de concessão:** CPFL Piratininga, abrangendo Santos, Cubatão e municípios da Baixada Santista.
- **Município dos registros GD:** Os 23 registros de geração distribuída identificados na base ANEEL MMGD estão cadastrados em **Cubatão/SP** (código IBGE associado aos empreendimentos). Não há evidência documental de que estes registros sejam de Santos.
- **Nota importante:** O alimentador STO33 pode atender eletricamente áreas de Cubatão mesmo partindo da subestação STO (Santos). A diferenciação entre município do registro e área elétrica atendida não foi comprovada individualmente para cada unidade consumidora.

---

## 3. Fontes de Dados

| Fonte | Descrição | Rastreabilidade |
|-------|-----------|-----------------|
| BDGD CPFL Piratininga | Base de Dados Geográfica da Distribuidora, ref. 2025-12-31 V11 | Arquivo `.gdb` presente em `02_DADOS_BRUTOS/` |
| ANEEL MMGD | Relação de empreendimentos de micro e minigeração distribuída | Arquivo `.parquet` presente em `02_DADOS_BRUTOS/` |
| DETRAN-SP | Frota ativa de veículos — Santos/SP, setembro 2026 | CSV em `02_DADOS_BRUTOS/` |

---

## 4. Metodologia

### 4.1 Extração e Cruzamento de Dados

1. Registros de geração distribuída extraídos da BDGD para o alimentador STO33.
2. Cruzamento individual dos 23 identificadores `CEG_GD` com a base ANEEL MMGD.
3. Potência instalada adotada: valores da base ANEEL (coluna `POT_INST`), não os valores da BDGD (`POT_INST_BDGD`).

### 4.2 Modelagem OpenDSS

- Circuito reconstruído a partir da BDGD: 792 trechos de linha (MT), 105 transformadores MT/BT, 94 cargas.
- 23 PVSystems conectados aos barramentos BT dos transformadores correspondentes ao campo `UNI_TR_MT` de cada registro GD.
- Tensão base: 13,8 kV (MT) / 0,22 kV (BT).
- Modo snapshot e simulação diária de 24 horas.

### 4.3 Cenários Simulados

| Cenário | Descrição |
|---------|-----------|
| A — Baseline | Rede atual com 23 PVSystems (195,89 kW) |
| B — Expansão FV + VE | Baseline + 100 veículos elétricos com recarga noturna |
| C — BESS Centralizado | Cenário B + BESS 1.500 kW / 3.000 kWh na subestação |
| D — BESS Distribuído | Cenário B + 3 × 50 kW / 150 kWh em transformadores críticos |
| E — Reforço Físico | Cenário B + recondutoramento e upgrade de transformadores |

---

## 5. Caracterização Fotovoltaica — Resultados Validados

### 5.1 Cruzamento CEG_GD/ANEEL

- **23 registros** identificados na BDGD para o alimentador STO33.
- **23/23 correspondências** encontradas na base ANEEL MMGD.
- **Potência instalada ANEEL:** 195,89 kW (soma dos 23 registros).
- **Potência registrada BDGD:** 44,35 kW (divergente — valores BDGD correspondem a frações da potência nominal, provavelmente potência demandada ou fator de capacidade).
- **Município:** Cubatão/SP (todos os 23 registros).
- **Fonte:** Solar fotovoltaica (todos os 23 registros).

### 5.2 Correção Crítica dos Barramentos

**Problema identificado e corrigido:** O arquivo `PVSystems.dss` original apresentava barramentos com prefixo duplicado (`BT_STO33_BT_STO33_XXXXXX`), resultando em **23 PVSystems eletricamente desconectados** do circuito. O OpenDSS criava esses barramentos como nós isolados (ilhas elétricas), sem conexão com nenhum transformador.

**Causa raiz:** O script construtor (`07_construir_modelo_opendss.py`) concatenava o prefixo `BT_STO33_` ao valor da coluna `PAC` do CSV, que já continha o prefixo `BT_STO33_`.

**Correção aplicada:** Mapeamento via `UNI_TR_MT` → `Transformer.TR_{UNI_TR_MT}` → barramento secundário real do transformador. Os 23 PVSystems agora estão conectados aos barramentos BT corretos, confirmado pela compilação do circuito OpenDSS.

### 5.3 Validação Pós-Correção (Snapshot)

| Indicador | Valor |
|-----------|-------|
| Compilação | OK, sem erros |
| Convergência | Sim |
| PVSystems reconhecidos | 23 |
| Potência FV total (Pmpp) | 195,89 kW |
| Potência total da fonte | 2.594,06 kW |
| Perdas totais | 101,68 kW (P) / 94,81 kvar (Q) |
| V_min (pu) | 0,9758 |
| V_max (pu) | 1,0254 |
| Carregamento máximo trafo | 26,0% (TR_1193581) |
| Barramentos | 898 |
| Nós | 2.692 |

---

## 6. Cenários BESS — Auditoria dos Resultados Existentes

### 6.1 Inconsistências Identificadas

> **⚠ ALERTA CRÍTICO: Os resultados dos cenários C e D na Missão 4 são idênticos e não podem ser utilizados como validados.**

| Indicador | Cenário C | Cenário D | Observação |
|-----------|-----------|-----------|------------|
| Pico de demanda (kW) | 1.959,38 | 1.959,38 | **Idênticos** |
| Hora de pico | 18 | 18 | Idênticos |
| Demanda mínima (kW) | 963,14 | 963,14 | **Idênticos** |
| V_min (pu) | 0,9905 | 0,9905 | **Idênticos** |
| V_max (pu) | 1,0282 | 1,0282 | **Idênticos** |

Um BESS centralizado de 1.500 kW / 3.000 kWh **não pode** produzir os mesmos indicadores elétricos que 3 unidades distribuídas de 50 kW / 150 kWh (total 150 kW / 450 kWh). A potência e energia do cenário D são 10× menores que as do cenário C.

### 6.2 Despacho BESS Distribuído — Valores Congelados

Os três BESS distribuídos (`BESS_DIST_1193581`, `BESS_DIST_1193923`, `BESS_DIST_1193996`) apresentam **perfis horários absolutamente idênticos** nas 24 horas, incluindo:
- Mesma potência requisitada a cada hora
- Mesmo SOC a cada hora
- Mesma energia armazenada
- Mesmas perdas de conversão

Isto é fisicamente impossível se cada BESS está conectado a um transformador diferente com cargas diferentes.

### 6.3 Balanço Energético — Cenário C

| Indicador | Valor |
|-----------|-------|
| SOC inicial | 20,0% (600 kWh) |
| SOC final | 12,807% (384,21 kWh) |
| Energia carregada | 2.060,53 kWh |
| Energia descarregada | 2.200 kWh |
| Perdas de conversão | 175,79 kWh |
| Eficiência de carga | 95% (aplicada) |
| Eficiência de descarga | 95% (aplicada) |
| Eficiência round-trip teórica | 90,25% |
| Eficiência round-trip observada | ~87,3% (considerando SOC final ≠ SOC inicial) |

**Nota:** O SOC final (12,807%) é inferior ao SOC inicial (20,0%), indicando que o sistema descarregou mais energia do que carregou. O balanço energético não fecha em ciclo completo.

### 6.4 Classificação Revisada

| Cenário | Status Anterior | Status Auditado |
|---------|----------------|-----------------|
| A — Baseline | Validado | **Validado** (pós-correção de barramentos FV) |
| B — FV + VE | Validado | **Validado** (pós-correção de barramentos FV) |
| C — BESS Centralizado | Validado c/ ressalvas | **Não validado** — resultados idênticos ao D |
| D — BESS Distribuído | Validado | **Não validado** — despacho congelado e resultados idênticos ao C |
| E — Reforço Físico | Validado | **Validado** (convergência e CAPEX auditados) |

### 6.5 Afirmação Sobre Redução de Pico

A alegação de redução de 47,4% na demanda de pico **não é reproduzível** com os dados apresentados. Os resultados dos cenários C e D são idênticos, sugerindo que ambos utilizaram o mesmo despacho (cenário C) aplicado ao circuito. A redução de pico, se real, não foi comprovada independentemente para o cenário D.

---

## 7. Auditoria Econômica

### 7.1 Dados Disponíveis

| Cenário | CAPEX Informado |
|---------|----------------|
| C — BESS Centralizado | R$ 4.950.000,00 |
| D — BESS Distribuído | R$ 945.000,00 |
| E — Reforço Físico | R$ 624.511,55 |

### 7.2 Limitações

- Os valores de CAPEX para BESS foram estimados com base em R$ 3.300/kWh (cenário C) e R$ 2.100/kWh (cenário D).
- **Não foram apresentadas fontes verificáveis** para os custos unitários de BESS no mercado brasileiro em 2026.
- OPEX, vida útil, degradação, substituição e custo de manutenção não foram modelados.
- **Não é possível declarar um vencedor econômico** entre as alternativas sem análise de fluxo de caixa descontado e dados de mercado verificáveis.

### 7.3 Cenário E — Reforço Físico

- CAPEX auditado: R$ 624.511,55.
- Composição: recondutoramento de 5,247 km + upgrade de 3 transformadores.
- Este é o cenário com menor CAPEX, porém não inclui funcionalidade de gestão de energia ou arbitragem tarifária.

---

## 8. Procedimento de Simulação

### 8.1 Reprodução

```bash
# Compilar e validar o modelo (snapshot)
cd 07_OPENDSS/modelo_parcial/
python3 -c "
import opendssdirect as dss
dss.Text.Command('Compile [Master.dss]')
print('Converged:', dss.Solution.Converged())
print('PVSystems:', dss.ActiveClass.Count())  # após SetActiveClass('PVSystem')
"
```

### 8.2 Dependências

- Python 3.9+
- `opendssdirect.py` 0.9.4+
- Arquivos DSS em `07_OPENDSS/modelo_parcial/`

---

## 9. Limitações

1. **PVSystems estavam desconectados** até esta correção. Resultados publicados anteriormente podem ter sido gerados com PVSystems em ilhas elétricas.
2. **Cenários C e D produzem resultados idênticos**, invalidando a comparação entre BESS centralizado e distribuído.
3. **Despacho BESS distribuído é uniforme** para os 3 equipamentos, desconsiderando as condições locais de cada transformador.
4. **SOC não retorna ao estado inicial**, impossibilitando análise cíclica consistente.
5. **Dados geográficos**: os 23 registros GD estão em Cubatão/SP, não em Santos.
6. **Custos de BESS** não possuem fonte verificável para o mercado brasileiro.
7. **Não há dados de medição real** — todos os valores são simulados.

---

## 10. Conclusão

O modelo OpenDSS do alimentador STO33 foi reconstruído com sucesso a partir da BDGD regulatória, com 792 trechos de linha, 105 transformadores e 94 cargas. Os 23 sistemas fotovoltaicos (195,89 kW) foram mapeados corretamente aos barramentos BT dos transformadores correspondentes, corrigindo um erro crítico de conectividade que invalidava simulações anteriores.

Os cenários A (Baseline), B (FV+VE) e E (Reforço Físico) foram validados com convergência do solver e resultados coerentes. Os cenários C e D (BESS) **não foram validados** devido a resultados idênticos entre configurações fisicamente distintas e despacho uniforme nos BESS distribuídos.

A análise econômica permanece inconclusiva por falta de fontes verificáveis para os custos de BESS e ausência de modelagem de OPEX e vida útil.

---

## 11. Arquivos de Referência

| Arquivo | Descrição |
|---------|-----------|
| `07_OPENDSS/modelo_parcial/Master.dss` | Arquivo mestre do circuito OpenDSS |
| `07_OPENDSS/modelo_parcial/PVSystems.dss` | 23 PVSystems com barramentos corrigidos |
| `03_DADOS_TRATADOS/geracao_distribuida.csv` | 23 registros GD cruzados com ANEEL |
| `08_RELATORIOS/validacao_missao_4/BESS_BALANCO_ENERGETICO_M4.csv` | Balanço BESS horário (preservado) |
| `08_RELATORIOS/validacao_missao_4/RESULTADOS_CENARIOS_COBRIC_M4.csv` | Resultados dos 5 cenários (preservado) |
| `08_RELATORIOS/RELATORIO_FINAL_COBRIC_2026.md` | Este relatório |
| `08_RELATORIOS/DADOS_BANNER_COBRIC_2026.md` | Dados consolidados para o banner |
| `08_RELATORIOS/STATUS_FINAL_ENTREGA.md` | Status de validação da entrega |
