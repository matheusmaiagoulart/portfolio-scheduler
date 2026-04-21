# 📈 PortfolioScheduler — Sistema de Compra Programada de Ações

> Sistema automatizado de investimento recorrente em uma carteira recomendada de ações ("Top Five"), construído com **.NET 8**, **Clean Architecture**, **CQRS** e **DDD**.

⚠️ **Este projeto está em desenvolvimento ativo.**

---

## 🧭 Visão Geral

O PortfolioScheduler permite que clientes adiram a um plano de investimento automatizado baseado em uma carteira curada de **5 ações**. O sistema consolida as compras na conta master da corretora, distribui os ativos proporcionalmente para a custódia de cada cliente e gerencia resíduos para a próxima rodada de execução.

### Funcionalidades Principais

- **Adesão de clientes** com valor mensal de aporte configurável
- **Gestão de carteira recomendada** (Top Five — 5 ações com percentuais alvo)
- **Importação de arquivo COTAHIST da B3** — parse de cotações diárias da Bolsa de Valores
- **Motor de Compra Programada** — compra consolidada nos dias 5, 15 e 25 de cada mês
- **Distribuição proporcional** dos ativos comprados para as custódias individuais
- **Split automático lote padrão vs. mercado fracionário** (múltiplos de 100 vs. restante)
- **Gestão de resíduos na custódia Master** — sobras mantidas para a próxima rodada
- **Preço médio ponderado** atualizado por ativo por cliente

---

## 🏗️ Arquitetura

```
PortfolioScheduler/
├── PortfolioScheduler.Api            # Camada de Apresentação (API REST)
├── PortfolioScheduler.Application    # Camada de Aplicação (CQRS Commands/Handlers)
├── PortfolioScheduler.Domain         # Camada de Domínio (Entidades, Services, Errors)
├── PortfolioScheduler.Infra          # Camada de Infraestrutura (EF Core, Repositórios, Parser B3)
└── cotacoes/                         # Arquivos COTAHIST diários da B3
```

### Padrões & Princípios

| Padrão | Uso |
|--------|-----|
| **Clean Architecture** | Separação em 4 camadas com dependências unidirecionais |
| **CQRS** | Commands via MediatR com handlers dedicados por feature |
| **DDD** | Entidades ricas com comportamento, Domain Services, Domain Errors |
| **Repository Pattern** | Abstração de acesso a dados com EF Core |
| **Result Pattern** | FluentResults para tratamento de erros sem exceções |
| **Pipeline Behaviors** | `ValidationBehavior` + `TransactionBehavior` via MediatR |
| **Factory Method** | Criação de entidades com validação de domínio |

---

## 🛠️ Stack Tecnológica

