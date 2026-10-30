import os
import opendssdirect as dss
import pandas as pd
import numpy as np

def run():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    master_file = os.path.join(script_dir, 'Master.dss')
    
    print("=" * 70)
    print("SIMULACAO OPENDSS DO ALIMENTADOR STO33 (SANTOS 1 / CPFL PIRATININGA)")
    print("REDE DE MEDIA TENSAO (13.8 kV) E BAIXA TENSAO (0.22 kV)")
    print("=" * 70)
    
    # -------------------------------------------------------------------------
    # 1. FLUXO DE CARGA ESTÁTICO (SNAPSHOT)
    # -------------------------------------------------------------------------
    dss.Basic.ClearAll()
    dss.Text.Command(f'Redirect "{master_file}"')
    
    convergiu = dss.Solution.Converged()
    print(f"\n[1] FLUXO DE CARGA ESTÁTICO (SNAPSHOT): {'CONVERGIU' if convergiu else 'FALHOU'}")
    
    if convergiu:
        losses = dss.Circuit.Losses()
        total_p_loss_kw = losses[0] / 1000.0
        total_q_loss_kvar = losses[1] / 1000.0
        
        dss.Circuit.SetActiveElement("Vsource.Source")
        powers = dss.CktElement.Powers()
        p_total_kw = -sum(powers[0:6:2])
        q_total_kvar = -sum(powers[1:6:2])
        s_total_kva = (p_total_kw**2 + q_total_kvar**2)**0.5
        fp_total = p_total_kw / s_total_kva if s_total_kva > 0 else 1.0
        
        voltages_pu = [v for v in dss.Circuit.AllBusMagPu() if v > 0.05]
        v_min_pu = min(voltages_pu) if voltages_pu else 0.0
        v_max_pu = max(voltages_pu) if voltages_pu else 0.0
        
        print("------------------------------------------------------------")
        print(f"Potência Ativa Injetada na SE:         {p_total_kw:10.2f} kW ({p_total_kw/1000:.3f} MW)")
        print(f"Potência Reativa Injetada na SE:       {q_total_kvar:10.2f} kvar")
        print(f"Fator de Potência Global na SE:        {fp_total:10.4f} ({'Indutivo' if q_total_kvar > 0 else 'Capacitivo'})")
        print(f"Perdas Ativas Totais na Rede:          {total_p_loss_kw:10.2f} kW ({total_p_loss_kw/p_total_kw*100:.2f}%)")
        print(f"Tensão Mínima Registrada:              {v_min_pu:10.4f} pu")
        print(f"Tensão Máxima Registrada:              {v_max_pu:10.4f} pu")
        print(f"Barras / Nós no Circuito:              {dss.Circuit.NumBuses()} barras / {dss.Circuit.NumNodes()} nós")
        print(f"Transformadores MT/BT Ativos:          {dss.Transformers.Count()}")
        print(f"Cargas Agregadas Modeladas:            {dss.Loads.Count()}")
        pv_count = dss.PVsystems.Count()
        total_pmpp = 0.0
        if dss.PVsystems.First() > 0:
            for _ in range(pv_count):
                total_pmpp += dss.PVsystems.Pmpp()
                dss.PVsystems.Next()
        print(f"Usinas Solares FV (Oficial MMGD):      {pv_count} ({total_pmpp:.2f} kW total)")
        print(f"Bancos de Capacitores Ativos:          {dss.Capacitors.Count()} (600 kvar)")
        print("------------------------------------------------------------")

    # -------------------------------------------------------------------------
    # 2. SIMULAÇÃO QUASE-ESTÁTICA TEMPORAL (QSTS DIÁRIA — 24 HORAS)
    # -------------------------------------------------------------------------
    print("\n[2] SIMULACAO QUASE-ESTATICA TEMPORAL (QSTS — 24 HORAS, PASSO 1 HORA)")
    dss.Text.Command('Set Mode=Daily number=1 stepsize=1h')
    
    hourly_records = []
    for h in range(24):
        dss.Text.Command(f'Set hour={h}')
        dss.Solution.Solve()
        
        losses = dss.Circuit.Losses()
        p_loss_kw = losses[0] / 1000.0
        
        dss.Circuit.SetActiveElement("Vsource.Source")
        powers = dss.CktElement.Powers()
        p_sub = -sum(powers[0:6:2])
        q_sub = -sum(powers[1:6:2])
        
        v_pu = [v for v in dss.Circuit.AllBusMagPu() if v > 0.05]
        vmin = min(v_pu)
        vmax = max(v_pu)
        
        hourly_records.append({
            'HORA': h + 1,
            'P_SUB_KW': round(p_sub, 2),
            'Q_SUB_KVAR': round(q_sub, 2),
            'PERDAS_KW': round(p_loss_kw, 2),
            'PERDAS_PCT': round(p_loss_kw / p_sub * 100, 2) if p_sub > 0 else 0.0,
            'VMIN_PU': round(vmin, 4),
            'VMAX_PU': round(vmax, 4)
        })
        
    df_daily = pd.DataFrame(hourly_records)
    
    # Energia diária e pico
    e_total_kwh = df_daily['P_SUB_KW'].sum()
    e_loss_kwh = df_daily['PERDAS_KW'].sum()
    p_peak_row = df_daily.loc[df_daily['P_SUB_KW'].idxmax()]
    p_min_row = df_daily.loc[df_daily['P_SUB_KW'].idxmin()]
    
    print("------------------------------------------------------------")
    print(f"Energia Ativa Total Injetada no Dia:   {e_total_kwh:10.2f} kWh ({e_total_kwh/1000:.2f} MWh/dia)")
    print(f"Perdas Técnicas Diárias Acumuladas:    {e_loss_kwh:10.2f} kWh ({e_loss_kwh/e_total_kwh*100:.2f}%)")
    print(f"Demanda Máxima de Ponta (Hora {int(p_peak_row['HORA']):02d}h):   {p_peak_row['P_SUB_KW']:10.2f} kW (Vmin: {p_peak_row['VMIN_PU']:.4f} pu)")
    print(f"Demanda Mínima (Hora {int(p_min_row['HORA']):02d}h):            {p_min_row['P_SUB_KW']:10.2f} kW (Vmax: {p_min_row['VMAX_PU']:.4f} pu)")
    print("------------------------------------------------------------")
    
    # Salvar resultados horários
    out_csv = os.path.join(script_dir, '..', 'dados_modelo', 'resultado_fluxo_diario_sto33.csv')
    df_daily.to_csv(out_csv, index=False)
    print(f"Curvas horárias de 24h salvas com sucesso em: {out_csv}")
    print("=" * 70)

if __name__ == '__main__':
    run()
