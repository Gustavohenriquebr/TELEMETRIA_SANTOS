# Validação e Correção Física dos BESS — Missão 3 (COBRIC 2026)
## Diretório de Auditoria Técnica e Execução Rastreável
### Repositório: https://github.com/Gustavohenriquebr/TELEMETRIA_SANTOS

Este diretório contém a documentação técnica, os scripts e as matrizes de resultados da **Missão 3**, dedicada à correção física dos modelos de armazenamento de energia em baterias (BESS) para o artigo do COBRIC 2026.

---

## 1. Conteúdo Deste Diretório

| Arquivo | Descrição Técnica | Formato |
|:---|:---|:---:|
| [`RELATORIO_CORRECAO_BESS_COBRIC.md`](RELATORIO_CORRECAO_BESS_COBRIC.md) | Relatório executivo completo da Missão 3 detalhando o que foi corrigido, mantido e alterado nos cenários | Markdown |
| [`RESULTADOS_CENARIOS_COBRIC_MISSAO_3.csv`](RESULTADOS_CENARIOS_COBRIC_MISSAO_3.csv) | Tabela comparativa consolidada dos 5 cenários com o modelo físico rigoroso de BESS | CSV |
| [`CURVAS_HORARIAS_COBRIC_MISSAO_3.csv`](CURVAS_HORARIAS_COBRIC_MISSAO_3.csv) | Séries temporais horárias de 24 horas (potência ativa, reativa, perdas e tensões extremas) para os 5 cenários | CSV |
| [`BALANCO_ENERGETICO_BESS.csv`](BALANCO_ENERGETICO_BESS.csv) | Balanço horário de potência AC, energia carregada/descarregada, perdas internas e $SOC$ do BESS Central e dos 3 BESS Distribuídos | CSV |
| [`COMPARACAO_ANTES_DEPOIS.csv`](COMPARACAO_ANTES_DEPOIS.csv) | Matriz de comparação numérica detalhada entre os resultados da Missão 2 (sem controle de $SOC$) e da Missão 3 (físico) | CSV |
| [`VALIDACAO_FISICA_ELETRICA.md`](VALIDACAO_FISICA_ELETRICA.md) | Checklist e equacionamento físico-matemático do modelo de bateria e verificação nodal de carregamento de trafos | Markdown |
| [`ANALISE_ECONOMICA_ATUALIZADA.md`](ANALISE_ECONOMICA_ATUALIZADA.md) | Análise econômica detalhada separando CAPEX, OPEX, vida útil regulatória (35 anos vs 12 anos), reposição e LCOS | Markdown |
| [`carregamento_trafos_missao_3.csv`](carregamento_trafos_missao_3.csv) | Carregamento horário em kVA e % para os 4 transformadores de distribuição mais críticos do alimentador STO33 | CSV |
| [`executar_missao_3.py`](executar_missao_3.py) | Script Python orquestrador que reexecuta o OpenDSS, calcula os fluxos de carga e gera todas as tabelas deste diretório | Python |

---

## 2. Instruções de Reprodução

Para reproduzir integralmente esta auditoria a partir do código-fonte:

```bash
python3 08_RELATORIOS/validacao_missao_3/executar_missao_3.py
```
