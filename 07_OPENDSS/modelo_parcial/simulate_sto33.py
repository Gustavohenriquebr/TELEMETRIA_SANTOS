import os
import opendssdirect as dss

def run():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    master_file = os.path.join(script_dir, 'Master.dss')
    
    print("Executando simulacao OpenDSS do Alimentador STO33...")
    dss.Basic.ClearAll()
    dss.Text.Command(f'Redirect "{master_file}"')
    
    convergiu = dss.Solution.Converged()
    print(f"Status da Convergencia: {'CONVERGIU COM SUCESSO' if convergiu else 'NAO CONVERGIU'}")
    
    if convergiu:
        # Perdas totais
        losses = dss.Circuit.Losses()
        total_p_loss_kw = losses[0] / 1000.0
        total_q_loss_kvar = losses[1] / 1000.0
        
        # Potencia total da fonte
        dss.Circuit.SetActiveElement("Vsource.Source")
        powers = dss.CktElement.Powers()
        p_total_kw = -sum(powers[0:6:2])
        q_total_kvar = -sum(powers[1:6:2])
        
        # Tensoes minimas e maximas pu
        voltages_pu = dss.Circuit.AllBusMagPu()
        valid_v = [v for v in voltages_pu if v > 0.05]
        v_min_pu = min(valid_v) if valid_v else 0.0
        v_max_pu = max(valid_v) if valid_v else 0.0
        
        print("------------------------------------------------------------")
        print(f"Potencia Ativa Injetada pela Subestacao: {p_total_kw:.2f} kW ({p_total_kw/1000:.3f} MW)")
        print(f"Potencia Reativa Injetada:              {q_total_kvar:.2f} kvar")
        print(f"Perdas Ativas Totais na Rede:           {total_p_loss_kw:.2f} kW ({total_p_loss_kw/p_total_kw*100:.2f}%)")
        print(f"Tensao Minima Registrada:               {v_min_pu:.4f} pu")
        print(f"Tensao Maxima Registrada:               {v_max_pu:.4f} pu")
        print(f"Total de Barras no Circuito:            {dss.Circuit.NumBuses()}")
        print(f"Total de Elementos de Linha:            {dss.Lines.Count()}")
        print(f"Total de Transformadores Ativos:        {dss.Transformers.Count()}")
        print(f"Total de Cargas Modeladas:              {dss.Loads.Count()}")
        print(f"Total de Usinas PV Ativas:              {dss.PVsystems.Count()}")
        print("------------------------------------------------------------")

if __name__ == '__main__':
    run()
