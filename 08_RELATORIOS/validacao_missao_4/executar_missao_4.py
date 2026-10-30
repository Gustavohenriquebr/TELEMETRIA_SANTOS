#!/usr/bin/env python3
"""
MISSÃO 4 — INTEGRAÇÃO REAL DO BESS AO OPENDSS E VALIDAÇÃO CIENTÍFICA
Projeto COBRIC 2026 — Alimentador STO33 — CPFL Piratininga — Santos/SP

Execução completa dos 5 cenários utilizando elementos Storage nativos do OpenDSS,
com registro de variáveis de estado internas (mode=3), potências AC (mode=1),
carregamento de transformadores críticos, testes físicos e correção econômica.
"""

import os
import sys
import math
import opendssdirect as dss
import pandas as pd
import numpy as np

BASE_DIR = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
DSS_DIR = os.path.join(BASE_DIR, '07_OPENDSS', 'cenarios_cobric')
MODEL_DIR = os.path.join(BASE_DIR, '07_OPENDSS', 'modelo_parcial')
OUTPUT_DIR = os.path.join(BASE_DIR, '08_RELATORIOS', 'validacao_missao_4')
REPORTS_DIR = os.path.join(BASE_DIR, '08_RELATORIOS')

CRITICAL_TRAFOS = ['TR_1193581', 'TR_1193923', 'TR_1193996', 'TR_48964485']

# Auditoria de CAPEX corrigido
# 188 trechos de PA6001 = 5,24749 km
# Upgrade de 3 trafos de 225 para 300 kVA
CAPEX_E_CORRIGIDO = round((3 * 42000.0) + (5.24749 * 95000.0), 2)  # R$ 624.511,55

CAPEX_MAP = {
    'Cenário A (Baseline)': 0.0,
    'Cenário B (FV + VE)': 0.0,
    'Cenário C (BESS Centralizado)': 4950000.0,
    'Cenário D (BESS Distribuído)': 945000.0,
    'Cenário E (Reforço Físico)': CAPEX_E_CORRIGIDO
}

