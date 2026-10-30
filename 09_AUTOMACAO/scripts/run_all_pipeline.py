import os
import sys
import subprocess
import time

def run_script(script_path):
    print(f"\n>>> EXECUTANDO: {os.path.basename(script_path)}...")
    t0 = time.time()
    res = subprocess.run([sys.executable, script_path], capture_output=True, text=True)
    dt = time.time() - t0
    if res.returncode == 0:
        print(f"    [SUCESSO] Concluído em {dt:.2f}s")
        if res.stdout:
            for line in res.stdout.strip().splitlines()[-5:]:
                print(f"      {line}")
    else:
        print(f"    [ERRO] Código de saída {res.returncode}")
        print(res.stderr)
        return False
    return True

def main():
    scripts_dir = os.path.dirname(os.path.abspath(__file__))
    log_dir = os.path.abspath(os.path.join(scripts_dir, '..', 'logs'))
    os.makedirs(log_dir, exist_ok=True)

    pipeline = [
        os.path.join(scripts_dir, '01_download_dados_aneel.py'),
        os.path.join(scripts_dir, '02_download_frota_detran_santos.py'),
        os.path.join(scripts_dir, '03_processar_dados_tratados.py'),
        os.path.join(scripts_dir, '04_construir_topologia_mapas.py'),
        os.path.join(scripts_dir, '05_gerar_diagnostico_critico.py'),
        os.path.join(scripts_dir, '06_estruturar_telemetria.py'),
        os.path.join(scripts_dir, '07_construir_modelo_opendss.py'),
        os.path.join(scripts_dir, '08_gerar_catalogo_fontes.py'),
        os.path.join(scripts_dir, '..', '..', '07_OPENDSS', 'modelo_parcial', 'simulate_sto33.py')
    ]

    print("================================================================================")
    print("PIPELINE COMPLETO DE AUTOMACAO — COBRIC TELEMETRIA SANTOS")
    print("================================================================================")

    t_start = time.time()
    for s in pipeline:
        if not run_script(s):
            print(f"\n[INTERROMPIDO] Falha na etapa {os.path.basename(s)}")
            sys.exit(1)

    print("\n================================================================================")
    print(f"PIPELINE EXECUTADO COM 100% DE SUCESSO EM {time.time()-t_start:.1f}s!")
    print("================================================================================")

if __name__ == '__main__':
    main()
