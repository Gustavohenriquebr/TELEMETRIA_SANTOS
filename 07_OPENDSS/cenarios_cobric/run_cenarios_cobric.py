#!/usr/bin/env python3
"""
SIMULAÇÃO E COMPARAÇÃO DOS 5 CENÁRIOS DO COBRIC 2026 — ALIMENTADOR STO33
Executa a simulação quase-estática temporal (QSTS de 24 horas, passo 1h)
para cada cenário no OpenDSS, calculando perdas técnicas, tensão mínima/máxima,
alívio de ponta na subestação e CAPEX estimado.
"""

import os
import opendssdirect as dss
import pandas as pd
import numpy as np

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
REPORTS_DIR = os.path.join(os.path.dirname(os.path.dirname(BASE_DIR)), '08_RELATORIOS')

def run_scenario(name, dss_file):
    dss.Basic.ClearAll()
    dss.Text.Command(f'Redirect "{dss_file}"')
    
    # Executa solve inicial em Snapshot para inicializar vetores nodais da rede
    dss.Text.Command('Set Mode=Snapshot')
    dss.Solution.Solve()
    
    # Inicia modo diário (QSTS) passo 1 hora
    dss.Text.Command('Set Mode=Daily number=1 stepsize=1h')
    
    records = []
    for h in range(24):
        dss.Solution.Solve()
        
        # Potência líquida suprida pela subestação / rede supridora
        tot_p, tot_q = dss.Circuit.TotalPower()
        p_sub = -tot_p
        q_sub = -tot_q
        
        # Perdas técnicas ativas
        losses = dss.Circuit.Losses()
        p_loss = losses[0] / 1000.0
        
        # Tensões nodais em pu (desconsiderando nós desconectados/zero)
        v_pu = [v for v in dss.Circuit.AllBusMagPu() if v > 0.05]
        vmin = min(v_pu) if v_pu else 1.0
        vmax = max(v_pu) if v_pu else 1.0
        
        records.append({
            'CENARIO': name,
            'HORA': h + 1,
            'P_SUB_KW': round(p_sub, 2),
            'Q_SUB_KVAR': round(q_sub, 2),
            'PERDAS_KW': round(p_loss, 2),
            'VMIN_PU': round(vmin, 4),
            'VMAX_PU': round(vmax, 4)
        })
        
    return pd.DataFrame(records)

def main():
    print("=" * 75)
    print("EXECUTANDO SIMULAÇÃO COMPARATIVA DOS 5 CENÁRIOS DO COBRIC 2026")
    print("ALIMENTADOR STO33 (SANTOS 1 / CPFL PIRATININGA)")
    print("=" * 75)
    
    # 1. CENÁRIO A: BASELINE
    print("\n[1/5] Executando Cenário A: Rede Atual Baseline (195.89 kW FV)...")
    file_a = os.path.join(BASE_DIR, 'Cenario_A_Baseline.dss')
    df_a = run_scenario('Cenário A (Baseline)', file_a)
    
    # 2. CENÁRIO B: EXPANSAO FV + VE
    print("[2/5] Executando Cenário B: Expansão FV (+100%) + Eletromobilidade (100 VEs)...")
    file_b = os.path.join(BASE_DIR, 'Cenario_B_Expansao_FV_VE.dss')
    df_b = run_scenario('Cenário B (FV + VE)', file_b)
    
    # 3. CENÁRIO C: BESS CENTRALIZADO
    print("[3/5] Executando Cenário C: BESS Centralizado na Subestação (1.5 MW / 3.0 MWh)...")
    file_c = os.path.join(BASE_DIR, 'Cenario_C_BESS_Centralizado.dss')
    df_c = run_scenario('Cenário C (BESS Centralizado)', file_c)
    
    # 4. CENÁRIO D: BESS DISTRIBUIDO
    print("[4/5] Executando Cenário D: BESS Distribuído nos Trafos Críticos (3 x 50 kW / 150 kWh)...")
    file_d = os.path.join(BASE_DIR, 'Cenario_D_BESS_Distribuido.dss')
    df_d = run_scenario('Cenário D (BESS Distribuído)', file_d)
    
    # 5. CENÁRIO E: REFORÇO FÍSICO CONVENCIONAL
    print("[5/5] Executando Cenário E: Reforço Físico Convencional de Rede (Wires Alternative)...")
    file_e = os.path.join(BASE_DIR, 'Cenario_E_Reforco_Fisico.dss')
    df_e = run_scenario('Cenário E (Reforço Físico)', file_e)
    
    # CONSOLIDAÇÃO DOS DADOS
    all_df = pd.concat([df_a, df_b, df_c, df_d, df_e], ignore_index=True)
    
    capex_map = {
        'Cenário A (Baseline)': 0.0,
        'Cenário B (FV + VE)': 0.0,  # Investimento de prosumidores/usuários
        'Cenário C (BESS Centralizado)': 3000 * 1650.0,  # 3.000 kWh @ R$ 1.650/kWh = R$ 4,95M
        'Cenário D (BESS Distribuído)': 3 * 150 * 2100.0,  # 450 kWh @ R$ 2.100/kWh = R$ 945k
        'Cenário E (Reforço Físico)': (3 * 42000.0) + (4.5 * 95000.0)  # 3 trafos + 4,5 km condutores = R$ 553,5k
    }
    
    summary = []
    for sc in all_df['CENARIO'].unique():
        sub = all_df[all_df['CENARIO'] == sc]
        p_peak = sub['P_SUB_KW'].max()
        p_peak_h = sub.loc[sub['P_SUB_KW'].idxmax(), 'HORA']
        p_min = sub['P_SUB_KW'].min()
        e_dia = sub['P_SUB_KW'].sum()
        perdas_dia = sub['PERDAS_KW'].sum()
        pct_perdas = perdas_dia / e_dia * 100 if e_dia > 0 else 0
        vmin = sub['VMIN_PU'].min()
        vmax = sub['VMAX_PU'].max()
        capex = capex_map.get(sc, 0.0)
        
        summary.append({
            'CENARIO': sc,
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
        
    df_res = pd.DataFrame(summary)
    
    print("\n" + "=" * 80)
    print("MATRIZ DE INDICADORES COMPARATIVOS DO COBRIC 2026 (SIMULADO NO OPENDSS)")
    print("=" * 80)
    print(df_res.to_string(index=False))
    print("=" * 80)
    
    # Salvar resultados
    out_csv = os.path.join(REPORTS_DIR, 'RESULTADOS_CENARIOS_COBRIC.csv')
    df_res.to_csv(out_csv, index=False)
    print(f"\nResumo salvo em: {out_csv}")
    
    curves_csv = os.path.join(REPORTS_DIR, 'CURVAS_HORARIAS_CENARIOS_COBRIC.csv')
    all_df.to_csv(curves_csv, index=False)
    print(f"Curvas horárias completas salvas em: {curves_csv}")

if __name__ == '__main__':
    main()
