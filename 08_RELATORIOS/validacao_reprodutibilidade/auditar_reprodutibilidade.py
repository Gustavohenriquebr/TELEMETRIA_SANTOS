#!/usr/bin/env python3
"""
AUDITORIA INDEPENDENTE DE REPRODUTIBILIDADE DOS 5 CENARIOS COBRIC 2026
Executa os modelos OpenDSS sem modificar os arquivos originais,
coletando metricas eletricas nodais, balanco de potencia e comparacao
precisa com os valores publicados.
"""

import os
import sys
import platform
import opendssdirect as dss
import pandas as pd
import numpy as np

BASE_DIR = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
DSS_SCENARIOS_DIR = os.path.join(BASE_DIR, '07_OPENDSS', 'cenarios_cobric')
REPORTS_DIR = os.path.join(BASE_DIR, '08_RELATORIOS')
OUTPUT_DIR = os.path.join(REPORTS_DIR, 'validacao_reprodutibilidade')

# Trafos críticos para monitoramento local
CRITICAL_TRAFOS = ['TR_1193581', 'TR_1193923', 'TR_1193996', 'TR_48964485']

def execute_scenario(scenario_name, dss_file):
    dss.Basic.ClearAll()
    dss.Text.Command(f'Redirect "{dss_file}"')
    
    # 1. Snapshot solve inicial
    dss.Text.Command('Set Mode=Snapshot')
    dss.Solution.Solve()
    conv_snap = dss.Solution.Converged()
    iters_snap = dss.Solution.Iterations()
    
    # Informações de contagem
    num_buses = dss.Circuit.NumBuses()
    num_nodes = dss.Circuit.NumNodes()
    num_lines = dss.Lines.Count()
    num_trafos = dss.Transformers.Count()
    num_loads = dss.Loads.Count()
    num_pv = dss.PVsystems.Count()
    num_caps = dss.Capacitors.Count()
    
    # Potencia PV total nominal
    total_pv_pmpp = 0.0
    if dss.PVsystems.First() > 0:
        for _ in range(num_pv):
            total_pv_pmpp += dss.PVsystems.Pmpp()
            dss.PVsystems.Next()
            
    # Potencia de Transformadores nominal total
    total_trafo_kva = 0.0
    trafo_ratings = {}
    if dss.Transformers.First() > 0:
        for _ in range(num_trafos):
            t_name = dss.Transformers.Name()
            kva = dss.Transformers.kVA()
            trafo_ratings[t_name] = kva
            total_trafo_kva += kva
            dss.Transformers.Next()
            
    # 2. Modo diário (QSTS) passo 1h
    dss.Text.Command('Set Mode=Daily number=1 stepsize=1h')
    
    hourly_records = []
    trafo_loading_records = []
    
    for h in range(24):
        dss.Solution.Solve()
        conv = dss.Solution.Converged()
        
        # Subestacao (Vsource.source / Circuit.TotalPower)
        tot_p, tot_q = dss.Circuit.TotalPower()
        p_sub = -tot_p
        q_sub = -tot_q
        
        # Perdas totais
        losses = dss.Circuit.Losses()
        p_loss = losses[0] / 1000.0
        q_loss = losses[1] / 1000.0
        
        # Tensoes em pu
        v_pu = [v for v in dss.Circuit.AllBusMagPu() if v > 0.05]
        vmin = min(v_pu) if v_pu else 1.0
        vmax = max(v_pu) if v_pu else 1.0
        
        # Bus com Vmin
        bus_names = dss.Circuit.AllBusNames()
        vmin_bus = 'unknown'
        for b_name in bus_names:
            dss.Circuit.SetActiveBus(b_name)
            b_vpu = dss.Bus.PuVoltage()
            b_mag = [b_vpu[i] for i in range(0, len(b_vpu), 2)]
            if any(abs(m - vmin) < 1e-4 for m in b_mag if m > 0.05):
                vmin_bus = b_name
                break
                
        # Cargas totais demandadas
        p_loads_now = 0.0
        q_loads_now = 0.0
        if dss.Loads.First() > 0:
            for _ in range(num_loads):
                dss.Circuit.SetActiveElement(f"Load.{dss.Loads.Name()}")
                lp = dss.CktElement.Powers()
                p_loads_now += sum(lp[0:len(lp):2])
                q_loads_now += sum(lp[1:len(lp):2])
                dss.Loads.Next()
                
        # Geração Solar total injetada
        p_pv_now = 0.0
        q_pv_now = 0.0
        if dss.PVsystems.First() > 0:
            for _ in range(num_pv):
                dss.Circuit.SetActiveElement(f"PVSystem.{dss.PVsystems.Name()}")
                pv_p = dss.CktElement.Powers()
                p_pv_now += -sum(pv_p[0:len(pv_p):2])
                q_pv_now += -sum(pv_p[1:len(pv_p):2])
                dss.PVsystems.Next()
                
        # Geração / BESS (Generators adicionais de BESS)
        p_gen_now = 0.0
        q_gen_now = 0.0
        num_gens = dss.Generators.Count()
        if dss.Generators.First() > 0:
            for _ in range(num_gens):
                dss.Circuit.SetActiveElement(f"Generator.{dss.Generators.Name()}")
                gp = dss.CktElement.Powers()
                p_gen_now += -sum(gp[0:len(gp):2])
                q_gen_now += -sum(gp[1:len(gp):2])
                dss.Generators.Next()
                
        # Balanço de potência ativa: P_sub + P_pv + P_gen = P_loads + P_loss
        balanco_erro = abs((p_sub + p_pv_now + p_gen_now) - (p_loads_now + p_loss))
        
        hourly_records.append({
            'CENARIO': scenario_name,
            'HORA': h + 1,
            'P_SUB_KW': round(p_sub, 2),
            'Q_SUB_KVAR': round(q_sub, 2),
            'PERDAS_KW': round(p_loss, 2),
            'PERDAS_KVAR': round(q_loss, 2),
            'VMIN_PU': round(vmin, 4),
            'VMAX_PU': round(vmax, 4),
            'VMIN_BUS': vmin_bus,
            'P_LOADS_KW': round(p_loads_now, 2),
            'P_PV_KW': round(p_pv_now, 2),
            'P_BESS_KW': round(p_gen_now, 2),
            'ERRO_BALANCO_KW': round(balanco_erro, 4),
            'CONVERGENCIA': conv
        })
        
        # Carregamento dos transformadores críticos
        for tr in CRITICAL_TRAFOS:
            dss.Circuit.SetActiveElement(f"Transformer.{tr}")
            tr_powers = dss.CktElement.Powers()
            tr_p = sum(tr_powers[0:6:2])
            tr_q = sum(tr_powers[1:6:2])
            tr_s = np.sqrt(tr_p**2 + tr_q**2)
            tr_nom = trafo_ratings.get(tr.lower(), trafo_ratings.get(tr, 225.0))
            pct_load = (tr_s / tr_nom) * 100.0 if tr_nom > 0 else 0.0
            
            trafo_loading_records.append({
                'CENARIO': scenario_name,
                'HORA': h + 1,
                'TRAFO': tr,
                'CAP_KVA': tr_nom,
                'DEMANDA_P_KW': round(tr_p, 2),
                'DEMANDA_S_KVA': round(tr_s, 2),
                'CARREGAMENTO_PCT': round(pct_load, 2)
            })
            
    df_hourly = pd.DataFrame(hourly_records)
    df_trafos = pd.DataFrame(trafo_loading_records)
    
    meta = {
        'num_buses': num_buses,
        'num_nodes': num_nodes,
        'num_lines': num_lines,
        'num_trafos': num_trafos,
        'num_loads': num_loads,
        'num_pv': num_pv,
        'num_caps': num_caps,
        'total_pv_pmpp': total_pv_pmpp,
        'total_trafo_kva': total_trafo_kva,
        'conv_snap': conv_snap,
        'iters_snap': iters_snap
    }
    
    return df_hourly, df_trafos, meta

