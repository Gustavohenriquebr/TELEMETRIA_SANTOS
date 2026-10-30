# METODOLOGIA DE ENGENHARIA, RASTREABILIDADE E LIMITAÇÕES TÉCNICAS
## INVESTIGAÇÃO DA REDE ELÉTRICA DE SANTOS/SP — ALIMENTADOR STO33

---

## 1. DIRETRIZ METODOLÓGICA DE EXECUÇÃO

A investigação foi orientada pelo princípio da **engenharia reversa estrita baseada em evidências**, estruturada nas 8 etapas recomendadas pela missão:

```
[ ETAPA A: Descoberta ] --> Localização de endpoints CKAN, serviços ArcGIS, dicionários e metadados.
          |
[ ETAPA B: Aquisição ]  --> Download automatizado via TLS 1.2 e streaming de bases oficiais completas.
          |
[ ETAPA C: Processamento ] --> Normalização, cruzamento por PACs e relacionamento relacional.
          |
[ ETAPA D: Reconstrução ] --> Construção do grafo topológico em NetworkX e validação de conectividade.
          |
[ ETAPA E: Diagnóstico ]   --> Cálculo de carregamentos, identificação de gargalos e classificação de riscos.
          |
[ ETAPA F: Telemetria ]   --> Matriz de variáveis, protocolos de comunicação e lacunas de dados.
          |
[ ETAPA G: OpenDSS ]      --> Modelagem elétrica parametrizada e simulação de fluxo de potência.
          |
[ ETAPA H: Expansão ]     --> Mapeamento dos 99 alimentadores municipais de Santos.
```

---

## 2. SISTEMA DE CLASSIFICAÇÃO DE QUALIDADE E CONFIABILIDADE (CLASSES A A G)

Todos os dados utilizados e gerados foram classificados de forma não ambígua:

| Classe | Descrição Regulatória | Exemplos Utilizados no Projeto |
| :---: | :--- | :--- |
| **A** | **Medição operacional real** | *Não disponível publicamente para circuitos de distribuição (restrito ao SCADA interno da CPFL).* |
| **B** | **Registro oficial cadastral** | Topologia da rede primária (BDGD SSDMT), cadastro de transformadores (UNTRMT/EQTRMT), condutores (SEGCON), chaves (UNSEMT), usinas solares (UGBT) e frota de veículos (DETRAN-SP). |
| **C** | **Indicador oficial agregado** | Percentual de carregamento máximo no INDGER (`PctCarregamentoMaxAlimentador`), contagem mensal de consumidores ativos, DIC/FIC da ANEEL. |
| **D** | **Dado oficial de referência ou proxy** | Curvas de demanda horárias do Sistema CTR da ANEEL (Campanhas de Medição Tarifária da CPFL Piratininga), Atlas Solarimétrico do INPE. |
| **E** | **Cálculo derivado de dados oficiais** | Demanda de pico estimada por transformador ($P_{\text{est}}$ calculada por fatores de simultaneidade regulatórios aplicados sobre o número de UCs da BDGD), perdas ativas no OpenDSS. |
| **F** | **Estimativa ou hipótese de engenharia** | Curva de recarga residencial de veículos elétricos (Wallbox de 7,4 kW com fator de coincidência empírico de 30% a 50%). |
| **G** | **Dado ausente ou não verificável** | Séries temporais de corrente minutais de fase ($I_a, I_b, I_c$), oscilografias de disparo, estado operacional em tempo real de chaves telecomandadas. |

---

## 3. LIMITAÇÕES TÉCNICAS E LACUNAS IDENTIFICADAS

1. **Ausência de Telemetria de Corrente em Tempo Real:**
   - A ANEEL e a CPFL disponibilizam apenas o valor de pico anual/mensal (`PctCarregamentoMaxAlimentador`) e energias mensais faturadas (`ENE_01` a `ENE_12`). O formato de onda e a curva de carga real de 15 minutos do alimentador dependem de fornecimento formal pela distribuidora.
2. **Representação da Carga Secundária Agregada:**
   - O modelo elétrico consolida os consumidores de baixa tensão em centros de carga concentrados nos barramentos secundários de cada um dos 105 transformadores MT/BT. Essa abordagem é padrão internacional para estudos de rede de média tensão, preservando com exatidão a impedância e o perfil térmico dos 19,37 km da rede primária, mas abstrai as quedas de tensão internas nos ramais prediais de baixa tensão.
3. **Fator de Simultaneidade e Diversidade de Carga:**
   - A demanda de pico de cada transformador foi calculada segundo a formulação empírica de diversidade residencial urbana $F_{\text{sim}} = 0,2 + 0,8 / \sqrt{N}$, calibrada para a classe socioeconômica de Santos. Variações microclimáticas de uso simultâneo de ar-condicionado podem elevar a demanda em dias de calor extremo.