def run_all_scenarios():
    scenarios = [
        ('Cenário A (Baseline)', os.path.join(DSS_DIR, 'Cenario_A_Baseline.dss')),
        ('Cenário B (FV + VE)', os.path.join(DSS_DIR, 'Cenario_B_Expansao_FV_VE.dss')),
        ('Cenário C (BESS Centralizado)', os.path.join(DSS_DIR, 'Cenario_C_BESS_Centralizado.dss')),
        ('Cenário D (BESS Distribuído)', os.path.join(DSS_DIR, 'Cenario_D_BESS_Distribuido.dss')),
        ('Cenário E (Reforço Físico)', os.path.join(DSS_DIR, 'Cenario_E_Reforco_Fisico.dss'))
    ]

    all_hourly = []
    all_summaries = []
    all_trafos = []
    all_bess_states = []

    for sc_name, dss_file in scenarios:
        print(f"\n{'='*75}")
        print(f"SIMULANDO: {sc_name}")
        print(f"Arquivo DSS: {os.path.basename(dss_file)}")
        print(f"{'='*75}")

        dss.Basic.ClearAll()
        dss.Text.Command(f'Redirect "{dss_file}"')

        # Monitores já definidos nos arquivos .dss correspondentes

        # Snapshot inicial para convergência das condições nodais
        dss.Text.Command('Set Mode=Snapshot')
        dss.Solution.Solve()
        if not dss.Solution.Converged():
            print(f"AVISO: Snapshot inicial não convergiu para {sc_name}")

        # Obter capacidades de trafos
        trafo_kvas = {}
        n_tr = dss.Transformers.Count()
        if dss.Transformers.First() > 0:
            for _ in range(n_tr):
                trafo_kvas[dss.Transformers.Name().lower()] = dss.Transformers.kVA()
                dss.Transformers.Next()

        # Inicia modo diário passo a passo (24 horas)
        dss.Text.Command('Set Mode=Daily number=1 stepsize=1h')

        hourly_records = []
        for h in range(24):
            dss.Solution.Solve()
            conv = dss.Solution.Converged()

            # Potência líquida da subestação
            tot_p, tot_q = dss.Circuit.TotalPower()
            p_sub = -tot_p
            q_sub = -tot_q

            # Perdas ativas e reativas
            losses = dss.Circuit.Losses()
            p_loss = losses[0] / 1000.0
            q_loss = losses[1] / 1000.0

            # Tensões nodais em pu
            v_pu = [v for v in dss.Circuit.AllBusMagPu() if v > 0.05]
            vmin = min(v_pu) if v_pu else 1.0
            vmax = max(v_pu) if v_pu else 1.0

            hourly_records.append({
                'CENARIO': sc_name,
                'HORA': h + 1,
                'P_SUB_KW': round(p_sub, 2),
                'Q_SUB_KVAR': round(q_sub, 2),
                'PERDAS_KW': round(p_loss, 2),
                'PERDAS_KVAR': round(q_loss, 2),
                'VMIN_PU': round(vmin, 4),
                'VMAX_PU': round(vmax, 4),
                'CONVERGIU': conv
            })

            # Carregamento dos transformadores críticos
            for tr in CRITICAL_TRAFOS:
                dss.Circuit.SetActiveElement(f'Transformer.{tr}')
                pws = dss.CktElement.Powers()
                tp = sum(pws[0:6:2])
                tq = sum(pws[1:6:2])
                ts = math.sqrt(tp**2 + tq**2)
                cap = trafo_kvas.get(tr.lower(), 225.0)
                all_trafos.append({
                    'CENARIO': sc_name,
                    'HORA': h + 1,
                    'TRAFO': tr,
                    'CAPACIDADE_KVA': cap,
                    'DEMANDA_P_KW': round(tp, 2),
                    'DEMANDA_Q_KVAR': round(tq, 2),
                    'DEMANDA_S_KVA': round(ts, 2),
                    'CARREGAMENTO_PCT': round((ts / cap) * 100.0, 2)
                })

        df_h = pd.DataFrame(hourly_records)
        all_hourly.append(df_h)

        # Resumo do cenário
        p_peak = df_h['P_SUB_KW'].max()
        p_peak_h = int(df_h.loc[df_h['P_SUB_KW'].idxmax(), 'HORA'])
        p_min = df_h['P_SUB_KW'].min()
        e_dia = df_h['P_SUB_KW'].sum()
        perdas_dia = df_h['PERDAS_KW'].sum()
        pct_perdas = (perdas_dia / e_dia) * 100.0 if e_dia > 0 else 0.0
        vmin_all = df_h['VMIN_PU'].min()
        vmax_all = df_h['VMAX_PU'].max()
        all_conv = df_h['CONVERGIU'].all()

        summary_entry = {
            'CENARIO': sc_name,
            'PICO_DEMANDA_KW': round(p_peak, 2),
            'HORA_PICO': p_peak_h,
            'DEMANDA_MIN_KW': round(p_min, 2),
            'ENERGIA_DIA_KWH': round(e_dia, 2),
            'PERDAS_DIA_KWH': round(perdas_dia, 2),
            'PERDAS_PCT': round(pct_perdas, 2),
            'VMIN_PU': round(vmin_all, 4),
            'VMAX_PU': round(vmax_all, 4),
            'CAPEX_ESTIMADO_RS': CAPEX_MAP.get(sc_name, 0.0),
            'CONVERGENCIA_24H': all_conv
        }
        all_summaries.append(summary_entry)

        print(f"  Pico na SE: {p_peak:.2f} kW @ {p_peak_h}h")
        print(f"  Perdas diárias: {perdas_dia:.2f} kWh ({pct_perdas:.2f}%)")
        print(f"  Faixa de tensão: [{vmin_all:.4f}, {vmax_all:.4f}] pu")
        print(f"  Convergência 24h: {'100% OK' if all_conv else 'FALHOU'}")

        # Extrair variáveis internas dos BESS via Monitors
        if 'Cenário C' in sc_name:
            dss.Monitors.Name('Mon_BESS_SE_State')
            kwh_c = dss.Monitors.Channel(1)
            st_c = dss.Monitors.Channel(2)
            dss.Monitors.Name('Mon_BESS_SE_P')
            p_c = [-(a+b+c) for a,b,c in zip(dss.Monitors.Channel(1), dss.Monitors.Channel(3), dss.Monitors.Channel(5))]
            for h in range(24):
                st_code = int(st_c[h])
                st_name = 'CHARGING' if st_code == -1 else ('DISCHARGING' if st_code == 1 else 'IDLING')
                e_kwh = kwh_c[h]
                soc = (e_kwh / 3000.0) * 100.0
                all_bess_states.append({
                    'CENARIO': sc_name,
                    'BESS_NAME': 'BESS_SE',
                    'HORA': h + 1,
                    'POTENCIA_AC_KW': round(p_c[h], 2),
                    'ENERGIA_ARMAZENADA_KWH': round(e_kwh, 2),
                    'SOC_PCT': round(soc, 2),
                    'STATE_CODE': st_code,
                    'STATE_NAME': st_name,
                    'CAPACIDADE_NOMINAL_KWH': 3000.0,
                    'POTENCIA_NOMINAL_KW': 1500.0
                })

        elif 'Cenário D' in sc_name:
            d_units = [
                ('BESS_DIST_1193581', 'D1'),
                ('BESS_DIST_1193923', 'D2'),
                ('BESS_DIST_1193996', 'D3')
            ]
            for bname, tag in d_units:
                dss.Monitors.Name(f'Mon_BESS_{tag}_State')
                kwh_d = dss.Monitors.Channel(1)
                st_d = dss.Monitors.Channel(2)
                dss.Monitors.Name(f'Mon_BESS_{tag}_P')
                p_d = [-(a+b+c) for a,b,c in zip(dss.Monitors.Channel(1), dss.Monitors.Channel(3), dss.Monitors.Channel(5))]
                for h in range(24):
                    st_code = int(st_d[h])
                    st_name = 'CHARGING' if st_code == -1 else ('DISCHARGING' if st_code == 1 else 'IDLING')
                    e_kwh = kwh_d[h]
                    soc = (e_kwh / 150.0) * 100.0
                    all_bess_states.append({
                        'CENARIO': sc_name,
                        'BESS_NAME': bname,
                        'HORA': h + 1,
                        'POTENCIA_AC_KW': round(p_d[h], 2),
                        'ENERGIA_ARMAZENADA_KWH': round(e_kwh, 2),
                        'SOC_PCT': round(soc, 2),
                        'STATE_CODE': st_code,
                        'STATE_NAME': st_name,
                        'CAPACIDADE_NOMINAL_KWH': 150.0,
                        'POTENCIA_NOMINAL_KW': 50.0
                    })

    df_summary = pd.DataFrame(all_summaries)
    df_all_hourly = pd.concat(all_hourly, ignore_index=True)
    df_trafos = pd.DataFrame(all_trafos)
    df_bess = pd.DataFrame(all_bess_states)

    return df_summary, df_all_hourly, df_trafos, df_bess


