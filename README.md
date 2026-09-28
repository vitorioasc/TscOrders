# TscOrders API

Backend de gestão de pedidos para um e-commerce simples, construído em **.NET 10** seguindo **Clean Architecture** e **CQRS** com **MediatR**.

## Stack

- .NET 10 — **Minimal API**
- Clean Architecture (Domain → Application → Infrastructure → API)
- CQRS com MediatR (commands e queries separados) + pipeline behaviors (validação e logging)
- Entity Framework Core + SQLite, com migrations aplicadas automaticamente na inicialização
- Autenticação JWT (usuário fixo em memória)
- FluentValidation, com validação executada via pipeline behavior do MediatR
- xUnit + Moq + FluentAssertions para testes unitários dos handlers e da entidade `Order`
- Teste de integração com `WebApplicationFactory` para os endpoints de autenticação
- Serilog (console) para logging estruturado
- OpenTelemetry (tracing ASP.NET Core) com exportação para console
- Docker / docker-compose

## Por que Minimal API (e não Controllers)

O escopo é pequeno (5 endpoints, 2 recursos: `auth` e `orders`) e a lógica de negócio já vive inteiramente na camada de Application (handlers do MediatR). Nesse cenário, uma controller seria só a porta de entreda reexpondo `ISender.Send(...)` — Minimal API remove essa camada de indireção sem custo de testabilidade (os handlers são testados diretamente, sem precisar subir a API) e é o padrão recomendado pela própria Microsoft para APIs deste tamanho a partir do .NET 6+. Os endpoints ficam organizados em `Endpoints/*.cs` com `MapGroup` por recurso, mantendo a mesma separação de responsabilidades que teríamos com Controllers, mas com menos boilerplate (sem atributos de rota, sem DI por construtor de classe).

## Arquitetura e mapeamento de camadas


| Camada Clean Architecture | Projeto | Conteúdo |
|---|---|---|
| **Domain** | `TscOrders.Core.Abstraction` | Entidades (`Order`, `OrderItem`), enums, exceções de domínio e as *interfaces* dos serviços (`IJwtTokenGenerator`, `IDateTimeProvider`) consumidos pela Application |
| **Application (contratos)** | `TscOrders.Data.Abstraction` | Interfaces de repositório (`IOrderRepository`) e `IUnitOfWork` — dependem apenas do Domain |
| **Application (implementação)** | `TscOrders.Core` | Commands/Queries + Handlers (CQRS/MediatR), validators (FluentValidation), pipeline behaviors, DTOs |
| **Infrastructure** | `TscOrders.Data` | `DbContext`, configurações do EF Core, `OrderRepository`, `UnitOfWork`, migrations, e as implementações concretas de `IJwtTokenGenerator`/`IDateTimeProvider` (não há um projeto `Infrastructure` dedicado na estrutura original, então essas implementações de infraestrutura ficam aqui, ao lado da persistência) |
| **API** | `TscOrders.WebApi` | Minimal API endpoints, composição da aplicação (DI, JWT, Swagger/Scalar, exception handling), `Program.cs` |

Regra de dependência respeitada: `Core.Abstraction` não depende de nada; `Data.Abstraction` depende só de `Core.Abstraction`; `Data` e `Core` dependem das suas respectivas abstrações; `WebApi` depende de tudo e é o único ponto de composição.


## Regras de negócio (aplicadas no domínio, `Order.cs`)

- Um pedido precisa ter pelo menos 1 item (`Order.Create` lança `DomainException` caso contrário).
- `Quantity` e `UnitPrice` devem ser maiores que zero (validado tanto no domínio quanto via FluentValidation, para retornar 400 amigável antes de chegar à regra de domínio).
- Apenas pedidos `Pending` podem ser cancelados (`Order.Cancel()`).
- `TotalAmount` é uma propriedade calculada (`Items.Sum(i => i.Quantity * i.UnitPrice)`) — nunca persistida, nunca calculada na Application.

Exceções de domínio/aplicação são traduzidas para HTTP no `GlobalExceptionHandler` (`WebApi/Extensions`):

| Exceção | Status |
|---|---|
| `FluentValidation.ValidationException` | 400 |
| `InvalidCredentialsException` | 401 |
| `NotFoundException` | 404 |
| `DomainException` | 409 |
| outras | 500 |

## Autenticação

`POST /auth/login` valida contra um usuário fixo em memória (`dev@martech.com` / `Senha@123`, ver `TscOrders.Core/Auth/InMemoryUser.cs`) e devolve um JWT assinado (HMAC-SHA256). Todos os endpoints de `/api/orders` exigem `Authorization: Bearer <token>`.


## Endpoints

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| POST | `/auth/login` | Retorna JWT | Não |
| POST | `/api/orders` | Cria um pedido | Sim |
| GET | `/api/orders?page=1&pageSize=10` | Lista pedidos paginados | Sim |
| GET | `/api/orders/{id}` | Busca pedido por id | Sim |
| PATCH | `/api/orders/{id}/cancel` | Cancela um pedido pendente | Sim |

Documentação interativa (Scalar, apenas em Development): `GET /scalar/v1`.

## Como rodar localmente

Pré-requisito: SDK do .NET 10.

```bash
dotnet run --project TscOrders.WebApi/TscOrders.WebApi.csproj
```

A API sobe em `http://localhost:5068` (ver `TscOrders.WebApi/Properties/launchSettings.json`), aplica as migrations do SQLite automaticamente e cria o arquivo `tscorders.db` (ou `tscorders.dev.db` em Development) na pasta do projeto.

Exemplo de fluxo via curl:

```bash
TOKEN=$(curl -s -X POST http://localhost:5068/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"dev@martech.com","password":"Senha@123"}' | grep -o '"token":"[^"]*"' | cut -d'"' -f4)

curl -X POST http://localhost:5068/api/orders \
  -H "Content-Type: application/json" -H "Authorization: Bearer $TOKEN" \
  -d '{"customerId":"11111111-1111-1111-1111-111111111111","items":[{"productName":"Widget","quantity":2,"unitPrice":9.99}]}'
```

## Como rodar via Docker

```bash
docker compose up --build
```

A API fica disponível em `http://localhost:8080`, com o SQLite persistido no volume `tscorders-data`. Para subir também o SonarQube (opcional, item desejável do enunciado):

```bash
docker compose --profile quality up sonarqube
```

## Testes

```bash
dotnet test
```

- `TscOrders.Tests/Domain` — regras de negócio da entidade `Order` (item mínimo, valores positivos, cancelamento).
- `TscOrders.Tests/Orders` e `.../Auth` — testes unitários de cada handler do MediatR (mocks de `IOrderRepository`/`IUnitOfWork`/`IJwtTokenGenerator` via Moq).
- `TscOrders.Tests/Common/Behaviors` — `ValidationBehavior` (pipeline do MediatR).
- `TscOrders.Tests/Integration` — teste de integração com `WebApplicationFactory` cobrindo `/auth/login` e a exigência de autenticação em `/api/orders` (SQLite in-memory, sem tocar no arquivo real).

## Migrations

As migrations já estão versionadas em `TscOrders.Data/Persistence/Migrations` e são aplicadas automaticamente no startup (`dbContext.Database.Migrate()` em `Program.cs`). Para gerar uma nova migration:

```bash
dotnet tool install --global dotnet-ef # se ainda não tiver
dotnet ef migrations add NomeDaMigration \
  --project TscOrders.Data/TscOrders.Data.csproj \
  --startup-project TscOrders.WebApi/TscOrders.WebApi.csproj \
  -o Persistence/Migrations
```