def main():
    print("Iniciando auditoria independente de reprodutibilidade dos 5 cenários...")
    
    scenarios = [
        ('Cenário A (Baseline)', os.path.join(DSS_SCENARIOS_DIR, 'Cenario_A_Baseline.dss')),
        ('Cenário B (FV + VE)', os.path.join(DSS_SCENARIOS_DIR, 'Cenario_B_Expansao_FV_VE.dss')),
        ('Cenário C (BESS Centralizado)', os.path.join(DSS_SCENARIOS_DIR, 'Cenario_C_BESS_Centralizado.dss')),
        ('Cenário D (BESS Distribuído)', os.path.join(DSS_SCENARIOS_DIR, 'Cenario_D_BESS_Distribuido.dss')),
        ('Cenário E (Reforço Físico)', os.path.join(DSS_SCENARIOS_DIR, 'Cenario_E_Reforco_Fisico.dss'))
    ]
    
    all_hourly = []
    all_trafos = []
    summaries = []
    
    capex_map = {
        'Cenário A (Baseline)': 0.0,
        'Cenário B (FV + VE)': 0.0,
        'Cenário C (BESS Centralizado)': 4950000.0,
        'Cenário D (BESS Distribuído)': 945000.0,
        'Cenário E (Reforço Físico)': 553500.0
    }
    
    metas = {}
    
    for name, dss_file in scenarios:
        print(f"Executando {name}...")
        df_h, df_tr, meta = execute_scenario(name, dss_file)
        all_hourly.append(df_h)
        all_trafos.append(df_tr)
        metas[name] = meta
        
        p_peak = df_h['P_SUB_KW'].max()
        p_peak_h = df_h.loc[df_h['P_SUB_KW'].idxmax(), 'HORA']
        p_min = df_h['P_SUB_KW'].min()
        e_dia = df_h['P_SUB_KW'].sum()
        perdas_dia = df_h['PERDAS_KW'].sum()
        pct_perdas = (perdas_dia / e_dia) * 100.0 if e_dia > 0 else 0.0
        vmin = df_h['VMIN_PU'].min()
        vmax = df_h['VMAX_PU'].max()
        capex = capex_map.get(name, 0.0)
        
        summaries.append({
            'CENARIO': name,
            'PICO_DEMANDA_KW': round(p_peak, 2),
            'HORA_PICO': int(p_peak_h),
            'DEMANDA_MIN_KW': round(p_min, 2),
            'ENERGIA_DIA_KWH': round(e_dia, 2),
            'PERDAS_DIA_KWH': round(perdas_dia, 2),
            'PERDAS_PCT': round(pct_perdas, 2),
            'VMIN_PU': round(vmin, 4),
            'VMAX_PU': round(vmax, 4),
            'CAPEX_ESTIMADO_RS': capex
        })
        
    df_reexec = pd.DataFrame(summaries)
    df_all_hourly = pd.concat(all_hourly, ignore_index=True)
    df_all_trafos = pd.concat(all_trafos, ignore_index=True)
    
    # Salvar reexecução independente
    reexec_csv = os.path.join(OUTPUT_DIR, 'reexecucao_cenarios_independente.csv')
    df_reexec.to_csv(reexec_csv, index=False)
    print(f"Resultados reexecutados salvos em: {reexec_csv}")
    
    hourly_reexec_csv = os.path.join(OUTPUT_DIR, 'reexecucao_horaria_detalhada.csv')
    df_all_hourly.to_csv(hourly_reexec_csv, index=False)
    
    trafo_csv = os.path.join(OUTPUT_DIR, 'carregamento_trafos_criticos_cenarios.csv')
    df_all_trafos.to_csv(trafo_csv, index=False)
    
    # Leitura do CSV publicado para comparação matemática estrita
    pub_csv = os.path.join(REPORTS_DIR, 'RESULTADOS_CENARIOS_COBRIC.csv')
    df_pub = pd.read_csv(pub_csv)
    
    comparacao_rows = []
    metricas = [
        ('PICO_DEMANDA_KW', 'kW'),
        ('HORA_PICO', 'h'),
        ('DEMANDA_MIN_KW', 'kW'),
        ('ENERGIA_DIA_KWH', 'kWh/dia'),
        ('PERDAS_DIA_KWH', 'kWh/dia'),
        ('PERDAS_PCT', '%'),
        ('VMIN_PU', 'pu'),
        ('VMAX_PU', 'pu'),
        ('CAPEX_ESTIMADO_RS', 'R$')
    ]
    
    for _, row_pub in df_pub.iterrows():
        sc = row_pub['CENARIO']
        row_re = df_reexec[df_reexec['CENARIO'] == sc].iloc[0]
        
        for col, unid in metricas:
            v_pub = float(row_pub[col])
            v_re = float(row_re[col])
            diff_abs = v_re - v_pub
            diff_pct = (diff_abs / v_pub * 100.0) if abs(v_pub) > 1e-9 else 0.0
            
            status = 'EXATO' if abs(diff_abs) < 1e-4 else ('DIVERGENCIA_MINIMA' if abs(diff_pct) < 0.1 else 'DIVERGENTE')
            
            comparacao_rows.append({
                'CENARIO': sc,
                'INDICADOR': col,
                'UNIDADE': unid,
                'VALOR_PUBLICADO': v_pub,
                'VALOR_REEXECUTADO': v_re,
                'DIFERENCA_ABSOLUTA': round(diff_abs, 4),
                'DIFERENCA_PERCENTUAL': round(diff_pct, 4),
                'STATUS': status
            })
            
    df_comp = pd.DataFrame(comparacao_rows)
    comp_csv = os.path.join(OUTPUT_DIR, 'COMPARACAO_RESULTADOS_PUBLICADOS.csv')
    df_comp.to_csv(comp_csv, index=False)
    print(f"Tabela de comparação salva em: {comp_csv}")
    
    # Exibir resumo da comparação
    print("\n" + "=" * 90)
    print("RESUMO DA COMPARAÇÃO DE REPRODUTIBILIDADE:")
    print("=" * 90)
    print(df_comp.to_string(index=False))
    print("=" * 90)
    
    # Salvar metadados de execução
    meta_df = pd.DataFrame.from_dict(metas, orient='index')
    meta_df.to_csv(os.path.join(OUTPUT_DIR, 'metadados_circuitos_cenarios.csv'))

if __name__ == '__main__':
    main()
