import os
import sys
import json
import numpy as np
import networkx as nx
import pandas as pd
import folium
from folium import plugins
import pyogrio

def main():
    base_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))
    treated_dir = os.path.join(base_dir, '03_DADOS_TRATADOS')
    raw_dir = os.path.join(base_dir, '02_DADOS_BRUTOS')
    topo_dir = os.path.join(base_dir, '04_TOPOLOGIA')
    os.makedirs(topo_dir, exist_ok=True)

    print("================================================================================")
    print("CONSTRUINDO TOPOLOGIA E MAPAS GEOESPACIAIS — ALIMENTADOR STO33 (SANTOS/SP)")
    print("================================================================================")

    # Carregar dados tratados
    df_seg = pd.read_csv(os.path.join(treated_dir, 'segmentos_rede.csv'))
    df_trafos = pd.read_csv(os.path.join(treated_dir, 'transformadores.csv'))
    df_ctrl = pd.read_csv(os.path.join(treated_dir, 'equipamentos_controle.csv'))
    df_gd = pd.read_csv(os.path.join(treated_dir, 'geracao_distribuida.csv'))
    df_nodes = pd.read_csv(os.path.join(treated_dir, 'nos_rede.csv'))

    # Subestação STO coordenadas (-46.383, -23.926)
    sub_coords = [-23.926, -46.383]

    # -------------------------------------------------------------------------
    # 1. TOPOLOGIA_STO33.GRAPHML
    # -------------------------------------------------------------------------
    print("\n[1/4] Gerando topologia em grafo NetworkX / GraphML...")
    G = nx.Graph()

    # Adicionar nós com atributos
    for idx, row in df_nodes.iterrows():
        G.add_node(
            str(row['BUS_ID']),
            lat=float(row['LAT']) if pd.notna(row['LAT']) else 0.0,
            lon=float(row['LON']) if pd.notna(row['LON']) else 0.0,
            tipo=str(row['TIPO']),
            tensao_kv=float(row['TENSAO_KV'])
        )

    # Adicionar arestas dos trechos de linha
    for idx, row in df_seg.iterrows():
        u = str(row['PAC_1'])
        v = str(row['PAC_2'])
        comp = float(row['COMP']) if pd.notna(row['COMP']) else 0.0
        r1 = float(row['R1_OHM_KM']) if pd.notna(row['R1_OHM_KM']) else 0.0
        x1 = float(row['X1_OHM_KM']) if pd.notna(row['X1_OHM_KM']) else 0.0
        cmax = float(row['AMPACIDADE_MAX_A']) if pd.notna(row['AMPACIDADE_MAX_A']) else 0.0
        G.add_edge(
            u, v,
            tipo='Linha MT',
            cod_id=str(row['COD_ID']),
            comprimento_m=comp,
            condutor=str(row['TIP_CND']),
            fases=str(row['FAS_CON']),
            r1_ohm_km=r1,
            x1_ohm_km=x1,
            ampacidade_a=cmax
        )

    # Adicionar arestas das chaves seccionadoras fechadas
    sw_closed = df_ctrl[df_ctrl['TIPO_EQUIPAMENTO'].str.contains('Chave', na=False) & (df_ctrl['ESTADO_CODIGO'] == 'F')]
    for idx, row in sw_closed.iterrows():
        u = str(row['PAC_1'])
        v = str(row['PAC_2'])
        G.add_edge(
            u, v,
            tipo='Chave Seccionadora',
            cod_id=str(row['COD_ID']),
            comprimento_m=0.1,
            condutor='Chave Fechada',
            fases=str(row['FASES']),
            r1_ohm_km=0.0001,
            x1_ohm_km=0.0001,
            ampacidade_a=600.0
        )

    graphml_path = os.path.join(topo_dir, 'topologia_sto33.graphml')
    nx.write_graphml(G, graphml_path)
    print(f"  -> Salvo: {graphml_path} ({G.number_of_nodes()} nós, {G.number_of_edges()} arestas)")

    # -------------------------------------------------------------------------
    # 2. INVENTARIO_TOPOLOGICO.CSV (Distâncias elétricas e ramos a partir do PAC_INI)
    # -------------------------------------------------------------------------
    print("\n[2/4] Calculando distâncias elétricas e inventário topológico...")
    pac_ini = 'MT_STO33_346'
    
    # Calcular distâncias pelo caminho mais curto em metros
    dist_from_sub = {}
    if pac_ini in G:
        try:
            lengths = nx.single_source_dijkstra_path_length(G, pac_ini, weight='comprimento_m')
            dist_from_sub = lengths
        except Exception as e:
            print("  [Aviso] Falha no cálculo Dijkstra ponderado:", e)

    topo_records = []
    for idx, row in df_trafos.iterrows():
        p1 = str(row['PAC_1'])
        dist_m = dist_from_sub.get(p1, np.nan)
        topo_records.append({
            'TIPO_ELEMENTO': 'Transformador MT/BT',
            'COD_ID': row['COD_ID'],
            'PAC_CONEXAO': p1,
            'POTENCIA_KVA': row['POT_NOM'],
            'TOTAL_CONSUMIDORES': row['QTD_UC_TOTAL'],
            'DEMANDA_MAX_EST_KW': row['DEMANDA_MAX_ESTIMADA_KW'],
            'CARREGAMENTO_PCT': row['CARREGAMENTO_ESTIMADO_PCT'],
            'DISTANCIA_SUBESTACAO_KM': round(dist_m / 1000.0, 3) if pd.notna(dist_m) else np.nan,
            'LON': row['LON'],
            'LAT': row['LAT']
        })

    total_line_km = df_seg['COMP'].sum() / 1000.0
    df_inv = pd.DataFrame(topo_records)
    inv_path = os.path.join(topo_dir, 'inventario_topologico.csv')
    df_inv.to_csv(inv_path, index=False, encoding='utf-8')
    print(f"  -> Salvo: {inv_path} (Extensão total da rede MT: {total_line_km:.2f} km)")

    # -------------------------------------------------------------------------
    # 3. TOPOLOGIA_STO33.GEOJSON (Linhas e Pontos)
    # -------------------------------------------------------------------------
    print("\n[3/4] Exportando modelo completo em formato GeoJSON...")
    geojson_features = []

    # Features das linhas MT
    for idx, row in df_seg.iterrows():
        x1, y1 = row['COORD_X1'], row['COORD_Y1']
        x2, y2 = row['COORD_X2'], row['COORD_Y2']
        if pd.notna(x1) and pd.notna(y1) and pd.notna(x2) and pd.notna(y2):
            feat = {
                "type": "Feature",
                "geometry": {
                    "type": "LineString",
                    "coordinates": [[x1, y1], [x2, y2]]
                },
                "properties": {
                    "tipo": "Trecho MT",
                    "cod_id": str(row['COD_ID']),
                    "pac_1": str(row['PAC_1']),
                    "pac_2": str(row['PAC_2']),
                    "comprimento_m": float(row['COMP']),
                    "condutor": str(row['TIP_CND']),
                    "fases": str(row['FAS_CON']),
                    "material": str(row.get('MATERIAL', '')),
                    "r1_ohm_km": float(row.get('R1_OHM_KM', 0)),
                    "x1_ohm_km": float(row.get('X1_OHM_KM', 0)),
                    "ampacidade_a": float(row.get('AMPACIDADE_MAX_A', 0))
                }
            }
            geojson_features.append(feat)

    # Features dos transformadores
    for idx, row in df_trafos.iterrows():
        lon, lat = row['LON'], row['LAT']
        if pd.notna(lon) and pd.notna(lat):
            feat = {
                "type": "Feature",
                "geometry": {
                    "type": "Point",
                    "coordinates": [lon, lat]
                },
                "properties": {
                    "tipo": "Transformador MT/BT",
                    "cod_id": str(row['COD_ID']),
                    "pac_primario": str(row['PAC_1']),
                    "pac_secundario": str(row['PAC_2']),
                    "potencia_kva": float(row['POT_NOM']),
                    "clientes_total": int(row['QTD_UC_TOTAL']),
                    "clientes_res": int(row['QTD_UC_RESIDENCIAL']),
                    "clientes_com": int(row['QTD_UC_COMERCIAL']),
                    "demanda_est_kw": float(row['DEMANDA_MAX_ESTIMADA_KW']),
                    "carregamento_pct": float(row['CARREGAMENTO_ESTIMADO_PCT'])
                }
            }
            geojson_features.append(feat)

    # Features dos equipamentos de manobra (chaves)
    for idx, row in df_ctrl.iterrows():
        lon, lat = row['LON'], row['LAT']
        if pd.notna(lon) and pd.notna(lat):
            feat = {
                "type": "Feature",
                "geometry": {
                    "type": "Point",
                    "coordinates": [lon, lat]
                },
                "properties": {
                    "tipo": str(row['TIPO_EQUIPAMENTO']),
                    "cod_id": str(row['COD_ID']),
                    "pac_1": str(row['PAC_1']),
                    "pac_2": str(row['PAC_2']),
                    "estado": str(row['ESTADO_OPERACIONAL']),
                    "fases": str(row['FASES'])
                }
            }
            geojson_features.append(feat)

    # Subestação STO
    geojson_features.append({
        "type": "Feature",
        "geometry": {
            "type": "Point",
            "coordinates": [sub_coords[1], sub_coords[0]]
        },
        "properties": {
            "tipo": "Subestação STO (Santos 1)",
            "cod_id": "STO",
            "tensao_at_mt": "88/13.8 kV",
            "transformador_potencia": "UNI_TR_AT 56622 (20 MVA)"
        }
    })

    geojson_collection = {
        "type": "FeatureCollection",
        "name": "Topologia_Eletrica_STO33_Santos",
        "features": geojson_features
    }

    geojson_path = os.path.join(topo_dir, 'topologia_sto33.geojson')
    with open(geojson_path, 'w', encoding='utf-8') as f_geo:
        json.dump(geojson_collection, f_geo, indent=2, ensure_ascii=False)
    print(f"  -> Salvo: {geojson_path} ({len(geojson_features)} elementos geoespaciais)")

    # -------------------------------------------------------------------------
    # 4. MAPA_STO33.HTML (Mapa Interativo Leaflet / Folium)
    # -------------------------------------------------------------------------
    print("\n[4/4] Gerando mapa interativo georreferenciado da rede STO33...")
    # Calcular centro do alimentador
    valid_lats = df_trafos['LAT'].dropna()
    valid_lons = df_trafos['LON'].dropna()
    center_lat = valid_lats.mean() if len(valid_lats) > 0 else -23.93
    center_lon = valid_lons.mean() if len(valid_lons) > 0 else -46.42

    m = folium.Map(
        location=[center_lat, center_lon],
        zoom_start=14,
        tiles='OpenStreetMap',
        control_scale=True
    )

    folium.TileLayer('OpenStreetMap', name='OpenStreetMap').add_to(m)
    folium.TileLayer(
        tiles='https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}',
        attr='Esri Satellite',
        name='Satélite (Esri World Imagery)'
    ).add_to(m)

    # Grupo 1: Linhas de Média Tensão
    fg_lines = folium.FeatureGroup(name='Rede MT (730 trechos - 13.8 kV)', show=True)
    for idx, row in df_seg.iterrows():
        y1, x1 = row['COORD_Y1'], row['COORD_X1']
        y2, x2 = row['COORD_Y2'], row['COORD_X2']
        if pd.notna(y1) and pd.notna(x1) and pd.notna(y2) and pd.notna(x2):
            amp = row.get('AMPACIDADE_MAX_A', 300)
            # Cor baseada na capacidade térmica
            color = '#d95f02' if amp >= 500 else ('#7570b3' if amp >= 350 else '#1b9e77')
            weight = 4 if amp >= 500 else 2.5
            
            popup_html = f"""
            <div style="font-family: Arial; font-size: 11px; width: 220px;">
                <b style="color: #2b5c8f;">TRECHO MT: {row['COD_ID']}</b><br>
                <b>Conexão:</b> {row['PAC_1']} &rarr; {row['PAC_2']}<br>
                <b>Comprimento:</b> {row['COMP']:.1f} m<br>
                <b>Condutor:</b> {row['TIP_CND']} ({row.get('MATERIAL', 'Alumínio')})<br>
                <b>Fases:</b> {row['FAS_CON']}<br>
                <b>Impedância:</b> R={row.get('R1_OHM_KM', 0):.4f} &Omega;/km, X={row.get('X1_OHM_KM', 0):.4f} &Omega;/km<br>
                <b>Ampacidade Máx:</b> {row.get('AMPACIDADE_MAX_A', 0)} A
            </div>
            """
            folium.PolyLine(
                locations=[[y1, x1], [y2, x2]],
                color=color,
                weight=weight,
                opacity=0.85,
                tooltip=f"Trecho {row['COD_ID']} ({row['COMP']:.1f}m - {row['TIP_CND']})",
                popup=folium.Popup(popup_html, max_width=250)
            ).add_to(fg_lines)
    fg_lines.add_to(m)

    # Grupo 2: Transformadores MT/BT
    fg_trafos = folium.FeatureGroup(name='Transformadores MT/BT (105 trafos)', show=True)
    for idx, row in df_trafos.iterrows():
        lat, lon = row['LAT'], row['LON']
        if pd.notna(lat) and pd.notna(lon):
            carr = row['CARREGAMENTO_ESTIMADO_PCT']
            # Cor pelo carregamento
            if carr >= 100:
                color = '#e41a1c' # Vermelho (sobrecarga)
                fill_color = '#ff7f00'
            elif carr >= 70:
                color = '#ff7f00' # Laranja (carregamento elevado)
                fill_color = '#ffff33'
            else:
                color = '#377eb8' # Azul normal
                fill_color = '#4daf4a'

            radius = 5 + (row['POT_NOM'] / 50.0)
            popup_html = f"""
            <div style="font-family: Arial; font-size: 11px; width: 240px;">
                <b style="color: #003366;">TRANSFORMADOR: {row['COD_ID']}</b><br>
                <b>Potência Nominal:</b> {row['POT_NOM']} kVA<br>
                <b>Barra Primária MT:</b> {row['PAC_1']}<br>
                <b>Barra Secundária BT:</b> {row['PAC_2']}<br>
                <hr style="margin: 4px 0;">
                <b>Total de Consumidores:</b> {row['QTD_UC_TOTAL']}<br>
                &bull; Residencial: {row['QTD_UC_RESIDENCIAL']}<br>
                &bull; Comercial: {row['QTD_UC_COMERCIAL']}<br>
                &bull; Poder Público: {row['QTD_UC_PODER_PUBLICO']}<br>
                <hr style="margin: 4px 0;">
                <b>Demanda Pico Estimada:</b> {row['DEMANDA_MAX_ESTIMADA_KW']} kW<br>
                <b>Carregamento Estimado:</b> <b>{row['CARREGAMENTO_ESTIMADO_PCT']:.1f}%</b>
            </div>
            """
            folium.CircleMarker(
                location=[lat, lon],
                radius=radius,
                color=color,
                weight=2,
                fill=True,
                fill_color=fill_color,
                fill_opacity=0.85,
                tooltip=f"Trafo {row['COD_ID']} ({row['POT_NOM']} kVA, {row['QTD_UC_TOTAL']} UCs)",
                popup=folium.Popup(popup_html, max_width=260)
            ).add_to(fg_trafos)
    fg_trafos.add_to(m)

    # Grupo 3: Chaves e Equipamentos de Manobra
    fg_sw = folium.FeatureGroup(name='Chaves de Manobra (86 seccionadores)', show=False)
    for idx, row in df_ctrl[df_ctrl['TIPO_EQUIPAMENTO'].str.contains('Chave', na=False)].iterrows():
        lat, lon = row['LAT'], row['LON']
        if pd.notna(lat) and pd.notna(lon):
            is_closed = row['ESTADO_CODIGO'] == 'F'
            color = '#4daf4a' if is_closed else '#e41a1c'
            icon = 'lock' if is_closed else 'unlock'
            folium.CircleMarker(
                location=[lat, lon],
                radius=4,
                color=color,
                weight=2,
                fill=True,
                fill_color=color,
                fill_opacity=0.9,
                tooltip=f"Chave {row['COD_ID']} ({row['ESTADO_OPERACIONAL']})",
                popup=f"Chave: {row['COD_ID']}<br>Estado: {row['ESTADO_OPERACIONAL']}<br>PACs: {row['PAC_1']} - {row['PAC_2']}"
            ).add_to(fg_sw)
    fg_sw.add_to(m)

    # Grupo 4: Geração Solar Fotovoltaica
    fg_pv = folium.FeatureGroup(name='Geração Solar FV (23 usinas STO33)', show=True)
    # Join GD with trafo locations
    trafo_locs = dict(zip(df_trafos['COD_ID'].astype(str), zip(df_trafos['LAT'], df_trafos['LON'])))
    for idx, row in df_gd.iterrows():
        tr_id = str(row['UNI_TR_MT'])
        if tr_id in trafo_locs:
            lat, lon = trafo_locs[tr_id]
            folium.Marker(
                location=[lat, lon],
                icon=folium.Icon(color='orange', icon='sun', prefix='fa'),
                tooltip=f"Solar FV {row['COD_ID']}: {row['POT_INST']:.2f} kW",
                popup=f"<b>UFV {row['COD_ID']}</b><br>Potência: {row['POT_INST']:.2f} kW<br>Data Conexão: {row['DAT_CON']}<br>Trafo: {tr_id}"
            ).add_to(fg_pv)
    fg_pv.add_to(m)

    # Grupo 5: Subestação STO
    fg_sub = folium.FeatureGroup(name='Subestação STO (Santos 1 - Origem)', show=True)
    folium.Marker(
        location=sub_coords,
        icon=folium.Icon(color='red', icon='bolt', prefix='fa'),
        tooltip="Subestação STO (Santos 1) — 88/13.8 kV, 20 MVA",
        popup="<b>Subestação STO (Santos 1)</b><br>Tensão Primária: 88 kV<br>Barramento MT: STO569794 (13.8 kV)<br>Trafo ETR102 (20 MVA)<br>Alimenta STO31 a STO38"
    ).add_to(fg_sub)
    fg_sub.add_to(m)

    # Controle de camadas e tela cheia
    folium.LayerControl(collapsed=False).add_to(m)
    plugins.Fullscreen(position='topright').add_to(m)
    plugins.MiniMap(toggle_display=True).add_to(m)

    map_path = os.path.join(topo_dir, 'mapa_sto33.html')
    m.save(map_path)
    print(f"  -> Salvo: {map_path} (Mapa interativo completo com camadas)")

    print("\n================================================================================")
    print("TODOS OS 4 ARQUIVOS TOPOLÓGICOS FORAM GERADOS COM SUCESSO!")
    print("================================================================================")

if __name__ == '__main__':
    main()
