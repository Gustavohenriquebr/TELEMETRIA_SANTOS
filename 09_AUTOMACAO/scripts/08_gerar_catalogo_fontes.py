import os
import pandas as pd

def main():
    base_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))
    doc_dir = os.path.join(base_dir, '01_FONTES_E_DOCUMENTACAO')
    tech_dir = os.path.join(doc_dir, 'documentos_tecnicos')
    os.makedirs(tech_dir, exist_ok=True)

    fontes = [
        {
            'ID_FONTE': 'F01_ANEEL_BDGD_CPFL_PIRATININGA',
            'NOME_BASE': 'Base de Dados Geográfica da Distribuidora (BDGD) — CPFL Piratininga',
            'ORGAO_GESTOR': 'ANEEL / CPFL Piratininga',
            'URL_ENDPOINT': 'https://www.arcgis.com/sharing/rest/content/items/75863ef447904bbdacc1435e7f47d43a/data',
            'DATA_ACESSO': '2026-10-30',
            'DATA_REFERENCIA': '2025-12-31 (Versão V11 submetida em 2026)',
            'FORMATO_ORIGINAL': 'ESRI File Geodatabase (.gdb.zip, 419.3 MB, 43 camadas)',
            'CLASSIFICACAO_QUALIDADE': 'B — Registro oficial cadastral',
            'DESCRICAO_USO': 'Reconstrução topológica integral do alimentador STO33 (730 trechos MT, 105 trafos, 86 chaves, 2 capacitores, 6.499 clientes BT, 23 UGBs e parâmetros de condutores SEGCON).'
        },
        {
            'ID_FONTE': 'F02_ANEEL_INDGER_ALIMENTADORES',
            'NOME_BASE': 'INDGER — Indicadores Gerenciais da Distribuição (Dados Técnicos de Alimentadores)',
            'ORGAO_GESTOR': 'SFT / ANEEL',
            'URL_ENDPOINT': 'https://dadosabertos.aneel.gov.br/dataset/7cacb2c4-b165-4591-a793-9ed20d1f167d/resource/8c05a6d2-c6d4-48d6-9cc9-f805652eea2e/download/indger-dados-tecnicos-alimentadores.parquet',
            'DATA_ACESSO': '2026-10-30',
            'DATA_REFERENCIA': 'Série mensal 2023 a 2026 (Atualizado até 19/10/2026)',
            'FORMATO_ORIGINAL': 'Apache Parquet (15.55 MB, 1.455.364 registros)',
            'CLASSIFICACAO_QUALIDADE': 'C — Indicador oficial agregado',
            'DESCRICAO_USO': 'Identificação do carregamento máximo do alimentador STO33 (PctCarregamentoMaxAlimentador = 91.04% a 94.00%), consumidores ativos e histórico de inspeções/manutenções.'
        },
        {
            'ID_FONTE': 'F03_ANEEL_INDGER_COMERCIAIS',
            'NOME_BASE': 'INDGER — Indicadores Gerenciais da Distribuição (Dados Comerciais e Faturamento)',
            'ORGAO_GESTOR': 'SFT / ANEEL',
            'URL_ENDPOINT': 'https://dadosabertos.aneel.gov.br/dataset/7cacb2c4-b165-4591-a793-9ed20d1f167d/resource/92492adf-9944-4702-becf-1340ee68859d/download/indger-dados-comerciais.parquet',
            'DATA_ACESSO': '2026-10-30',
            'DATA_REFERENCIA': '2023 a 2026',
            'FORMATO_ORIGINAL': 'Apache Parquet (13.78 MB)',
            'CLASSIFICACAO_QUALIDADE': 'C — Indicador oficial agregado',
            'DESCRICAO_USO': 'Indicadores de faturamento, atendimento comercial e ocorrências de danos elétricos na área de concessão de Santos.'
        },
        {
            'ID_FONTE': 'F04_ANEEL_GERACAO_DISTRIBUIDA',
            'NOME_BASE': 'Relação de Empreendimentos de Mini e Microgeração Distribuída (MMGD)',
            'ORGAO_GESTOR': 'STD / CEGDI / ANEEL',
            'URL_ENDPOINT': 'https://dadosabertos.aneel.gov.br/dataset/5e0fafd2-21b9-4d5b-b622-40438d40aba2/resource/cd29f6eb-e08d-4db7-b6fb-ed6e3b682d27/download/empreendimento-geracao-distribuida.parquet',
            'DATA_ACESSO': '2026-10-30',
            'DATA_REFERENCIA': 'Diária (Atualizado até 29/10/2026)',
            'FORMATO_ORIGINAL': 'Apache Parquet (102.18 MB, 4.696.988 registros nacionais)',
            'CLASSIFICACAO_QUALIDADE': 'B — Registro oficial cadastral',
            'DESCRICAO_USO': 'Mapeamento de todas as 1.342 usinas solares do município de Santos (16.89 MWp total) e cruzamento com os 23 geradores do STO33.'
        },
        {
            'ID_FONTE': 'F05_ANEEL_CTR_CURVAS_CARGA',
            'NOME_BASE': 'CTR — Curvas de Carga de Redes e Consumidores Tipo (Campanha de Medidas)',
            'ORGAO_GESTOR': 'STR / CEGDI / ANEEL',
            'URL_ENDPOINT': 'https://dadosabertos.aneel.gov.br/dataset/2594ebad-1306-49d1-9f30-ded144acca87/resource/fae0d52c-5f02-4e84-a11b-d03db87e990d/download/ctr-curvas-carga-consumidor-tipo.parquet',
            'DATA_ACESSO': '2026-10-30',
            'DATA_REFERENCIA': 'Revisões Tarifárias Periódicas (RTP)',
            'FORMATO_ORIGINAL': 'Apache Parquet (32.19 MB)',
            'CLASSIFICACAO_QUALIDADE': 'D — Dado oficial de referência ou proxy',
            'DESCRICAO_USO': 'Curvas de demanda horária de 24 horas normalizadas para consumidores B1 (Residencial), B3 (Comercial) e B4 (Iluminação Pública) da CPFL Piratininga.'
        },
        {
            'ID_FONTE': 'F06_DETRAN_SP_FROTA_ATIVA',
            'NOME_BASE': 'Detran-SP — Frota Ativa de Veículos do Estado de São Paulo por Combustível',
            'ORGAO_GESTOR': 'DETRAN-SP / Governo do Estado de São Paulo',
            'URL_ENDPOINT': 'https://dadosabertos.sp.gov.br/dataset/frota-ativa',
            'DATA_ACESSO': '2026-10-30',
            'DATA_REFERENCIA': 'Setembro de 2026 (base mensal oficial)',
            'FORMATO_ORIGINAL': 'CSV delimitado por vírgula (filtrado para 20.752 registros de Santos)',
            'CLASSIFICACAO_QUALIDADE': 'B — Registro oficial cadastral',
            'DESCRICAO_USO': 'Levantamento oficial da frota total de Santos (218.471 veículos), frota eletrificada (4.593) e veículos recarregáveis na rede (1.101 plug-in BEV/PHEV).'
        },
        {
            'ID_FONTE': 'F07_IBGE_CIDADES_SANTOS',
            'NOME_BASE': 'IBGE — Panorama Municipal e Censo Demográfico 2022 (Santos/SP - 3548500)',
            'ORGAO_GESTOR': 'Instituto Brasileiro de Geografia e Estatística (IBGE)',
            'URL_ENDPOINT': 'https://servicodados.ibge.gov.br/api/v1/localidades/municipios/3548500',
            'DATA_ACESSO': '2026-10-30',
            'DATA_REFERENCIA': 'Censo Demográfico 2022 / Estimativas 2024-2026',
            'FORMATO_ORIGINAL': 'JSON REST API',
            'CLASSIFICACAO_QUALIDADE': 'B — Registro oficial cadastral',
            'DESCRICAO_USO': 'População oficial (418.608 habitantes), área territorial (281.03 km²), densidade demográfica e caracterização socioeconômica de Santos.'
        },
        {
            'ID_FONTE': 'F08_INPE_ATLAS_SOLAR',
            'NOME_BASE': 'INPE — Atlas Brasileiro de Energia Solar (2ª Edição)',
            'ORGAO_GESTOR': 'Instituto Nacional de Pesquisas Espaciais (INPE) / LABREN',
            'URL_ENDPOINT': 'http://labren.ccst.inpe.br/atlas_2017.html',
            'DATA_ACESSO': '2026-10-30',
            'DATA_REFERENCIA': 'Climatologia solarimétrica anual',
            'FORMATO_ORIGINAL': 'Modelagem numérica / Mapas de Irradiação',
            'CLASSIFICACAO_QUALIDADE': 'D — Dado oficial de referência ou proxy',
            'DESCRICAO_USO': 'Irradiação solar global horizontal média diária para Santos/SP (4.35 kWh/m²/dia) para dimensionamento de curva de geração PV.'
        },
        {
            'ID_FONTE': 'F09_PRODIST_MODULO_10',
            'NOME_BASE': 'PRODIST Módulo 10 — Sistema de Informação Geográfica Regulatório (SIG-R)',
            'ORGAO_GESTOR': 'ANEEL / Resolução Normativa nº 956/2021',
            'URL_ENDPOINT': 'https://www.gov.br/aneel/pt-br/centrais-de-conteudo/manuais-e-guias/prodist',
            'DATA_ACESSO': '2026-10-30',
            'DATA_REFERENCIA': 'Vigência 2021-2026',
            'FORMATO_ORIGINAL': 'Norma Regulatória / PDF',
            'CLASSIFICACAO_QUALIDADE': 'B — Registro oficial cadastral',
            'DESCRICAO_USO': 'Interpretação e validação do modelo de dados da BDGD (tabelas SSDMT, UNTRMT, UNREMT, UNSEMT, SEGCON, CRVCRG, etc.).'
        },
        {
            'ID_FONTE': 'F10_PRODIST_MODULO_8',
            'NOME_BASE': 'PRODIST Módulo 8 — Qualidade da Energia Elétrica',
            'ORGAO_GESTOR': 'ANEEL / Resolução Normativa nº 956/2021',
            'URL_ENDPOINT': 'https://www.gov.br/aneel/pt-br/centrais-de-conteudo/manuais-e-guias/prodist',
            'DATA_ACESSO': '2026-10-30',
            'DATA_REFERENCIA': 'Vigência 2021-2026',
            'FORMATO_ORIGINAL': 'Norma Regulatória / PDF',
            'CLASSIFICACAO_QUALIDADE': 'B — Registro oficial cadastral',
            'DESCRICAO_USO': 'Limites de conformidade de tensão permanente em regime de distribuição (Faixa Adequada: 0.93 a 1.05 pu) e limites de distorção harmônica.'
        }
    ]

    df_fontes = pd.DataFrame(fontes)
    out_cat = os.path.join(doc_dir, 'catalogo_fontes.csv')
    df_fontes.to_csv(out_cat, index=False, encoding='utf-8')
    print(f"Catalogo de fontes salvo em {out_cat} ({len(df_fontes)} fontes oficiais catalogadas).")

    # Documento de normas tecnicas
    normas_txt = """# NORMAS TÉCNICAS E REGULATÓRIAS APLICADAS
1. ANEEL Resolução Normativa nº 1.000/2021 — Regras de Prestação do Serviço Público de Distribuição de Energia Elétrica.
2. ANEEL PRODIST Módulo 8 — Qualidade da Energia Elétrica (Tensão em Regime Permanente, Desequilíbrio, Flutuação e Harmônicos).
3. ANEEL PRODIST Módulo 10 — Sistema de Informação Geográfica Regulatório (SIG-R / BDGD).
4. ANEEL PRORET Módulo 7 — Estrutura Tarifária e Metodologia da Campanha de Medidas (CTR).
5. ABNT NBR 5410 / NBR 14039 — Instalações Elétricas de Baixa e Média Tensão.
6. IEEE Std 1547-2018 — Standard for Interconnection and Interoperability of Distributed Energy Resources with Associated Electric Power Systems Interfaces.
7. IEEE Std 519-2022 — Standard for Harmonic Control in Electric Power Systems.
"""
    with open(os.path.join(tech_dir, 'normas_regulatorias.md'), 'w', encoding='utf-8') as f:
        f.write(normas_txt)

if __name__ == '__main__':
    main()
