import os
import sys
import time
import requests

def main():
    url = 'https://0010068detrandadoabprdsa.blob.core.windows.net/detran-public/dados-abertos/frota/Sep-2026/30/frota_ativa_sp.csv?sp=r&st=2026-05-15T16:49:14Z&se=2027-05-15T03:00:00Z&spr=https&sv=2025-11-05&sr=c&sig=ylRiD8xZxlulYOcxzK8g8a0NFu%2BI5zBi5Acp13JrL%2B4%3D'
    base_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))
    out_file = os.path.join(base_dir, '02_DADOS_BRUTOS', 'frota_ativa_santos_setembro_2026.csv')

    print(f"Baixando e filtrando frota de Santos de {url}...")
    t0 = time.time()
    r = requests.get(url, stream=True, timeout=120)
    r.raise_for_status()

    total_bytes = int(r.headers.get('content-length', 0))
    read_bytes = 0
    santos_rows = 0

    with open(out_file, 'w', encoding='utf-8') as f_out:
        line_buffer = ""
        header_written = False
        for chunk in r.iter_content(chunk_size=1024*1024*4):
            if chunk:
                read_bytes += len(chunk)
                text = chunk.decode('latin1', errors='ignore')
                line_buffer += text
                lines = line_buffer.split('\n')
                line_buffer = lines[-1] # keep remainder

                for line in lines[:-1]:
                    if not header_written:
                        f_out.write(line + '\n')
                        header_written = True
                        continue
                    parts = line.split(',')
                    if len(parts) > 1 and parts[1].strip().upper() == 'SANTOS':
                        f_out.write(line + '\n')
                        santos_rows += 1

                mb = read_bytes / (1024*1024)
                pct = (read_bytes / total_bytes * 100) if total_bytes else 0
                sys.stdout.write(f"\rProcessado {mb:.1f} MB ({pct:.1f}%) | Linhas Santos: {santos_rows} | {time.time()-t0:.1f}s")
                sys.stdout.flush()

        # Handle last line if any
        if line_buffer:
            parts = line_buffer.split(',')
            if len(parts) > 1 and parts[1].strip().upper() == 'SANTOS':
                f_out.write(line_buffer + '\n')
                santos_rows += 1

    print(f"\n[SUCESSO] Arquivo gerado: {out_file}")
    print(f"Total de registros de Santos salvos: {santos_rows}")

if __name__ == '__main__':
    main()