def build_bess_power_and_energy_reports(df_bess):
    """
    Gera os relatórios de potência e energia a partir da telemetria dos
    monitores Storage. Convenção adotada: carga negativa; descarga positiva.
    Os despachos esperados espelham os LoadShapes dos arquivos DSS.
    """
    eta_c = 0.95
    eta_d = 0.95
    schedules = {
        'BESS_SE': [0, -450, -450, -450, -450, 0, 0, 0, 0, 0, 0, -250, -160.53, 0, 0, 0, 0, 0, 700, 950, 550, 0, 0, 0],
        'BESS_DIST_1193581': [0, -25, -25, -25, 0, 0, 0, 0, 0, 0, 0, -20, -15.53, 0, 0, 0, 0, 0, 35, 45, 30, 0, 0, 0],
        'BESS_DIST_1193923': [0, -25, -25, -25, 0, 0, 0, 0, 0, 0, 0, -20, -15.53, 0, 0, 0, 0, 0, 35, 45, 30, 0, 0, 0],
        'BESS_DIST_1193996': [0, -25, -25, -25, 0, 0, 0, 0, 0, 0, 0, -20, -15.53, 0, 0, 0, 0, 0, 35, 45, 30, 0, 0, 0]
    }
    power_rows, energy_rows = [], []
    for bname, grp in df_bess.groupby('BESS_NAME'):
        grp = grp.sort_values('HORA')
        if len(grp) != 24:
            raise RuntimeError(f'{bname}: esperado 24 amostras do monitor; recebidas {len(grp)}.')
        schedule = schedules[bname]
        for _, r in grp.iterrows():
            h = int(r['HORA'])
            p_meas = float(r['POTENCIA_AC_KW'])
            p_req = float(schedule[h - 1])
            e_start = float(r['ENERGIA_ARMAZENADA_KWH'])
            if p_meas < -0.01:
                e_charge_ac = abs(p_meas)
                e_discharge_ac = 0.0
                e_end = e_start + e_charge_ac * eta_c
                losses = e_charge_ac * (1.0 - eta_c)
            elif p_meas > 0.01:
                e_charge_ac = 0.0
                e_discharge_ac = p_meas
                e_end = e_start - e_discharge_ac / eta_d
                losses = e_discharge_ac * (1.0 / eta_d - 1.0)
            else:
                e_charge_ac = e_discharge_ac = losses = 0.0
                e_end = e_start
            soc_start = 100.0 * e_start / float(r['CAPACIDADE_NOMINAL_KWH'])
            soc_end = 100.0 * e_end / float(r['CAPACIDADE_NOMINAL_KWH'])
            power_rows.append({
                'CENARIO': r['CENARIO'], 'HORA': h, 'BESS_NAME': bname,
                'P_REQUESTED_KW': p_req, 'P_AC_REFERENCIA_LOADSHAPE_KW': p_req,
                'P_AC_OPENDSS_KW': round(p_meas, 4),
                'DELTA_KW': round(p_meas - p_req, 4),
                'STATE': r['STATE_NAME'], 'SOC_INICIO_PCT': round(soc_start, 3),
                'SOC_FIM_PCT': round(soc_end, 3),
                'E_ARMAZENADA_KWH': round(e_start, 4),
                'ENERGIA_CARGA_KWH': round(e_charge_ac, 4),
                'ENERGIA_DESCARGA_KWH': round(e_discharge_ac, 4),
                'PERDAS_CONV_KWH': round(losses, 4)
            })
            energy_rows.append({
                'CENARIO': r['CENARIO'], 'HORA': h, 'BESS_NAME': bname,
                'ENERGIA_INICIAL_KWH': round(e_start, 4),
                'ENERGIA_CARGA_AC_KWH': round(e_charge_ac, 4),
                'ENERGIA_DESCARGA_AC_KWH': round(e_discharge_ac, 4),
                'PERDAS_CONVERSAO_KWH': round(losses, 4),
                'ENERGIA_FINAL_ESTIMADA_KWH': round(e_end, 4),
                'SOC_INICIAL_PCT': round(soc_start, 3),
                'SOC_FINAL_ESTIMADO_PCT': round(soc_end, 3),
                'OBSERVACAO': 'Energia final estimada pelo despacho medido e eficiencia nominal de 95%; monitor mode=3 registra estado no instante de amostragem.'
            })
    return pd.DataFrame(power_rows), pd.DataFrame(energy_rows)

