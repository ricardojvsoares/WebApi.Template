# WebApi

A .NET 10 Web API built with Clean Architecture. It ships with permission-based JWT auth, Todo and User CRUD, FluentMigrator migrations for Postgres, and RabbitMQ integration events via Wolverine.

---

## Architecture

| Project            | Responsibility                                           |
| ------------------ | -------------------------------------------------------- |
| **Domain**         | Entities, permissions, repository interfaces             |
| **Application**    | Commands, queries, validators, handlers, responses       |
| **Infrastructure** | Password hashing, JWT / refresh tokens, RabbitMQ options |
| **Persistence**    | Dapper repositories, FluentMigrator migrations, seeding  |
| **WebApi**         | HTTP endpoints, auth wiring, OpenAPI / Scalar            |
| **Migrator**       | Console app for `up` / `down` / `list`                   |

Dependencies point inward: WebApi → Application ← Infrastructure / Persistence → Domain.

---

## Features

### Auth (`/api/v1/auth`)

| Method | Path        | Auth      | Summary        |
| ------ | ----------- | --------- | -------------- |
| `POST` | `/register` | Anonymous | Register       |
| `POST` | `/login`    | Anonymous | Log in         |
| `POST` | `/refresh`  | Anonymous | Refresh tokens |
| `POST` | `/logout`   | Bearer    | Log out        |
| `GET`  | `/me`       | Bearer    | Current user   |

Access tokens carry `permission` claims. Refresh tokens rotate on use; replaying a revoked token invalidates the whole family.

### Todos (`/api/v1/todos`)

Requires `*:todo` permissions. Callers only see their own todos.

| Method   | Path               | Permission    |
| -------- | ------------------ | ------------- |
| `POST`   | `/`                | `create:todo` |
| `GET`    | `/`                | `read:todo`   |
| `GET`    | `/{id}`            | `read:todo`   |
| `PUT`    | `/{id}`            | `update:todo` |
| `PUT`    | `/{id}/completion` | `update:todo` |
| `DELETE` | `/{id}`            | `delete:todo` |

Creating a todo publishes `TodoCreatedEvent` over RabbitMQ when messaging is enabled.

### Users (`/api/v1/users`)

Requires `*:user` permissions (admin role by default).

| Method   | Path          | Permission    |
| -------- | ------------- | ------------- |
| `POST`   | `/`           | `create:user` |
| `GET`    | `/`           | `read:user`   |
| `GET`    | `/{id}`       | `read:user`   |
| `PUT`    | `/{id}`       | `update:user` |
| `POST`   | `/{id}/roles` | `update:user` |
| `DELETE` | `/{id}`       | `delete:user` |

---

## Feature structure

Endpoints live under `Features/<Feature>/`. One Carter module owns the route group; each endpoint is its own file:

```
src/WebApi/Features/
  Auth/
    AuthModule.cs
    Endpoints/
      Register.cs
      Login.cs
      ...
  Todos/
    TodosModule.cs
    Endpoints/
      CreateTodo.cs
      ...
  Users/
    UsersModule.cs
    Endpoints/
      CreateUser.cs
      ...
```

Each endpoint implements `IEndpoint` and is wired with `MapEndpoint<T>()`. Request bodies bind to Application commands where the shape matches (for example `LoginCommand`). When the route also carries an id, a uniquely named body type is used so OpenAPI does not collide schemas across endpoints.

---

## Authentication & authorization

The API owns its user store and issues its own JWTs. Authorization is **permission based** — roles are only bundles of permissions.

Permissions are `action:resource` constants under `src/Domain/Authorization`, one file per group:

```csharp
public static class TodoPermissions
{
    public const string Create = "create:todo";
    public const string Read   = "read:todo";
    public const string Update = "update:todo";
    public const string Delete = "delete:todo";
}
```

`PermissionRegistry` aggregates every group. The seed migration mirrors it into the database, and startup fails fast if a declared permission has no matching row.

Endpoints opt in with:

```csharp
.RequirePermission(TodoPermissions.Create)
```

Default roles:

- **admin** — all permissions
- **user** — all `*:todo` permissions (assigned on self-registration)

---

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/) (Postgres + RabbitMQ)

### Run with Docker Compose

1. Copy env and set secrets:

   ```bash
   cp .env.example .env
   ```

   At minimum set a long `Jwt__SigningKey` (≥ 32 bytes) and update `PG_PASSWORD` / `Auth__SeedAdmin__*`.

2. Build and start:

   ```bash
   docker compose up --build
   ```

3. Open:

   | Service     | URL                            |
   | ----------- | ------------------------------ |
   | API (HTTP)  | `http://localhost:44320`       |
   | API (HTTPS) | `https://localhost:44321`      |
   | Scalar docs | `https://localhost:44321/docs` |
   | RabbitMQ UI | `http://localhost:15672`       |

Log in at `POST /api/v1/auth/login` with the seeded admin from `.env`, then paste the access token into Scalar's Authorize dialog (Bearer).

### Run the API locally

Start dependencies only, then run the host:

```bash
docker compose up -d postgres rabbitmq
dotnet run --project src/WebApi
```

Local URLs come from `launchSettings.json` (HTTP `http://localhost:5291`, docs at `/docs`). Point `PG_HOST` / `RabbitMq__Host` at `localhost` when not running inside Compose.

---

## Migrations

Migrations live in `src/Persistence/Migrations` as FluentMigrator classes.

- **Development:** pending migrations apply on API startup when `PG_RUN_MIGRATIONS_ON_STARTUP=true`.
- **CI / production:** use the Migrator console:

```bash
dotnet run --project src/Migrator -- list
dotnet run --project src/Migrator -- up
dotnet run --project src/Migrator -- down --steps 1
```

The admin user is **not** created by a migration. `AdminUserSeeder` runs after migrations and uses `Auth__SeedAdmin__Email` / `Auth__SeedAdmin__Password` (no-op if either is empty or the user already exists).

---

## Messaging

Wolverine publishes integration events to RabbitMQ when `RabbitMq__Enabled=true`. Set it to `false` to boot without a broker — publishing becomes a no-op and the rest of the API keeps working.

---

## Stack

- ASP.NET Core 10, Carter, Asp.Versioning, Scalar
- Wolverine (mediator + RabbitMQ transport) + FluentValidation
- Dapper + Npgsql (Postgres only)
- FluentMigrator
- Serilog
- ErrorOr

---

## Roadmap

### Done

- Clean Architecture layering
- Feature-based endpoints (`Features/*/Endpoints`)
- JWT auth with refresh-token rotation
- Permission-based authorization (`action:resource`)
- Todo and User CRUD
- FluentMigrator + Migrator console
- RabbitMQ integration events via Wolverine
- Global exception handling and OpenAPI bearer scheme

### Planned

- Architecture / unit tests
- CI/CD pipeline
- OpenTelemetry
- Health checks
- Rate limiting
- Transactional outbox for integration events
