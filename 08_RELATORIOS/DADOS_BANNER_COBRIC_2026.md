# DADOS PARA O BANNER — COBRIC 2026
## Simulação de BESS e Geração Distribuída no Alimentador STO33

---

## Título Proposto

**Avaliação Técnica de Armazenamento de Energia por Baterias (BESS) e Geração Distribuída Fotovoltaica em Alimentador de Distribuição: Simulação OpenDSS do STO33 — CPFL Piratininga**

---

## Problema

O crescimento da geração distribuída fotovoltaica e da recarga de veículos elétricos impõe desafios à operação de alimentadores de distribuição, incluindo sobrecarga de transformadores, violações de tensão e aumento de perdas elétricas. Soluções como armazenamento de energia por baterias (BESS) e reforço físico da rede precisam ser avaliadas tecnicamente.

---

## Objetivo

Modelar e simular o alimentador STO33 (CPFL Piratininga) em OpenDSS, com dados regulatórios reais da BDGD/ANEEL, para avaliar cinco cenários operacionais: rede atual (baseline), expansão FV+VE, BESS centralizado, BESS distribuído e reforço físico convencional.

---

## Metodologia

- Reconstrução do circuito a partir da BDGD regulatória (CPFL Piratininga, ref. 2025).
- Cruzamento individual de registros GD com a base ANEEL MMGD via identificadores CEG.
- Simulação em OpenDSS (OpenDSSDirect.py) com fluxo de potência diário de 24h.
- Cinco cenários comparados: A (Baseline), B (FV+VE), C (BESS Central), D (BESS Distribuído), E (Reforço Físico).

---

## Dados Confirmados — USAR NO BANNER ✓

| Indicador | Valor | Fonte |
|-----------|-------|-------|
| Alimentador | STO33 — CPFL Piratininga | BDGD |
| Trechos de linha MT | 792 | OpenDSS compilado |
| Transformadores MT/BT | 105 | OpenDSS compilado |
| Cargas modeladas | 94 | OpenDSS compilado |
| Barramentos | 898 | OpenDSS compilado |
| PVSystems conectados | 23 | OpenDSS compilado |
| Potência FV instalada (ANEEL) | 195,89 kW | Base ANEEL MMGD |
| Registros GD cruzados | 23/23 (100%) | CEG_GD → ANEEL |
| Município dos registros GD | Cubatão/SP | Base ANEEL MMGD |
| Convergência do modelo | 100% (snapshot e 24h) | OpenDSS |
| V_min (Baseline, snapshot) | 0,9758 pu | OpenDSS |
| V_max (Baseline, snapshot) | 1,0254 pu | OpenDSS |
| Perdas totais (snapshot) | 101,68 kW | OpenDSS |
| Carregamento máx. trafo (snapshot) | 26,0% (TR_1193581) | OpenDSS |
| CAPEX Reforço Físico (Cen. E) | R$ 624.511,55 | Auditado |

---

## Cenários — Resumo para Banner

| Cenário | Descrição | Status |
|---------|-----------|--------|
| A — Baseline | Rede atual com 23 PVSystems | ✓ Validado |
| B — FV + VE | Baseline + 100 VEs (recarga noturna) | ✓ Validado |
| C — BESS Centralizado | 1.500 kW / 3.000 kWh na SE | ⚠ Não validado (resultados idênticos ao D) |
| D — BESS Distribuído | 3 × 50 kW / 150 kWh em trafos | ⚠ Não validado (despacho congelado) |
| E — Reforço Físico | Recondutoramento + upgrade trafos | ✓ Validado |

---

## Indicadores Comprovados para o Banner

### Topologia e Modelo
- "Alimentador STO33 reconstruído com **792 trechos, 105 transformadores e 898 barramentos** a partir da BDGD regulatória"
- "**23 sistemas fotovoltaicos** (195,89 kW) cruzados com base ANEEL e conectados ao modelo OpenDSS"

### Validação Elétrica
- "Convergência 100% do fluxo de potência em modo snapshot e diário"
- "Tensão dentro dos limites ANEEL: **0,9758 pu ≤ V ≤ 1,0254 pu**"

### Reforço Físico
- "Cenário de reforço convencional auditado: **R$ 624.511,55** (recondutoramento + upgrade de transformadores)"

---

## DADOS QUE NÃO DEVEM SER USADOS NO BANNER ✗

| Indicador | Razão |
|-----------|-------|
| Redução de pico de 47,4% | Não reproduzida independentemente |
| Pico de 1.959,38 kW (Cen. C e D) | Resultados idênticos — bug confirmado |
| Eficiência round-trip de 99,52% | Valor reportado é eficiência de carga, não round-trip |
| CAPEX BESS R$ 4.950.000 e R$ 945.000 | Sem fonte verificável para custo unitário |
| "BESS distribuído reduz carregamento de 94,67% para 54,58%" | Não reproduzido com modelo corrigido |
| Qualquer comparação direta C vs D | Resultados são idênticos (bug) |

---

## Conclusão Curta (para Banner)

> O alimentador STO33 foi modelado em OpenDSS a partir da BDGD regulatória com 792 trechos de linha, 105 transformadores e 23 sistemas fotovoltaicos (195,89 kW) cruzados com a base ANEEL. Os cenários Baseline, Expansão FV+VE e Reforço Físico foram validados com convergência e tensões dentro dos limites ANEEL. Os cenários BESS requerem reexecução devido a inconsistências no despacho entre configurações centralizada e distribuída.

---

## Referências e Fontes

1. BDGD CPFL Piratininga — ref. 2025-12-31, versão V11.
2. ANEEL — Relação de Empreendimentos de Micro e Minigeração Distribuída.
3. OpenDSS / OpenDSSDirect.py v0.9.4.
4. DETRAN-SP — Frota ativa de Santos/SP, setembro 2026.