def execute_physical_tests(df_bess, df_summary, df_trafos, df_power):
    """Testes com evidência observável; itens não medidos ficam inconclusivos."""
    tests = []
    for bname, grp in df_bess.groupby('BESS_NAME'):
        sc = grp['CENARIO'].iloc[0]
        pg = df_power[df_power['BESS_NAME'] == bname].sort_values('HORA')
        kw_nom = float(grp['POTENCIA_NOMINAL_KW'].iloc[0])
        soc_values = pd.concat([pg['SOC_INICIO_PCT'], pg['SOC_FIM_PCT']])
        soc_min, soc_max = float(soc_values.min()), float(soc_values.max())
        p_max = float(pg['P_AC_OPENDSS_KW'].abs().max())
        t1 = soc_min >= 9.99 and soc_max <= 90.01
        t2 = bool(((pg.loc[pg['STATE'] == 'DISCHARGING', 'SOC_FIM_PCT'] >= 9.99)).all())
        t3 = bool(((pg.loc[pg['STATE'] == 'CHARGING', 'SOC_FIM_PCT'] <= 90.01)).all())
        t4 = p_max <= kw_nom + 0.01
        state_sign_ok = (
            ((pg['STATE'] == 'CHARGING') & (pg['P_AC_OPENDSS_KW'] < -0.01)) |
            ((pg['STATE'] == 'DISCHARGING') & (pg['P_AC_OPENDSS_KW'] > 0.01)) |
            ((pg['STATE'] == 'IDLING') & (pg['P_AC_OPENDSS_KW'].abs() <= 0.01))
        )
        t10 = bool(state_sign_ok.all())
        delta_soc = abs(float(pg['SOC_FIM_PCT'].iloc[-1]) - float(pg['SOC_INICIO_PCT'].iloc[0]))
        for name, ok, evidence in [
            ('T1 - Limites operacionais de SOC', t1, f'SOC observado/estimado entre {soc_min:.2f}% e {soc_max:.2f}%.'),
            ('T2 - Bloqueio de descarga no SOC minimo', t2, 'SOC final estimado de cada intervalo de descarga >= 10%.'),
            ('T3 - Bloqueio de carga no SOC maximo', t3, 'SOC final estimado de cada intervalo de carga <= 90%.'),
            ('T4 - Potencia nominal do inversor', t4, f'Pico medido={p_max:.3f} kW; nominal={kw_nom:.2f} kW.'),
            ('T10 - Consistencia entre estado e sinal', t10, 'Estado CHARGING exige P negativa; DISCHARGING P positiva; IDLING proximo de zero.')
        ]:
            tests.append({'TESTE': name, 'SISTEMA': bname, 'CENARIO': sc,
                          'STATUS': 'APROVADO' if ok else 'REPROVADO', 'EVIDENCIA': evidence})

        # Um único dia que começa e termina com SOC diferente não permite medir
        # eficiência round-trip de ciclo fechado nem provar repetibilidade diária.
        tests.append({
            'TESTE': 'T5 - Eficiencia round-trip e balanco de ciclo',
            'SISTEMA': bname, 'CENARIO': sc,
            'STATUS': 'INCONCLUSIVO',
            'EVIDENCIA': f'Delta SOC no dia={delta_soc:.2f} pontos percentuais. Eficiência nominal configurada=90,25%; eficiência round-trip medida exige ciclo fechado.'
        })
        tests.append({
            'TESTE': 'T6 - Repetibilidade diaria do SOC',
            'SISTEMA': bname, 'CENARIO': sc,
            'STATUS': 'INCONCLUSIVO' if delta_soc > 0.5 else 'APROVADO',
            'EVIDENCIA': f'Delta entre SOC inicial e final estimado={delta_soc:.2f} pp; um único dia não demonstra repetibilidade em dias sucessivos.'
        })
        tests.append({
            'TESTE': 'T8 - Balanco eletrico nodal independente (LKC)',
            'SISTEMA': bname, 'CENARIO': sc,
            'STATUS': 'NAO AVALIADO',
            'EVIDENCIA': 'O script não calcula resíduos nodais independentes; convergência do solver, isoladamente, não comprova este teste.'
        })

    conv_all = bool(df_summary['CONVERGENCIA_24H'].all())
    tests.append({
        'TESTE': 'T7 - Convergencia do fluxo de potencia',
        'SISTEMA': 'REDE_STO33', 'CENARIO': 'TODOS OS 5 CENARIOS',
        'STATUS': 'APROVADO' if conv_all else 'REPROVADO',
        'EVIDENCIA': f"{int(df_summary['CONVERGENCIA_24H'].sum())} de {len(df_summary)} cenários convergiram em seus 24 passos horários."
    })

    if not df_trafos.empty:
        max_row = df_trafos.loc[df_trafos['CARREGAMENTO_PCT'].idxmax()]
        thermal_ok = bool((df_trafos['CARREGAMENTO_PCT'] <= 100.0).all())
        thermal_evidence = (f"Maior carregamento de transformador={max_row['CARREGAMENTO_PCT']:.2f}% "
                            f"({max_row['TRAFO']}, {max_row['CENARIO']}, hora {int(max_row['HORA'])}). "
                            "Ampacidade de linhas não foi avaliada por este teste.")
    else:
        thermal_ok, thermal_evidence = False, 'Dados de carregamento de transformadores ausentes.'
    tests.append({
        'TESTE': 'T9 - Carregamento termico de transformadores',
        'SISTEMA': 'TRANSFORMADORES', 'CENARIO': 'TODOS OS 5 CENARIOS',
        'STATUS': 'APROVADO' if thermal_ok else 'REPROVADO',
        'EVIDENCIA': thermal_evidence
    })
    return pd.DataFrame(tests)

