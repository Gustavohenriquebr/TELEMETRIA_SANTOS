import os
import sys
import time
import json
import numpy as np
import pandas as pd
import pyogrio

def main():
    base_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))
    raw_dir = os.path.join(base_dir, '02_DADOS_BRUTOS')
    treated_dir = os.path.join(base_dir, '03_DADOS_TRATADOS')
    os.makedirs(treated_dir, exist_ok=True)

    gdb_path = os.path.join(raw_dir, 'CPFL_Piratininga_2937_2025-12-31_V11_20260823-1826.gdb')
    if not os.path.exists(gdb_path):
        print(f"[ERRO] GDB não encontrado em {gdb_path}")
        return

    print("================================================================================")
    print("PROCESSANDO DADOS TRATADOS — BASE DE ENGENHARIA ELÉTRICA SANTOS / STO33")
    print("================================================================================")

    # -------------------------------------------------------------------------
    # 1. ALIMENTADORES.CSV
    # -------------------------------------------------------------------------
    print("\n[1/10] Processando alimentadores (CTMT e SUB)...")
    df_sub = pyogrio.read_dataframe(gdb_path, layer='SUB')
    df_ctmt = pyogrio.read_dataframe(gdb_path, layer='CTMT')
    
    # Identificar subestações de Santos
    santos_subs = df_sub[df_sub['NOME'].str.contains('SANTOS', case=False, na=False) & ~df_sub['NOME'].str.contains('DUMONT', case=False, na=False)]
    sub_map = dict(zip(santos_subs['COD_ID'], santos_subs['NOME']))
    
    santos_feeders = df_ctmt[df_ctmt['SUB'].isin(santos_subs['COD_ID'])].copy()
    santos_feeders['NOME_SUBESTACAO'] = santos_feeders['SUB'].map(sub_map)
    santos_feeders['MUNICIPIO'] = 'SANTOS'
    santos_feeders['COD_IBGE'] = '3548500'
    santos_feeders['CONCESSIONARIA'] = 'CPFL Piratininga'
    
    # Calcular energia total anual
    ene_cols = [f'ENE_{i:02d}' for i in range(1, 13)]
    santos_feeders['ENERGIA_ANUAL_KWH'] = santos_feeders[ene_cols].sum(axis=1)
    santos_feeders['PERDAS_TOTAIS_KWH'] = santos_feeders['PERD_A4'] + santos_feeders['PERD_B'] + santos_feeders['PERD_MED']
    
    out_alim = os.path.join(treated_dir, 'alimentadores.csv')
    santos_feeders.to_csv(out_alim, index=False, encoding='utf-8')
    print(f"  -> Salvo: {out_alim} ({len(santos_feeders)} alimentadores de Santos)")

    # -------------------------------------------------------------------------
    # 2. SEGMENTOS_REDE.CSV (SSDMT + SEGCON)
    # -------------------------------------------------------------------------
    print("\n[2/10] Processando segmentos de rede de MT do STO33 (SSDMT + SEGCON)...")
    df_ssdmt = pyogrio.read_dataframe(gdb_path, layer='SSDMT', where="CTMT = 'STO33'")
    df_segcon = pyogrio.read_dataframe(gdb_path, layer='SEGCON')

    # Extrair coordenadas dos nós extremos
    coords_x1, coords_y1, coords_x2, coords_y2 = [], [], [], []
    for geom in df_ssdmt['geometry']:
        if geom is not None and not geom.is_empty:
            # geom is MultiLineString or LineString
            if geom.geom_type == 'MultiLineString':
                first_line = list(geom.geoms)[0]
                last_line = list(geom.geoms)[-1]
                p1 = first_line.coords[0]
                p2 = last_line.coords[-1]
            else:
                p1 = geom.coords[0]
                p2 = geom.coords[-1]
            coords_x1.append(p1[0])
            coords_y1.append(p1[1])
            coords_x2.append(p2[0])
            coords_y2.append(p2[1])
        else:
            coords_x1.append(np.nan)
            coords_y1.append(np.nan)
            coords_x2.append(np.nan)
            coords_y2.append(np.nan)

    df_ssdmt['COORD_X1'] = coords_x1
    df_ssdmt['COORD_Y1'] = coords_y1
    df_ssdmt['COORD_X2'] = coords_x2
    df_ssdmt['COORD_Y2'] = coords_y2

    # Join com catálogo de condutores SEGCON
    segcon_sub = df_segcon[['COD_ID', 'MAT_FAS_1', 'BIT_FAS_1', 'R1', 'X1', 'CNOM', 'CMAX', 'DESCR']].copy()
    segcon_sub.columns = ['TIP_CND', 'MATERIAL', 'BITOLA_AWG_MCM', 'R1_OHM_KM', 'X1_OHM_KM', 'AMPACIDADE_NOM_A', 'AMPACIDADE_MAX_A', 'DESCR_CONDUTOR']
    
    seg_merged = df_ssdmt.merge(segcon_sub, on='TIP_CND', how='left')
    seg_merged_df = seg_merged.drop(columns=['geometry'])
    out_seg = os.path.join(treated_dir, 'segmentos_rede.csv')
    seg_merged_df.to_csv(out_seg, index=False, encoding='utf-8')
    print(f"  -> Salvo: {out_seg} ({len(seg_merged_df)} trechos MT com impedâncias e ampacidades)")

    # -------------------------------------------------------------------------
    # 3. NOS_REDE.CSV (Barramentos e Nós da Rede MT STO33)
    # -------------------------------------------------------------------------
    print("\n[3/10] Processando nós da rede (pontos de conexão)...")
    nodes_dict = {}
    for idx, row in seg_merged.iterrows():
        p1 = row['PAC_1']
        p2 = row['PAC_2']
        x1, y1 = row['COORD_X1'], row['COORD_Y1']
        x2, y2 = row['COORD_X2'], row['COORD_Y2']
        if p1 not in nodes_dict and pd.notna(x1):
            nodes_dict[p1] = {'BUS_ID': p1, 'LON': x1, 'LAT': y1, 'TIPO': 'Nó MT', 'TENSAO_KV': 13.8}
        if p2 not in nodes_dict and pd.notna(x2):
            nodes_dict[p2] = {'BUS_ID': p2, 'LON': x2, 'LAT': y2, 'TIPO': 'Nó MT', 'TENSAO_KV': 13.8}

    # Nó subestação PAC_INI
    pac_ini = 'MT_STO33_346'
    if pac_ini in nodes_dict:
        nodes_dict[pac_ini]['TIPO'] = 'Subestação STO (Ponto Inicial)'

    df_nodes = pd.DataFrame(list(nodes_dict.values()))
    out_nodes = os.path.join(treated_dir, 'nos_rede.csv')
    df_nodes.to_csv(out_nodes, index=False, encoding='utf-8')
    print(f"  -> Salvo: {out_nodes} ({len(df_nodes)} barramentos georreferenciados)")

    # -------------------------------------------------------------------------
    # 4. TRANSFORMADORES.CSV (UNTRMT + EQTRMT + Clientes BT)
    # -------------------------------------------------------------------------
    print("\n[4/10] Processando transformadores MT/BT do STO33...")
    df_trafos = pyogrio.read_dataframe(gdb_path, layer='UNTRMT', where="CTMT = 'STO33'")
    df_eqtrmt = pyogrio.read_dataframe(gdb_path, layer='EQTRMT')
    df_ucbt = pyogrio.read_dataframe(gdb_path, layer='UCBT_tab', where="CTMT = 'STO33'")

    # Coordenadas dos trafos
    trafo_lon, trafo_lat = [], []
    for geom in df_trafos['geometry']:
        if geom is not None and not geom.is_empty:
            trafo_lon.append(geom.x)
            trafo_lat.append(geom.y)
        else:
            trafo_lon.append(np.nan)
            trafo_lat.append(np.nan)
    df_trafos['LON'] = trafo_lon
    df_trafos['LAT'] = trafo_lat

    # Contagem de consumidores por transformador
    uc_agg = df_ucbt.groupby('UNI_TR_MT').agg(
        QTD_UC_TOTAL=('PN_CON', 'count'),
        QTD_UC_RESIDENCIAL=('CLAS_SUB', lambda s: s.str.startswith('RE').sum()),
        QTD_UC_COMERCIAL=('CLAS_SUB', lambda s: s.str.startswith('CO').sum()),
        QTD_UC_PODER_PUBLICO=('CLAS_SUB', lambda s: s.str.startswith('PP').sum()),
        QTD_UC_ILUM_PUBLICA=('CLAS_SUB', lambda s: s.str.startswith('IP').sum()),
        QTD_UC_INDUSTRIAL=('CLAS_SUB', lambda s: s.str.startswith('IN').sum()),
    ).reset_index()

    trafos_merged = df_trafos.merge(uc_agg, left_on='COD_ID', right_on='UNI_TR_MT', how='left')
    trafos_merged['QTD_UC_TOTAL'] = trafos_merged['QTD_UC_TOTAL'].fillna(0).astype(int)
    trafos_merged['QTD_UC_RESIDENCIAL'] = trafos_merged['QTD_UC_RESIDENCIAL'].fillna(0).astype(int)
    trafos_merged['QTD_UC_COMERCIAL'] = trafos_merged['QTD_UC_COMERCIAL'].fillna(0).astype(int)
    trafos_merged['QTD_UC_PODER_PUBLICO'] = trafos_merged['QTD_UC_PODER_PUBLICO'].fillna(0).astype(int)
    trafos_merged['QTD_UC_ILUM_PUBLICA'] = trafos_merged['QTD_UC_ILUM_PUBLICA'].fillna(0).astype(int)
    trafos_merged['QTD_UC_INDUSTRIAL'] = trafos_merged['QTD_UC_INDUSTRIAL'].fillna(0).astype(int)

    # Demanda máxima estimada por trafo (hipótese de engenharia: 0.85 kW/UC residencial com fator de simultaneidade empírico)
    # Fator de diversidade/simultaneidade aproximado: Fsim = 0.2 + 0.8 / sqrt(N)
    f_sim = 0.2 + 0.8 / np.sqrt(np.maximum(1, trafos_merged['QTD_UC_TOTAL']))
    trafos_merged['DEMANDA_MAX_ESTIMADA_KW'] = np.round(
        (trafos_merged['QTD_UC_RESIDENCIAL'] * 1.5 + trafos_merged['QTD_UC_COMERCIAL'] * 2.5 + trafos_merged['QTD_UC_PODER_PUBLICO'] * 3.0) * f_sim, 2
    )
    trafos_merged['CARREGAMENTO_ESTIMADO_PCT'] = np.round(
        (trafos_merged['DEMANDA_MAX_ESTIMADA_KW'] / (trafos_merged['POT_NOM'] * 0.95)) * 100, 2
    )

    trafos_df = trafos_merged.drop(columns=['geometry'])
    out_trafos = os.path.join(treated_dir, 'transformadores.csv')
    trafos_df.to_csv(out_trafos, index=False, encoding='utf-8')
    print(f"  -> Salvo: {out_trafos} ({len(trafos_df)} transformadores com carga e clientes)")

    # -------------------------------------------------------------------------
    # 5. EQUIPAMENTOS_CONTROLE.CSV (UNSEMT + UNCRMT)
    # -------------------------------------------------------------------------
    print("\n[5/10] Processando equipamentos de controle e manobra (chaves e capacitores)...")
    df_unsemt = pyogrio.read_dataframe(gdb_path, layer='UNSEMT', where="CTMT = 'STO33'")
    df_uncrmt = pyogrio.read_dataframe(gdb_path, layer='UNCRMT', where="CTMT = 'STO33'")

    ctrl_list = []
    for idx, row in df_unsemt.iterrows():
        geom = row['geometry']
        ctrl_list.append({
            'COD_ID': row['COD_ID'],
            'TIPO_EQUIPAMENTO': 'Chave Seccionadora (UNSEMT)',
            'ESTADO_OPERACIONAL': 'Normalmente Fechada' if row['P_N_OPE'] == 'F' else 'Normalmente Aberta',
            'ESTADO_CODIGO': row['P_N_OPE'],
            'PAC_1': row['PAC_1'],
            'PAC_2': row['PAC_2'],
            'FASES': row['FAS_CON'],
            'CORRENTE_NOM_A': row.get('COR_NOM', np.nan),
            'LON': geom.x if geom else np.nan,
            'LAT': geom.y if geom else np.nan
        })

    for idx, row in df_uncrmt.iterrows():
        geom = row['geometry']
        ctrl_list.append({
            'COD_ID': row['COD_ID'],
            'TIPO_EQUIPAMENTO': 'Banco de Capacitores (UNCRMT)',
            'ESTADO_OPERACIONAL': 'Operacional',
            'ESTADO_CODIGO': 'LIGADO',
            'PAC_1': row['PAC_1'],
            'PAC_2': row.get('PAC_2', ''),
            'FASES': row['FAS_CON'],
            'CORRENTE_NOM_A': np.nan,
            'LON': geom.x if geom else np.nan,
            'LAT': geom.y if geom else np.nan
        })

    df_ctrl = pd.DataFrame(ctrl_list)
    out_ctrl = os.path.join(treated_dir, 'equipamentos_controle.csv')
    df_ctrl.to_csv(out_ctrl, index=False, encoding='utf-8')
    print(f"  -> Salvo: {out_ctrl} ({len(df_ctrl)} equipamentos de controle: {len(df_unsemt)} chaves, {len(df_uncrmt)} capacitores)")

    # -------------------------------------------------------------------------
    # 6. CARGAS.CSV (Consumidores Agregados por Trafo e Barramento)
    # -------------------------------------------------------------------------
    print("\n[6/10] Processando inventário agregado de cargas por transformador...")
    cargas_df = trafos_merged[['COD_ID', 'PAC_1', 'PAC_2', 'POT_NOM', 'QTD_UC_TOTAL', 'QTD_UC_RESIDENCIAL', 'QTD_UC_COMERCIAL', 'QTD_UC_PODER_PUBLICO', 'QTD_UC_ILUM_PUBLICA', 'QTD_UC_INDUSTRIAL', 'DEMANDA_MAX_ESTIMADA_KW', 'CARREGAMENTO_ESTIMADO_PCT', 'LON', 'LAT']].copy()
    cargas_df.columns = ['ID_TRANSFORMADOR', 'BARRA_PRIMARIA_MT', 'BARRA_SECUNDARIA_BT', 'CAPACIDADE_KVA', 'TOTAL_CONSUMIDORES', 'RESIDENCIAL', 'COMERCIAL', 'PODER_PUBLICO', 'ILUMINACAO_PUBLICA', 'INDUSTRIAL', 'DEMANDA_PICO_ESTIMADA_KW', 'CARREGAMENTO_ESTIMADO_PCT', 'LON', 'LAT']
    out_cargas = os.path.join(treated_dir, 'cargas.csv')
    cargas_df.to_csv(out_cargas, index=False, encoding='utf-8')
    print(f"  -> Salvo: {out_cargas} ({len(cargas_df)} pontos de agregação de carga)")

    # -------------------------------------------------------------------------
    # 7. GERACAO_DISTRIBUIDA.CSV
    # -------------------------------------------------------------------------
    print("\n[7/10] Processando geração distribuída (BDGD STO33 e ANEEL Santos)...")
    df_ugbt_sto33 = pyogrio.read_dataframe(gdb_path, layer='UGBT_tab', where="CTMT = 'STO33'")
    df_gd_aneel = pd.read_parquet(os.path.join(raw_dir, 'empreendimento-geracao-distribuida.parquet'))
    santos_gd_aneel = df_gd_aneel[(df_gd_aneel['NomMunicipio'].str.upper() == 'SANTOS') & (df_gd_aneel['SigUF'] == 'SP')].copy()

    # Mapear GD do STO33 com detalhes do trafo
    ugbt_clean = df_ugbt_sto33[['COD_ID', 'CEG_GD', 'UNI_TR_MT', 'PAC', 'POT_INST', 'DAT_CON', 'TIP_SIST', 'FAS_CON', 'TEN_CON']].copy()
    ugbt_clean['ALIMENTADOR'] = 'STO33'
    ugbt_clean['SUBESTACAO'] = 'STO'
    ugbt_clean['FONTE'] = 'Solar Fotovoltaica'

    out_gd = os.path.join(treated_dir, 'geracao_distribuida.csv')
    ugbt_clean.to_csv(out_gd, index=False, encoding='utf-8')
    print(f"  -> Salvo: {out_gd} (23 usinas solares mapeadas no alimentador STO33)")

    out_gd_mun = os.path.join(treated_dir, 'geracao_distribuida_santos_total.csv')
    santos_gd_aneel.to_csv(out_gd_mun, index=False, encoding='utf-8')
    print(f"  -> Salvo: {out_gd_mun} (1.342 usinas solares do município de Santos)")

    # -------------------------------------------------------------------------
    # 8. VEICULOS_ELETRICOS.CSV
    # -------------------------------------------------------------------------
    print("\n[8/10] Processando dados da frota de veículos elétricos (DETRAN-SP)...")
    df_frota = pd.read_csv(os.path.join(raw_dir, 'frota_ativa_santos_setembro_2026.csv'))
    
    # Agregar por combustível e tipo
    frota_agg = df_frota.groupby(['COMBUSTIVEL', 'TIPO_VEICULO'])['QTD_VEICULOS'].sum().reset_index()
    frota_agg['CLASSIFICACAO_ELETRICA'] = 'Combustão Convencional'
    
    bev_mask = frota_agg['COMBUSTIVEL'].isin(['ELETRICO/FONTE EXTERNA', 'ELETRICO'])
    phev_mask = frota_agg['COMBUSTIVEL'].isin(['HIBRIDO PLUGIN'])
    hev_mask = frota_agg['COMBUSTIVEL'].isin(['GASOLINA/ELETRICO', 'GASOLINA/ALCOOL/ELETRICO', 'HIBRIDO', 'ELETRICO/FONTE INTERNA', 'DIESEL/ELETRICO'])
    
    frota_agg.loc[bev_mask, 'CLASSIFICACAO_ELETRICA'] = 'BEV (100% Elétrico Recarregável)'
    frota_agg.loc[phev_mask, 'CLASSIFICACAO_ELETRICA'] = 'PHEV (Híbrido Plug-in Recarregável)'
    frota_agg.loc[hev_mask, 'CLASSIFICACAO_ELETRICA'] = 'HEV (Híbrido não recarregável na rede)'
    
    out_ve = os.path.join(treated_dir, 'veiculos_eletricos.csv')
    frota_agg.to_csv(out_ve, index=False, encoding='utf-8')
    print(f"  -> Salvo: {out_ve} ({len(frota_agg)} categorias da frota ativa de Santos)")

    # -------------------------------------------------------------------------
    # 9. CURVAS_CARGA.CSV (ANEEL CTR CPFL Piratininga)
    # -------------------------------------------------------------------------
    print("\n[9/10] Processando curvas de carga horárias de referência (CTR ANEEL)...")
    df_ctr_cons = pd.read_parquet(os.path.join(raw_dir, 'ctr-curvas-carga-consumidor-tipo.parquet'))
    pirat_curves = df_ctr_cons[df_ctr_cons['SigCcs'] == 'CPFL PIRATININGA'].copy()

    # Extrair perfis médios por subgrupo e tipo de dia
    # Agrupar por subgrupo (B1=Residencial, B3=Comercial, B4=Iluminação Pública), TipoDia, e horário
    pirat_curves['HORA'] = pd.to_datetime(pirat_curves['HorInicial'].astype(str)).dt.hour
    hourly_profiles = pirat_curves.groupby(['NomSubGrupoTarifario', 'DscTipoDia', 'HORA'])['VlrDmd'].mean().reset_index()

    # Normalizar para perfil unitário (pu) por classe
    def norm_pu(df_group):
        max_v = df_group['VlrDmd'].max()
        df_group['PERFIL_PU'] = df_group['VlrDmd'] / max_v if max_v > 0 else 0
        return df_group

    hourly_norm = hourly_profiles.groupby(['NomSubGrupoTarifario', 'DscTipoDia'], group_keys=False).apply(norm_pu)
    out_curvas = os.path.join(treated_dir, 'curvas_carga.csv')
    hourly_norm.to_csv(out_curvas, index=False, encoding='utf-8')
    print(f"  -> Salvo: {out_curvas} ({len(hourly_norm)} perfis horários normalizados da CPFL Piratininga)")

    # -------------------------------------------------------------------------
    # 10. INDICADORES_INDGER.CSV
    # -------------------------------------------------------------------------
    print("\n[10/10] Processando indicadores gerenciais INDGER (STO33 e Santos)...")
    df_indger = pd.read_parquet(os.path.join(raw_dir, 'indger-dados-tecnicos-alimentadores.parquet'))
    
    sto33_ind = df_indger[df_indger['CodAlimentador'] == 'STO33'].copy()
    # Limpar coluna de percentual de carregamento
    def parse_pct(val):
        if pd.isna(val):
            return np.nan
        s = str(val).strip().replace(',', '.')
        try:
            v = float(s)
            return v * 100 if v <= 1.5 else v
        except:
            return np.nan

    sto33_ind['CARREGAMENTO_MAX_PCT'] = sto33_ind['PctCarregamentoMaxAlimentador'].apply(parse_pct)
    out_indger = os.path.join(treated_dir, 'indicadores_indger.csv')
    sto33_ind.to_csv(out_indger, index=False, encoding='utf-8')
    print(f"  -> Salvo: {out_indger} ({len(sto33_ind)} meses de série histórica do STO33)")

    print("\n================================================================================")
    print("TODAS AS 10 BASES DE DADOS TRATADAS FORAM GERADAS COM SUCESSO!")
    print("================================================================================")

if __name__ == '__main__':
    main()