| Componente | Tecnologia |
|------------|-----------|
| **Runtime** | .NET 8 (C#) |
| **Banco de Dados** | MySQL + Entity Framework Core |
| **Mensageria** | Apache Kafka *(planejado)* |
| **Validação** | FluentValidation |
| **Tratamento de Erros** | FluentResults (Result Pattern) |
| **Mediator** | MediatR |
| **Documentação API** | Swagger / OpenAPI |
| **Cotações** | Arquivo COTAHIST da B3 (TXT com campos fixos) |

---

## 🚀 Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Instância MySQL rodando
- Arquivo(s) COTAHIST da B3 na pasta `cotacoes/`

### Setup

```bash
# Clonar o repositório
git clone https://github.com/matheusmaiagoulart/portfolio-scheduler.git
cd portfolio-scheduler

# Atualizar a connection string no appsettings.json
# src/PortfolioScheduler.Api/appsettings.json

# Executar migrations
dotnet ef database update --project src/PortfolioScheduler.Infra --startup-project src/PortfolioScheduler.Api

# Executar a aplicação
dotnet run --project src/PortfolioScheduler.Api
```

O Swagger estará disponível em `https://localhost:{port}/swagger`.

---

## ⚙️ Motor de Compra — Como Funciona

O motor de compra executa nos dias úteis iguais ou subsequentes ao **5, 15 e 25** de cada mês:

```
1. Coletar 1/3 do aporte mensal de cada cliente ativo
2. Calcular quantidade consolidada por ativo (baseado nos % da carteira)
3. Descontar resíduos disponíveis na custódia Master
4. Executar compra — lote padrão (×100) + mercado fracionário (restante)
5. Distribuir ativos proporcionalmente para a custódia de cada cliente
6. Atualizar preço médio ponderado por ativo por cliente
7. Armazenar resíduos na custódia Master para a próxima rodada
```

### Split por Tipo de Mercado

```
350 ações de PETR4:
├── 300 → Lote padrão (PETR4)    — 3 lotes de 100
└──  50 → Fracionário (PETR4F)   — 50 unidades
```

### Distribuição

Cada cliente recebe ações proporcionalmente ao seu aporte:

```
Cliente A: R$ 1.000 (33,33% do total) → recebe ~33% de cada ativo
Cliente B: R$ 2.000 (66,67% do total) → recebe ~67% de cada ativo
Resíduos por truncamento → permanecem na custódia Master
```

---

## 📂 Arquivos COTAHIST da B3

Coloque os arquivos COTAHIST diários na pasta `cotacoes/`:

```
cotacoes/
├── COTAHIST_D27032026.TXT
├── COTAHIST_D28032026.TXT
└── ...
```

Download em: [Cotações Históricas B3](https://www.b3.com.br/pt_br/market-data-e-indices/servicos-de-dados/market-data/historico/mercado-a-vista/cotacoes-historicas/)

---

## 📊 Conceitos de Domínio

| Conceito | Descrição |
|----------|-----------|
| **Conta Master** (`BrokerageAccount` com `BrokerageAccountType.MASTER`) | Conta da corretora que consolida todas as compras antes da distribuição |
| **Conta Filhote** (`BrokerageAccount` com `BrokerageAccountType.CLIENT`) | Conta individual criada para cada cliente no momento da adesão |
| **Custódia Master** (`Custody` vinculada à conta Master) | Posição de ativos remanescentes na conta master após distribuição |
| **Custódia Filhote** (`Custody` vinculada à conta Client) | Posição individual de ativos por cliente, com quantidade e preço médio |
| **Carteira Top Five** (`RecommendedPortfolio` + `PortfolioItem`) | 5 ações recomendadas com percentuais de alocação (soma = 100%) |
| **Ordem de Compra** (`PurchaseOrder`) | Registro de compra consolidada na conta master com tipo de mercado (`MarketType`) |
| **Cliente** (`Customer`) | Pessoa que adere ao produto com valor mensal de aporte |
| **Lote Padrão** (`MarketType.BATCH`) | Negociação na B3 em múltiplos de 100 ações (ex: `PETR4`) |
| **Mercado Fracionário** (`MarketType.FRACTIONAL`) | Negociação de 1 a 99 ações com sufixo `F` (ex: `PETR4F`) |
| **Preço Médio Ponderado** (`Custody.AveragePrice`) | `(Qtd × PreçoMédio + NovaQtd × NovoPreço) / (Qtd + NovaQtd)` |

---

## 📋 Progresso das Funcionalidades

| Funcionalidade | Status |
|----------------|--------|
| Adesão de clientes | ✅ |
| Gestão da carteira Top Five | ✅ |
| Importação e parse do COTAHIST | ✅ |
| Motor de Compra (passos 1–6) | ✅ |
| Distribuição proporcional | ✅ |
| Gestão de resíduos na Master | ✅ |
| Preço médio ponderado | ✅ |
| Batch processing (escalabilidade) | ✅ |
| IR dedo-duro (Kafka) | 🟡 Planejado |
| Rebalanceamento de carteira | 🟡 Planejado |
| Consulta de carteira do cliente | 🟡 Planejado |
| Testes unitários (70%+ cobertura) | 🟡 Planejado |
| Docker Compose | 🟡 Planejado |

---

## 📐 Estrutura do Projeto

```
src/
├── PortfolioScheduler.Api/
│   ├── Controller/             # Endpoints REST
│   ├── Extensions/             # Helpers de mapeamento de Result
│   └── Middlewares/            # Tratamento de exceções
│
├── PortfolioScheduler.Application/
│   ├── Commands/               # Comandos CQRS + Handlers
│   ├── Behaviors/              # Pipeline MediatR (Validação, Transação)
│   └── Validators/             # Regras FluentValidation
│
├── PortfolioScheduler.Domain/
│   ├── Entities/               # Entidades ricas de domínio
│   ├── Repositories/           # Interfaces de repositório
│   ├── Services/               # Domain Services (interfaces + implementações)
│   ├── DomainErrors/           # Definições centralizadas de erros
│   └── Services/DTOs/          # Objetos de transferência de domínio
│
└── PortfolioScheduler.Infra/
    ├── Data/                   # EF Core DbContext, Migrations, Mappings
    ├── Persistence/            # Implementações de repositório
    └── ExternalData/           # Parser COTAHIST da B3
```

---

## 📄 Licença

Este projeto é para fins educacionais e de portfólio.
