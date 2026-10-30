import os
import sys
import time
import ssl
import requests
from requests.adapters import HTTPAdapter

class TLS12Adapter(HTTPAdapter):
    def init_poolmanager(self, *args, **kwargs):
        ctx = ssl.SSLContext(ssl.PROTOCOL_TLSv1_2)
        kwargs['ssl_context'] = ctx
        return super().init_poolmanager(*args, **kwargs)

def get_session():
    s = requests.Session()
    s.mount('https://', TLS12Adapter())
    s.headers.update({
        'User-Agent': 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36'
    })
    return s

def download_file(session, url, target_path, desc):
    if os.path.exists(target_path) and os.path.getsize(target_path) > 1000:
        print(f"[JA EXISTE] {desc}: {target_path} ({os.path.getsize(target_path)/(1024*1024):.2f} MB)")
        return True

    print(f"[BAIXANDO] {desc}...")
    t0 = time.time()
    try:
        r = session.get(url, stream=True, timeout=60)
        r.raise_for_status()
        total_size = int(r.headers.get('content-length', 0))
        downloaded = 0
        os.makedirs(os.path.dirname(target_path), exist_ok=True)
        with open(target_path, 'wb') as f:
            for chunk in r.iter_content(chunk_size=1024*1024):
                if chunk:
                    f.write(chunk)
                    downloaded += len(chunk)
                    mb = downloaded / (1024*1024)
                    pct = (downloaded / total_size * 100) if total_size else 0
                    sys.stdout.write(f"\r  -> {mb:.1f} MB ({pct:.1f}%) em {time.time()-t0:.1f}s")
                    sys.stdout.flush()
        dt = time.time() - t0
        print(f"\n  [OK] Concluído: {os.path.getsize(target_path)/(1024*1024):.2f} MB em {dt:.1f}s")
        return True
    except Exception as e:
        print(f"\n  [ERRO] Falha ao baixar {desc}: {e}")
        return False

