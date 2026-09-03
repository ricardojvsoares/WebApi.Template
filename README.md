# 📦 WebApi

A modern .NET 10 Web API built with Clean Architecture, focusing on scalability, maintainability, and clear separation of concerns.

This project follows a layered architecture approach to ensure:

- ✅ Clear boundaries between business logic and external concerns

- ✅ Testable and maintainable codebase

- ✅ Dependency inversion and SOLID principles

- ✅ Scalable and production-ready structure


The primary goal of this project is to provide a robust foundation for building enterprise-grade APIs. It encourages best practices and clean coding standards, supporting extensibility and long-term maintainability for teams and organizations.

---

# 🏗️ Architecture Overview

The solution is organized into the following layers:

- Domain – Core business rules, entities, permissions and value objects

- Application – Use cases, responses, interfaces, validations, and business workflows

- Infrastructure – External services, security primitives and third-party integrations

- Persistence – Database access and migrations

- WebApi – ASP.NET Core Web API endpoints and configuration

- Migrator – Console entry point for applying database migrations

---

# 🧭 Feature structure

Endpoints are organized by feature. Each feature owns a single Carter module that declares the route prefix, API version set and tags, and one file per endpoint:

```
src/WebApi/Features/
  Todos/
    TodosModule.cs
    Endpoints/
      CreateTodo.cs
      ListTodos.cs
      GetTodoById.cs
      UpdateTodo.cs
      CompleteTodo.cs
      DeleteTodo.cs
```

Each endpoint file implements `IEndpoint`, and the module wires them together with `MapEndpoint<T>()`.

---

# 🔐 Authentication & Authorization

The API owns its user store and issues its own JWTs. Authorization is **permission based**, not role based — roles are only a convenient bundle of permissions.

Permissions are named `action:resource`, for example `create:todo` or `read:user`. They are declared as constants in `src/Domain/Authorization`, one file per group:

```csharp
public static class TodoPermissions
{
    public const string Create = "create:todo";
    public const string Read   = "read:todo";
    public const string Update = "update:todo";
    public const string Delete = "delete:todo";
}
```

An endpoint then requires one:

```csharp
app.MapPost("/", Handler).RequirePermission(TodoPermissions.Create);
```

Effective permissions are resolved at login (the union across the user's roles) and written into the access token as `permission` claims. A dynamic policy provider builds the matching authorization policy on demand, so no policy registration is needed per permission.

---

# 🛠️ Getting Started

## Prerequisites

Before running the project, make sure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/) (required for PostgreSQL and RabbitMQ)
- A compatible IDE: [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

## Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/plex4it/WebApi.git
   cd WebApi
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore WebApi.slnx
   ```

3. **Configure environment variables**

   Copy the example env file and update it with your local values:
   ```bash
   cp .env.example .env
   ```

   At minimum set `Jwt__SigningKey` to a long random value, and update the connection
   and admin bootstrap values.

4. **Run the application with Docker Compose**
   ```bash
   docker compose up --build
   ```

   The API will be available at `https://localhost:44321/docs/` by default, with
   RabbitMQ's management UI at `http://localhost:15672`.

## Running migrations

Migrations are FluentMigrator classes in `src/Persistence/Migrations`. In development the
API applies pending migrations on startup (`PG_RUN_MIGRATIONS_ON_STARTUP`). Everywhere
else, drive them explicitly with the Migrator console app:

```bash
dotnet run --project src/Migrator -- list           # show applied / pending migrations
dotnet run --project src/Migrator -- up             # apply all pending migrations
dotnet run --project src/Migrator -- down --steps 1 # roll back the last migration
```

## Messaging

Integration events are published over RabbitMQ using Wolverine's transport. Set
`RabbitMq__Enabled=false` to run the API without a broker — event publishing becomes a
no-op and everything else keeps working.

---
# 🗺️ Roadmap

## ✅ Completed
- ✅ Clean Architecture project structure
- ✅ Domain layer with entities and value objects
- ✅ Application layer with use cases and validation
- ✅ Persistence layer with Dapper integration
- ✅ WebApi layer with ASP.NET Core endpoints with API versioning
- ✅ Logging and observability setup (Serilog)
- ✅ Global error handling
- ✅ Authentication & Authorization (JWT, permission based)
- ✅ Database migrations (FluentMigrator)
- ✅ Message queuing (RabbitMQ via Wolverine)

## 🔄 In Progress
- [ ] CI/CD pipeline configuration (GitHub Actions)
- [ ] OpenTelemetry traces and metrics

## 🔮 Planned
- [ ] Architecture tests
- [ ] Rate limiting and throttling
- [ ] Health checks and monitoring endpoints
- [ ] Background job processing (Hangfire / Quartz.NET)
- [ ] Transactional outbox for integration events

> 💡 Have a suggestion? Feel free to open an issue or submit a pull request!
