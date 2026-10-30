# MODELO ELÉTRICO OPENDSS DO ALIMENTADOR STO33 (SANTOS/SP)

## 1. DESCRIÇÃO DO MODELO
Este modelo representa a reconstrução eletrogeográfica fidedigna do alimentador **STO33 da CPFL Piratininga**, derivado da Subestação Santos 1 (STO), a partir da Base de Dados Geográfica da Distribuidora (BDGD 2025/2026, Módulo 10 PRODIST/ANEEL).

## 2. ESTRUTURA DOS ARQUIVOS
- `Master.dss`: Arquivo principal executivo do OpenDSS.
- `Circuit.dss`: Definição da barra de cabeceira de 13,8 kV (`PAC_INI: MT_STO33_346`) e potência de curto-circuito.
- `LineCodes.dss`: Parâmetros de sequência positiva e zero ($R_1, X_1, R_0, X_0$) e ampacidades térmicas dos 19 tipos de condutores catalogados.
- `Lines.dss`: 730 trechos de média tensão com comprimentos exatos em km e fases elétricas, além das 86 chaves seccionadoras normalmente fechadas.
- `Transformers.dss`: 105 transformadores trifásicos e monofásicos de distribuição com potências nominais (15 a 300 kVA), perdas no ferro e no cobre.
- `Loads.dss`: 105 centros de carga de baixa tensão agregando 6.499 consumidores residenciais, comerciais e públicos.
- `PVSystems.dss`: 23 sistemas fotovoltaicos distribuídos cadastrados na BT do alimentador.
- `Capacitors.dss`: 2 bancos de capacitores de média tensão para suporte de reativos.
- `LoadShapes.dss`: Curvas de demanda horárias normalizadas de 24 horas obtidas da campanha oficial de medição da ANEEL (CTR) para a CPFL Piratininga.
- `simulate_sto33.py`: Script de execução automatizada em Python via `opendssdirect`.

## 3. COMO EXECUTAR NO PYTHON
```bash
python 07_OPENDSS/modelo_parcial/simulate_sto33.py
```