def audit_capex():
    """
    Audita com precisão o CAPEX do Cenário E e compara com BESS.
    """
    total_km = 5.24749
    n_trafos = 3
    custo_km = 95000.0
    custo_trafo = 42000.0

    capex_rede = total_km * custo_km
    capex_trafos = n_trafos * custo_trafo
    capex_total = capex_rede + capex_trafos
    capex_m3 = 553500.0
    delta_capex = capex_total - capex_m3

    rec = [{
        'ITEM': 'Recondutoramento de Tronco PA6001 (575 A)',
        'QUANTIDADE_MODELO': f'{total_km:.5f} km (188 trechos)',
        'QUANTIDADE_MISSAO_3': '4.500 km',
        'CUSTO_UNITARIO_RS': custo_km,
        'FONTE_CUSTO': 'Banco de Preços Referenciais ANEEL / CPFL Piratininga',
        'CAPEX_CORRIGIDO_RS': round(capex_rede, 2)
    }, {
        'ITEM': 'Substituição de Trafos (225 kVA -> 300 kVA)',
        'QUANTIDADE_MODELO': f'{n_trafos} unidades (TR_1193581, TR_1193923, TR_1193996)',
        'QUANTIDADE_MISSAO_3': '3 unidades',
        'CUSTO_UNITARIO_RS': custo_trafo,
        'FONTE_CUSTO': 'Tabela Referencial de Obras de Distribuição (Classe 4 AACE)',
        'CAPEX_CORRIGIDO_RS': round(capex_trafos, 2)
    }, {
        'ITEM': 'TOTAL REFORÇO FÍSICO CONVENCIONAL',
        'QUANTIDADE_MODELO': '188 trechos + 3 trafos',
        'QUANTIDADE_MISSAO_3': '4,5 km + 3 trafos',
        'CUSTO_UNITARIO_RS': '-',
        'FONTE_CUSTO': 'Soma das rubricas auditadas',
        'CAPEX_CORRIGIDO_RS': round(capex_total, 2)
    }]

    df_capex = pd.DataFrame(rec)
    return df_capex, capex_total, delta_capex

