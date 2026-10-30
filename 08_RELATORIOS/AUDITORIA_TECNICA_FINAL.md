# Auditoria Técnica Final — COBRIC 2026
## Alimentador STO33 — Geração distribuída, OpenDSS e Missão 4

**Escopo:** auditoria documental e de consistência dos dados e arquivos versionados.
**Commit de referência:** `cffd62a90599d08875b2479ac752210f86721680`.

## 1. Síntese executiva

A auditoria identificou evidências consistentes para a potência fotovoltaica total de 195,89 kW associada aos 23 registros do alimentador STO33, mediante cruzamento individual dos identificadores CEG com a base bruta ANEEL MMGD.

Entretanto, foi identificada uma divergência crítica na conexão elétrica dos sistemas fotovoltaicos no arquivo `PVSystems.dss`: os barramentos utilizados não correspondem aos barramentos secundários `PAC_2` associados aos transformadores no restante do modelo.

Também foram identificadas limitações metodológicas nos resultados da Missão 4. Os resultados computacionais não devem ser apresentados como validação elétrica independente enquanto as inconsistências descritas neste relatório não forem resolvidas.

## 2. Auditoria da geração distribuída

A consulta à camada `UGBT_tab` da BDGD, filtrada por `CTMT = 'STO33'`, identificou 23 registros.

O cruzamento dos 23 identificadores `CEG_GD` com a coluna `CodEmpreendimento` da base bruta ANEEL MMGD encontrou:

- Registros STO33: 23.
- Correspondências ANEEL: 23.
- Registros sem correspondência: 0.
- Potência total ANEEL: 195,89 kW.
- Município dos 23 registros: Cubatão/SP.
- Código IBGE: 3513504.

A soma da potência original `POT_INST` da BDGD é 44,35277778 kW. A soma da potência nominal correspondente na base ANEEL é 195,89 kW.

O arquivo tratado e o CSV utilizado na preparação do modelo OpenDSS apresentam 23 registros, mesmos identificadores CEG e total de 195,89 kW. A diferença entre os totais corrigidos é zero.

**Conclusão:** o total de 195,89 kW possui suporte no cruzamento individual com a base bruta ANEEL. A rastreabilidade do procedimento de correção, porém, precisa ser incorporada ao script de tratamento para que a transformação seja reproduzível.

## 3. Divergência na conexão fotovoltaica do OpenDSS

O arquivo `PVSystems.dss` contém 23 sistemas fotovoltaicos e soma 195,89 kW em `pmpp`.

O `Master.dss` inclui esse arquivo por meio de `Redirect PVSystems.dss`.

A comparação dos barramentos identificou que:

- Os 23 sistemas fotovoltaicos utilizam barramentos derivados de `PAC`, com prefixo `BT_STO33_` duplicado.
- Os 23 barramentos `PAC_2` associados aos transformadores aparecem nos demais arquivos do modelo.
- Nenhum dos 23 valores de `PAC` foi localizado nos demais arquivos consultados.
- Os 23 barramentos `bus1` dos sistemas fotovoltaicos diferem dos respectivos `PAC_2`.

O construtor `09_AUTOMACAO/scripts/07_construir_modelo_opendss.py`, por sua vez, utiliza `PAC_2` ao gerar os sistemas fotovoltaicos.

O histórico do commit de referência mostra que `PVSystems.dss` teve seus barramentos alterados de valores correspondentes a `PAC_2` para valores derivados de `PAC`, além da atualização das potências.

**Conclusão:** existe inconsistência entre o arquivo DSS versionado e a lógica do construtor. Isso indica possível desconexão elétrica dos sistemas fotovoltaicos em relação aos barramentos secundários esperados. A desconexão efetiva ainda precisa ser confirmada pela inspeção da topologia compilada e por testes de conectividade no OpenDSS.

## 4. Auditoria metodológica da Missão 4

Os resultados existentes indicam execução de 24 horas e 120 intervalos por cenário. A convergência do solver, isoladamente, não comprova a validade física do modelo nem a adequação dos resultados.

Foram identificadas as seguintes limitações:

1. Os cenários C e D apresentam valores de pico idênticos, apesar de capacidades distintas.
2. A redução de demanda atribuída aos cenários com BESS precisa ser reavaliada diante dos períodos de potência congelada e da consistência dos perfis.
3. O balanço energético apresenta divergência entre o SOC inicial e final, comprometendo a interpretação de ciclo repetível.
4. A eficiência de 99,52% apresentada no teste T5 não representa eficiência de ida e volta calculada a partir da energia efetivamente retirada e devolvida ao armazenamento. Com eficiências de carga e descarga de 95% cada, a eficiência teórica combinada seria 90,25%.
5. O teste T4 compara grandezas distintas, kVA e kW, sem explicitar adequadamente o fator de potência.
6. A classificação dos cenários é parcialmente codificada de forma fixa, sem derivação integral dos resultados dos testes.
7. O teste T9 menciona transformadores e linhas, mas a evidência apresentada se concentra nos transformadores.
8. O CAPEX do cenário de reforço físico utiliza parâmetros fixos de extensão, custo por quilômetro e quantidade de transformadores. As referências e premissas precisam ser detalhadas.
9. A redução de 47,4% de demanda atribuída ao cenário BESS não deve ser considerada comprovada até que os perfis, limites de potência, SOC e balanços sejam novamente verificados.

## 5. Reprodutibilidade e proveniência

O script `03_processar_dados_tratados.py` copia a potência original da BDGD para o CSV tratado e exporta separadamente os registros ANEEL de Santos. Não implementa, no trecho auditado, o cruzamento individual dos CEGs do STO33 com a base ANEEL.

O construtor OpenDSS utiliza `PAC_2`, enquanto o `PVSystems.dss` versionado no commit de referência utiliza barramentos derivados de `PAC`.

Portanto, os arquivos gerados e a lógica de geração não estão integralmente alinhados. O procedimento de retificação precisa ser automatizado e documentado.

## 6. Ações técnicas necessárias

1. Corrigir o tratamento de geração distribuída para executar e registrar o cruzamento individual dos 23 CEGs.
2. Regenerar `PVSystems.dss` utilizando os barramentos secundários `PAC_2` correspondentes aos transformadores.
3. Compilar o `Master.dss` e verificar a existência e conectividade dos barramentos fotovoltaicos.
4. Comparar potência ativa, potência reativa, tensão e perdas com e sem os sistemas fotovoltaicos.
5. Reexecutar os cenários da Missão 4 após a correção topológica.
6. Recalcular o balanço energético e a eficiência de ida e volta do BESS.
7. Documentar as premissas e fontes do CAPEX do reforço físico.
8. Atualizar os relatórios e classificações somente após os testes independentes.

## 7. Conclusão

A potência fotovoltaica de 195,89 kW possui suporte no cruzamento dos 23 registros com a base ANEEL MMGD, mas a representação elétrica desses sistemas no arquivo `PVSystems.dss` apresenta divergência de barramentos que precisa ser corrigida e validada.

Os resultados da Missão 4 devem ser tratados como resultados computacionais preliminares, não como comprovação definitiva de desempenho da solução.

Esta auditoria registra as evidências e limitações identificadas. Não declara como executadas as correções ou simulações que ainda estão pendentes.
