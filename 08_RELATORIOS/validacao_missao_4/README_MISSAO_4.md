# Diretório de Validação Científica — Missão 4
## Integração de Sistemas de Armazenamento BESS via Elementos Storage Nativos do OpenDSS
### Projeto COBRIC 2026 — Alimentador STO33 (CPFL Piratininga)

---

## 1. Conteúdo Deste Diretório

Este diretório contém todos os artefatos, códigos-fonte, relatórios e evidências numéricas geradas na **Missão 4**, garantindo a reprodutibilidade integral de todos os resultados:

1. **`executar_missao_4.py`:** Orquestrador principal em Python que executa os 5 cenários, coleta métricas elétricas, telemetria interna dos elementos Storage e realiza a auditoria dos 10 testes físicos e do CAPEX.
2. **`RELATORIO_TECNICO_MISSAO_4.md`:** Relatório técnico detalhado com a fundamentação de engenharia elétrica, equacionamento do balanço de energia dos BESS, auditoria do reforço físico e diretrizes para o artigo científico do COBRIC 2026.
3. **`RESULTADOS_CENARIOS_COBRIC_M4.csv`:** Tabela consolidada de indicadores de desempenho dos 5 cenários (pico de demanda, perdas ativas, tensões mínimas/máximas, CAPEX e convergência).
4. **`CURVAS_HORARIAS_COBRIC_M4.csv`:** Série temporal horária (24 passos) para cada cenário contendo potências ativa e reativa da subestação, perdas e limites de tensão.
5. **`BESS_ESTADO_INTERNO_M4.csv`:** Telemetria interna extraída diretamente dos registradores do elemento `Storage` do OpenDSS via monitores de estado (mode=3), registrando `kWhStored`, `SOC (%)`, `State` e potência nos terminais.
6. **`BESS_TESTES_VALIDACAO_M4.csv`:** Registro dos 10 testes de integridade física e operacional dos acumuladores eletroquímicos (limites de SOC, potências, eficiências e ausência de sobreposição de despacho).
7. **`CARREGAMENTO_TRAFOS_M4.csv`:** Carregamento horário dos transformadores de distribuição críticos (`TR_1193581`, `TR_1193923`, `TR_1193996`, `TR_48964485`).
8. **`AUDITORIA_CAPEX_CENARIO_E.csv`:** Recálculo auditado das quantidades físicas e custos do reforço convencional (188 trechos PA6001 = 5,247 km + 3 transformadores = R$ 624.511,55).
9. **`CLASSIFICACAO_VALIDACAO_M4.csv`:** Parecer formal de validação de cada cenário.
10. **`backup_m3/`:** Cópia de segurança intacta dos arquivos `.dss` da Missão 3 antes das modificações.

---

## 2. Como Reproduzir a Simulação

Para reproduzir todos os resultados de forma independente a partir do terminal:

```bash
cd /Users/guga/Desktop/COBRIC_Simulador_Visual/COBRIC_TELEMETRIA_SANTOS
/Users/guga/Desktop/COBRIC_Simulador_Visual/.venv/bin/python3 08_RELATORIOS/validacao_missao_4/executar_missao_4.py
```

---

## 3. Principais Conclusões Científicas

- **BESS Nativo OpenDSS:** A modelagem através de elementos `Storage` em modo `DispMode=Follow` reproduz com fidelidade a física dos acumuladores, com gestão interna de estado de carga e perdas intrínsecas de conversão ($\eta_{\text{RT}} = 90,25\%$).
- **Centralizado vs Distribuído:** O BESS Centralizado na subestação atua como recurso de corte de ponta sistêmico, mas é ineficaz para mitigar sobrecargas térmicas na baixa tensão. O BESS Distribuído (Cenário D) reduz o carregamento do transformador crítico de 94,67% para 54,58%, apresentando melhor custo-benefício por kW de alívio local.
- **CAPEX Corrigido do Reforço Físico:** A extensão real de 5,247 km de condutores PA6001 eleva o CAPEX do Cenário E de R$ 553.500,00 para R$ 624.511,55 (+12,83%). Ainda assim, a obra convencional apresenta o menor custo de implantação imediata, cabendo ao artigo acadêmico destacar as vantagens de flexibilidade operacional que apenas as baterias podem oferecer.