def main():
    session = get_session()
    base_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))
    raw_dir = os.path.join(base_dir, '02_DADOS_BRUTOS')
    doc_dir = os.path.join(base_dir, '01_FONTES_E_DOCUMENTACAO')
    dict_dir = os.path.join(doc_dir, 'dicionarios_de_dados')

    downloads = [
        # INDGER Parquet
        (
            'https://dadosabertos.aneel.gov.br/dataset/7cacb2c4-b165-4591-a793-9ed20d1f167d/resource/8c05a6d2-c6d4-48d6-9cc9-f805652eea2e/download/indger-dados-tecnicos-alimentadores.parquet',
            os.path.join(raw_dir, 'indger-dados-tecnicos-alimentadores.parquet'),
            'INDGER - Dados Técnicos de Alimentadores (Parquet)'
        ),
        (
            'https://dadosabertos.aneel.gov.br/dataset/7cacb2c4-b165-4591-a793-9ed20d1f167d/resource/92492adf-9944-4702-becf-1340ee68859d/download/indger-dados-comerciais.parquet',
            os.path.join(raw_dir, 'indger-dados-comerciais.parquet'),
            'INDGER - Dados Comerciais (Parquet)'
        ),
        # INDGER Dicionários de Dados (PDF)
        (
            'https://dadosabertos.aneel.gov.br/dataset/7cacb2c4-b165-4591-a793-9ed20d1f167d/resource/cce61404-687d-42ad-924d-90fb02a3daed/download/dm-indger-dados-tecnicos-de-alimentadores.pdf',
            os.path.join(dict_dir, 'dm-indger-dados-tecnicos-de-alimentadores.pdf'),
            'Dicionário - INDGER Dados Técnicos de Alimentadores'
        ),
        (
            'https://dadosabertos.aneel.gov.br/dataset/7cacb2c4-b165-4591-a793-9ed20d1f167d/resource/163de7c4-899e-423f-b7fa-c1bdf208df8c/download/dm-indger-dados-tecnicos-de-subestacoes.pdf',
            os.path.join(dict_dir, 'dm-indger-dados-tecnicos-de-subestacoes.pdf'),
            'Dicionário - INDGER Dados Técnicos de Subestações'
        ),
        (
            'https://dadosabertos.aneel.gov.br/dataset/7cacb2c4-b165-4591-a793-9ed20d1f167d/resource/756794aa-0828-4a3f-83f5-d36f863dd802/download/dm-indger-dados-comerciais.pdf',
            os.path.join(dict_dir, 'dm-indger-dados-comerciais.pdf'),
            'Dicionário - INDGER Dados Comerciais'
        ),
        (
            'https://dadosabertos.aneel.gov.br/dataset/7cacb2c4-b165-4591-a793-9ed20d1f167d/resource/86e54795-42be-4938-9cb3-b77797d43980/download/dm-indger-dados-de-servicos-comerciais.pdf',
            os.path.join(dict_dir, 'dm-indger-dados-de-servicos-comerciais.pdf'),
            'Dicionário - INDGER Dados de Serviços Comerciais'
        ),
        # Geração Distribuída
        (
            'https://dadosabertos.aneel.gov.br/dataset/5e0fafd2-21b9-4d5b-b622-40438d40aba2/resource/cd29f6eb-e08d-4db7-b6fb-ed6e3b682d27/download/empreendimento-geracao-distribuida.parquet',
            os.path.join(raw_dir, 'empreendimento-geracao-distribuida.parquet'),
            'ANEEL - Empreendimentos de Geração Distribuída (Parquet)'
        ),
        (
            'https://dadosabertos.aneel.gov.br/dataset/5e0fafd2-21b9-4d5b-b622-40438d40aba2/resource/3fabb9e8-668a-4f94-8f0e-ed9cd2682979/download/dm-geracao-distribuida-relacao-de-empreendimentos.pdf',
            os.path.join(dict_dir, 'dm-geracao-distribuida-relacao-de-empreendimentos.pdf'),
            'Dicionário - Geração Distribuída'
        ),
        # CTR - Curvas de Carga Redes Tipo
        (
            'https://dadosabertos.aneel.gov.br/dataset/2594ebad-1306-49d1-9f30-ded144acca87/resource/0fa851c4-3421-4cd1-ab9d-cddbce37d28a/download/ctr-curvas-carga-redes-tipo.parquet',
            os.path.join(raw_dir, 'ctr-curvas-carga-redes-tipo.parquet'),
            'CTR - Curvas de Carga Redes Tipo (Parquet)'
        ),
        (
            'https://dadosabertos.aneel.gov.br/dataset/2594ebad-1306-49d1-9f30-ded144acca87/resource/fae0d52c-5f02-4e84-a11b-d03db87e990d/download/ctr-curvas-carga-consumidor-tipo.parquet',
            os.path.join(raw_dir, 'ctr-curvas-carga-consumidor-tipo.parquet'),
            'CTR - Curvas de Carga Consumidor Tipo (Parquet)'
        ),
        (
            'https://dadosabertos.aneel.gov.br/dataset/2594ebad-1306-49d1-9f30-ded144acca87/resource/f6ebf857-44c7-42fd-a246-61c3fe4e447e/download/dm-ctr-curva-de-carga-redes-tipo.pdf',
            os.path.join(dict_dir, 'dm-ctr-curva-de-carga-redes-tipo.pdf'),
            'Dicionário - CTR Curva de Carga Redes Tipo'
        ),
    ]

    print(f"Iniciando download de {len(downloads)} recursos da ANEEL...")
    success = 0
    for url, path, desc in downloads:
        if download_file(session, url, path, desc):
            success += 1

    print(f"\nResumo: {success}/{len(downloads)} arquivos baixados/validados com sucesso.")

if __name__ == '__main__':
    main()