def classify_scenarios(df_summary, df_tests):
    """Classificação conservadora baseada na convergência efetivamente registrada."""
    rows = []
    for _, r in df_summary.iterrows():
        converged = bool(r['CONVERGENCIA_24H'])
        rows.append({
            'CENARIO': r['CENARIO'],
            'CLASSIFICACAO': 'SIMULACAO EXECUTADA - CONVERGENCIA OK' if converged else 'SIMULACAO COM FALHA DE CONVERGENCIA',
            'JUSTIFICATIVA': (
                'Fluxo diário convergiu nos 24 passos registrados. Isto comprova execução numérica, '
                'não validação externa dos dados, calibração ou adequação operacional.'
                if converged else
                'Um ou mais passos não convergiram; resultados do cenário não devem ser tratados como validados.'
            )
        })
    return pd.DataFrame(rows)

def main():
    print("█" * 80)
    print("EXECUTANDO ORQUESTRADOR COMPLETO — MISSÃO 4 COBRIC 2026")
    print("█" * 80)

    os.makedirs(OUTPUT_DIR, exist_ok=True)

    # 1. Executar os 5 cenários
    df_summary, df_hourly, df_trafos, df_bess = run_all_scenarios()

    # 2. Gerar comparacao de potencia e balanco energetico a partir dos monitores
    print("\nGerando relatorios de potencia e energia dos BESS...")
    df_bess_power, df_bess_energy = build_bess_power_and_energy_reports(df_bess)

    # 3. Executar testes com dados efetivamente medidos
    print("Executando testes de validacao fisica...")
    df_tests = execute_physical_tests(df_bess, df_summary, df_trafos, df_bess_power)

    # 4. Auditoria do CAPEX
    print("Executando auditoria do CAPEX Cenário E...")
    df_capex, capex_corrigido, delta_capex = audit_capex()

    # 5. Classificação dos cenários
    print("Classificando os cenários...")
    df_classif = classify_scenarios(df_summary, df_tests)

    # 6. Salvar arquivos em validacao_missao_4
    print("\nSalvando arquivos de auditoria em 08_RELATORIOS/validacao_missao_4/...")
    df_summary.to_csv(os.path.join(OUTPUT_DIR, 'RESULTADOS_CENARIOS_COBRIC_M4.csv'), index=False)
    df_hourly.to_csv(os.path.join(OUTPUT_DIR, 'CURVAS_HORARIAS_COBRIC_M4.csv'), index=False)
    df_trafos.to_csv(os.path.join(OUTPUT_DIR, 'CARREGAMENTO_TRAFOS_M4.csv'), index=False)
    df_bess.to_csv(os.path.join(OUTPUT_DIR, 'BESS_ESTADO_INTERNO_M4.csv'), index=False)
    df_bess_power.to_csv(os.path.join(OUTPUT_DIR, 'BESS_POTENCIA_OPENDSS_M4.csv'), index=False)
    df_bess_energy.to_csv(os.path.join(OUTPUT_DIR, 'BESS_BALANCO_ENERGETICO_M4.csv'), index=False)
    df_tests.to_csv(os.path.join(OUTPUT_DIR, 'BESS_TESTES_VALIDACAO_M4.csv'), index=False)
    df_capex.to_csv(os.path.join(OUTPUT_DIR, 'AUDITORIA_CAPEX_CENARIO_E.csv'), index=False)
    df_classif.to_csv(os.path.join(OUTPUT_DIR, 'CLASSIFICACAO_VALIDACAO_M4.csv'), index=False)

    # 7. Atualizar arquivos consolidados do repositório
    print("Atualizando arquivos consolidados em 08_RELATORIOS/...")
    df_summary.to_csv(os.path.join(REPORTS_DIR, 'RESULTADOS_CENARIOS_COBRIC.csv'), index=False)
    df_hourly.to_csv(os.path.join(REPORTS_DIR, 'CURVAS_HORARIAS_CENARIOS_COBRIC.csv'), index=False)

    print("\n" + "=" * 80)
    print("MATRIZ FINAL CONSOLIDADA — MISSÃO 4 (STORAGE NATIVO OPENDSS)")
    print("=" * 80)
    print(df_summary.to_string(index=False))
    print("=" * 80)
    print(f"\nCAPEX Cenário E Corrigido: R$ {capex_corrigido:,.2f} (Delta vs Missão 3: +R$ {delta_capex:,.2f})")
    print("\nClassificação Final dos Cenários:")
    print(df_classif.to_string(index=False))
    print("=" * 80)

if __name__ == '__main__':
    main()
