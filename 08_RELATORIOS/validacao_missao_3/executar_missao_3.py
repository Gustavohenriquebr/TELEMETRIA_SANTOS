#!/usr/bin/env python3
"""
EXECUCAO, VALIDACAO FISICA E BALANCO ENERGETICO DOS 5 CENARIOS — MISSAO 3 COBRIC 2026
Executa os 5 cenarios no OpenDSS com o modelo fisico rigoroso de BESS,
calculando fluxo de potencia, carregamento de transformadores criticos,
balanco horario de energia e comparacao antes vs depois.
"""

import os
import sys
import opendssdirect as dss
import pandas as pd
import numpy as np

BASE_DIR = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
DSS_DIR = os.path.join(BASE_DIR, '07_OPENDSS', 'cenarios_cobric')
OUTPUT_DIR = os.path.join(BASE_DIR, '08_RELATORIOS', 'validacao_missao_3')
PREV_DIR = os.path.join(BASE_DIR, '08_RELATORIOS', 'validacao_reprodutibilidade')

CRITICAL_TRAFOS = ['TR_1193581', 'TR_1193923', 'TR_1193996', 'TR_48964485']

def run_simulation():
    scenarios = [
        ('Cenário A (Baseline)', os.path.join(DSS_DIR, 'Cenario_A_Baseline.dss')),
        ('Cenário B (FV + VE)', os.path.join(DSS_DIR, 'Cenario_B_Expansao_FV_VE.dss')),
        ('Cenário C (BESS Centralizado)', os.path.join(DSS_DIR, 'Cenario_C_BESS_Centralizado.dss')),
        ('Cenário D (BESS Distribuído)', os.path.join(DSS_DIR, 'Cenario_D_BESS_Distribuido.dss')),
        ('Cenário E (Reforço Físico)', os.path.join(DSS_DIR, 'Cenario_E_Reforco_Fisico.dss'))
    ]
    
    capex_map = {
        'Cenário A (Baseline)': 0.0,
        'Cenário B (FV + VE)': 0.0,
        'Cenário C (BESS Centralizado)': 4950000.0,
        'Cenário D (BESS Distribuído)': 945000.0,
        'Cenário E (Reforço Físico)': 553500.0
    }
    
    all_hourly = []
    summary = []
    trafo_records = []
    
    for sc_name, dss_file in scenarios:
        print(f"Simulando {sc_name}...")
        dss.Basic.ClearAll()
        dss.Text.Command(f'Redirect "{dss_file}"')
        
        # Snapshot inicial
        dss.Text.Command('Set Mode=Snapshot')
        dss.Solution.Solve()
        
        # Obter capacidades de trafos
        num_trafos = dss.Transformers.Count()
        trafo_kvas = {}
        if dss.Transformers.First() > 0:
            for _ in range(num_trafos):
                trafo_kvas[dss.Transformers.Name().lower()] = dss.Transformers.kVA()
                dss.Transformers.Next()
                
        # Modo diário
        dss.Text.Command('Set Mode=Daily number=1 stepsize=1h')
        
        records = []
        for h in range(24):
            dss.Solution.Solve()
            conv = dss.Solution.Converged()
            
            # Potencia na SE
            tot_p, tot_q = dss.Circuit.TotalPower()
            p_sub = -tot_p
            q_sub = -tot_q
            
            # Perdas tecnicas
            losses = dss.Circuit.Losses()
            p_loss = losses[0] / 1000.0
            q_loss = losses[1] / 1000.0
            
            # Tensoes
            v_pu = [v for v in dss.Circuit.AllBusMagPu() if v > 0.05]
            vmin = min(v_pu) if v_pu else 1.0
            vmax = max(v_pu) if v_pu else 1.0
            
            records.append({
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
            
            # Trafo loadings
            for tr in CRITICAL_TRAFOS:
                dss.Circuit.SetActiveElement(f"Transformer.{tr}")
                pws = dss.CktElement.Powers()
                tp = sum(pws[0:6:2])
                tq = sum(pws[1:6:2])
                ts = np.sqrt(tp**2 + tq**2)
                cap = trafo_kvas.get(tr.lower(), 225.0)
                trafo_records.append({
                    'CENARIO': sc_name,
                    'HORA': h + 1,
                    'TRAFO': tr,
                    'CAPACIDADE_KVA': cap,
                    'DEMANDA_P_KW': round(tp, 2),
                    'DEMANDA_S_KVA': round(ts, 2),
                    'CARREGAMENTO_PCT': round((ts / cap) * 100.0, 2)
                })
                
        df_h = pd.DataFrame(records)
        all_hourly.append(df_h)
        
        p_peak = df_h['P_SUB_KW'].max()
        p_peak_h = df_h.loc[df_h['P_SUB_KW'].idxmax(), 'HORA']
        p_min = df_h['P_SUB_KW'].min()
        e_dia = df_h['P_SUB_KW'].sum()
        perdas_dia = df_h['PERDAS_KW'].sum()
        pct_perdas = (perdas_dia / e_dia) * 100.0 if e_dia > 0 else 0.0
        vmin_all = df_h['VMIN_PU'].min()
        vmax_all = df_h['VMAX_PU'].max()
        all_conv = df_h['CONVERGIU'].all()
        
        summary.append({
            'CENARIO': sc_name,
            'PICO_DEMANDA_KW': round(p_peak, 2),
            'HORA_PICO': int(p_peak_h),
            'DEMANDA_MIN_KW': round(p_min, 2),
            'ENERGIA_DIA_KWH': round(e_dia, 2),
            'PERDAS_DIA_KWH': round(perdas_dia, 2),
            'PERDAS_PCT': round(pct_perdas, 2),
            'VMIN_PU': round(vmin_all, 4),
            'VMAX_PU': round(vmax_all, 4),
            'CAPEX_ESTIMADO_RS': capex_map.get(sc_name, 0.0),
            'CONVERGENCIA_100PCT': all_conv
        })
        
    df_summary = pd.DataFrame(summary)
    df_all_hourly = pd.concat(all_hourly, ignore_index=True)
    df_all_trafos = pd.DataFrame(trafo_records)
    
    return df_summary, df_all_hourly, df_all_trafos

def compute_bess_energy_balance():
    # 1. BESS Centralizado (1.500 kW / 3.000 kWh)
    # Profile: 02h-05h: -450 kW; 12h: -250 kW; 13h: -160.5 kW; 19h: 700 kW; 20h: 950 kW; 21h: 550 kW
    bess_c_profile = [
        0.0, -450.0, -450.0, -450.0, -450.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
        -250.0, -160.5, 0.0, 0.0, 0.0, 0.0, 0.0, 700.0, 950.0, 550.0, 0.0, 0.0, 0.0
    ]
    
    # 2. BESS Distribuído (3 unidades de 50 kW / 150 kWh cada)
    # Profile por unidade: 02h-04h: -25 kW; 12h: -20 kW; 13h: -15.5 kW; 19h: 35 kW; 20h: 45 kW; 21h: 30 kW
    bess_d_profile = [
        0.0, -25.0, -25.0, -25.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
        -20.0, -15.5, 0.0, 0.0, 0.0, 0.0, 0.0, 35.0, 45.0, 30.0, 0.0, 0.0, 0.0
    ]
    
    def simulate_bess_lifecycle(name, profile, e_nom, p_max, soc_ini=0.20, soc_min=0.10, soc_max=0.90, eta_c=0.95, eta_d=0.95):
        records = []
        e_stored = soc_ini * e_nom
        e_min = soc_min * e_nom
        e_max = soc_max * e_nom
        
        for h in range(24):
            p_req = profile[h]
            soc_start = e_stored / e_nom
            
            if p_req < 0: # Carga
                p_chg_req = abs(p_req)
                max_chem = e_max - e_stored
                p_chg_lim = min(p_max, max_chem / (eta_c * 1.0))
                p_ac_chg = min(p_chg_req, p_chg_lim)
                delta_chem = p_ac_chg * eta_c * 1.0
                e_stored += delta_chem
                loss = p_ac_chg * (1.0 - eta_c) * 1.0
                p_ac = -p_ac_chg
                e_chg = p_ac_chg * 1.0
                e_dis = 0.0
            elif p_req > 0: # Descarga
                p_dis_req = p_req
                max_chem = e_stored - e_min
                p_dis_lim = min(p_max, (max_chem * eta_d) / 1.0)
                p_ac_dis = min(p_dis_req, p_dis_lim)
                delta_chem = (p_ac_dis / eta_d) * 1.0
                e_stored -= delta_chem
                loss = delta_chem - (p_ac_dis * 1.0)
                p_ac = p_ac_dis
                e_chg = 0.0
                e_dis = p_ac_dis * 1.0
            else:
                p_ac = 0.0
                loss = 0.0
                e_chg = 0.0
                e_dis = 0.0
                
            soc_end = e_stored / e_nom
            records.append({
                'SISTEMA_BESS': name,
                'HORA': h + 1,
                'POTENCIA_AC_KW': round(p_ac, 2),
                'ENERGIA_CARGA_KWH': round(e_chg, 2),
                'ENERGIA_DESCARGA_KWH': round(e_dis, 2),
                'PERDAS_CONVERSAO_KWH': round(loss, 2),
                'ENERGIA_ARMAZENADA_KWH': round(e_stored, 2),
                'SOC_INICIO_PCT': round(soc_start * 100.0, 2),
                'SOC_FIM_PCT': round(soc_end * 100.0, 2),
                'STATUS_OPERACIONAL': 'CARREGANDO' if p_ac < 0 else ('DESCARREGANDO' if p_ac > 0 else 'STANDBY')
            })
            
        return pd.DataFrame(records)
        
    df_bess_c = simulate_bess_lifecycle('BESS_SE_Centralizado (1.5 MW / 3.0 MWh)', bess_c_profile, 3000.0, 1500.0, soc_ini=0.20, soc_min=0.10, soc_max=0.90)
    
    df_bess_d1 = simulate_bess_lifecycle('BESS_Distribuido_TR1193581 (50 kW / 150 kWh)', bess_d_profile, 150.0, 50.0, soc_ini=0.20, soc_min=0.10, soc_max=0.90)
    df_bess_d2 = simulate_bess_lifecycle('BESS_Distribuido_TR1193923 (50 kW / 150 kWh)', bess_d_profile, 150.0, 50.0, soc_ini=0.20, soc_min=0.10, soc_max=0.90)
    df_bess_d3 = simulate_bess_lifecycle('BESS_Distribuido_TR1193996 (50 kW / 150 kWh)', bess_d_profile, 150.0, 50.0, soc_ini=0.20, soc_min=0.10, soc_max=0.90)
    
    df_balanco = pd.concat([df_bess_c, df_bess_d1, df_bess_d2, df_bess_d3], ignore_index=True)
    return df_balanco

def compare_before_after(df_new):
    # Carregar resultados da Missao 2 (auditoria anterior)
    prev_file = os.path.join(PREV_DIR, 'reexecucao_cenarios_independente.csv')
    df_prev = pd.read_csv(prev_file)
    
    rows = []
    cols = ['PICO_DEMANDA_KW', 'HORA_PICO', 'DEMANDA_MIN_KW', 'ENERGIA_DIA_KWH', 'PERDAS_DIA_KWH', 'PERDAS_PCT', 'VMIN_PU', 'VMAX_PU']
    
    for _, r_new in df_new.iterrows():
        sc = r_new['CENARIO']
        r_prev = df_prev[df_prev['CENARIO'] == sc].iloc[0]
        
        for c in cols:
            v_ant = float(r_prev[c])
            v_dep = float(r_new[c])
            diff = v_dep - v_ant
            pct = (diff / v_ant) * 100.0 if abs(v_ant) > 1e-6 else 0.0
            
            rows.append({
                'CENARIO': sc,
                'INDICADOR': c,
                'VALOR_MISSAO_2': v_ant,
                'VALOR_MISSAO_3': v_dep,
                'DIFERENCA_ABSOLUTA': round(diff, 4),
                'DIFERENCA_PERCENTUAL': round(pct, 4),
                'STATUS': 'ALTERADO' if abs(diff) > 1e-4 else 'INALTERADO'
            })
            
    return pd.DataFrame(rows)

def main():
    print("Iniciando orquestrador da Missão 3...")
    df_summary, df_hourly, df_trafos = run_simulation()
    
    # 1. Salvar RESULTADOS_CENARIOS_COBRIC_MISSAO_3.csv
    out_summary = os.path.join(OUTPUT_DIR, 'RESULTADOS_CENARIOS_COBRIC_MISSAO_3.csv')
    df_summary.to_csv(out_summary, index=False)
    print(f"Salvo: {out_summary}")
    
    # 2. Salvar CURVAS_HORARIAS_COBRIC_MISSAO_3.csv
    out_hourly = os.path.join(OUTPUT_DIR, 'CURVAS_HORARIAS_COBRIC_MISSAO_3.csv')
    df_hourly.to_csv(out_hourly, index=False)
    print(f"Salvo: {out_hourly}")
    
    # 3. Gerar e salvar BALANCO_ENERGETICO_BESS.csv
    df_bess_balanco = compute_bess_energy_balance()
    out_bess = os.path.join(OUTPUT_DIR, 'BALANCO_ENERGETICO_BESS.csv')
    df_bess_balanco.to_csv(out_bess, index=False)
    print(f"Salvo: {out_bess}")
    
    # 4. Gerar e salvar COMPARACAO_ANTES_DEPOIS.csv
    df_comp = compare_before_after(df_summary)
    out_comp = os.path.join(OUTPUT_DIR, 'COMPARACAO_ANTES_DEPOIS.csv')
    df_comp.to_csv(out_comp, index=False)
    print(f"Salvo: {out_comp}")
    
    # Salvar carregamento de trafos para análise
    out_tr = os.path.join(OUTPUT_DIR, 'carregamento_trafos_missao_3.csv')
    df_all_trafos = df_trafos
    df_all_trafos.to_csv(out_tr, index=False)
    
    print("\n" + "=" * 80)
    print("RESUMO CONSOLIDADO DOS CENÁRIOS — MISSÃO 3 (MODELO FÍSICO BESS)")
    print("=" * 80)
    print(df_summary.to_string(index=False))
    print("=" * 80)

if __name__ == '__main__':
    main()
