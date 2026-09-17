# Adding features

How to extend this API: a **brand-new feature** (table, permissions, full CRUD) or a **new endpoint** on an existing feature.

Copy **Products** (or **Todos**) as the vertical-slice template. Stack reminders:

| Layer | Tech |
| ----- | ---- |
| Domain | Entities (audit via `Domain.Common.Entity` when needed), permission constants, repository interfaces |
| Application | Wolverine commands/queries (static handlers), FluentValidation, per-use-case responses |
| Persistence | EF Core schema/migrations + Dapper repositories |
| WebApi | Carter modules + `IEndpoint` classes |

No MediatR, AutoMapper, or MVC controllers. Repositories, handlers, validators, Carter modules, and permission policies are **auto-discovered** — you rarely edit DI.

---

## Naming conventions

Replace `{Feature}` (plural folder) and `{Entity}` (singular type):

| Piece | Pattern | Example |
| ----- | ------- | ------- |
| Entity | `Domain/{Feature}/Entities/{Entity}.cs` | `Domain/Products/Entities/Product.cs` |
| Repo interface | `Domain/{Feature}/Repositories/I{Entity}Repository.cs` | `IProductRepository` |
| Permissions | `Domain/Authorization/{Entity}Permissions.cs` | `create:product`, … |
| Command | `Application/{Feature}/Commands/{Verb}{Entity}/` | `CreateProductCommand` + handler + response + validator |
| Query | `Application/{Feature}/Queries/{Name}/` | `GetProductByIdQuery` + handler + response |
| Response | `{Verb}{Entity}Response.cs` in the same use-case folder | `CreateProductResponse` |
| Access (ownership) | `Application/{Feature}/{Entity}Access.cs` | optional |
| Repo impl | `Persistence/{Feature}/Repositories/{Entity}Repository.cs` | `internal sealed` |
| SQL | `Persistence/{Feature}/Sql/{Entity}Sql.cs` | Dapper statement constants |
| EF config | `Persistence/Configurations/{Entity}Configuration.cs` | `IEntityTypeConfiguration<>` |
| Migration | `dotnet ef migrations add {Name}` → `Persistence/Migrations/` | EF Core |
| API module | `WebApi/Features/{Feature}/{Feature}Module.cs` | `ICarterModule` |
| Endpoint | `WebApi/Features/{Feature}/Endpoints/{Verb}{Entity}.cs` | `IEndpoint` |
| Body DTO (when route has `{id}`) | `{Verb}{Entity}Body` in the endpoint file | avoid generic `Request` |

- Permissions: `action:resource` (`create:todo`, `read:user`).
- Routes: `api/v{apiVersion:apiVersion}/{feature-kebab}` (e.g. `products`).
- Tables/columns: `snake_case` (EF naming conventions for migrations; Dapper `MatchNamesWithUnderscores` at runtime).
- Audited entities inherit `Domain.Common.Entity` and set `CreatedBy` / `UpdatedBy` in handlers.

---

## 1. Brand-new feature (table + permissions + full CRUD)

Work **bottom-up**. Example: feature `Widgets` / entity `Widget` / resource `widget`.

### Checklist (dependency order)

#### Step 1 — Domain

1. `src/Domain/Widgets/Entities/Widget.cs` — `public sealed class` (inherit `Entity` when audit fields apply) with public getters/setters (no factory `Create` / mutator `Update`).
2. `src/Domain/Widgets/Repositories/IWidgetRepository.cs` — mirror Products: `GetByIdAsync`, `ListAsync`, `CountAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`.
3. `src/Domain/Authorization/WidgetPermissions.cs` — `Create` / `Read` / `Update` / `Delete` + `All`.
4. Edit `src/Domain/Authorization/PermissionRegistry.cs` — add `.. WidgetPermissions.All`.

#### Step 2 — Application

5. Optional: `WidgetAccess.cs` if callers are scoped to ownership (see Todos).
6. Per use case under `Commands/` and `Queries/` — **one file each** for Command/Query, Handler, and Response (plus Validator when needed):
   - `CreateWidget` — Command, Handler, Response, Validator
   - `UpdateWidget` — Command, Handler, Response, Validator
   - `DeleteWidget` — Command, Handler (no Response when returning `Deleted`)
   - `GetWidgetById` — Query, Handler, Response
   - `ListWidgets` — Query, Handler, Response (item + page wrapper), Validator

Handler shape:

```csharp
public static class CreateWidgetCommandHandler
{
    public static async Task<ErrorOr<CreateWidgetResponse>> HandleAsync(
        CreateWidgetCommand command,
        ILogger logger,
        IWidgetRepository widgetRepository,
        /* other DI */,
        CancellationToken cancellationToken = default)
    {
        // Map the entity to CreateWidgetResponse inline.
    }
}
```

#### Step 3 — Persistence

7. `src/Persistence/Widgets/Sql/WidgetSql.cs` — `internal static` SQL constants (shared `Columns`, GetById/List/Count/Insert/Update/Delete).
8. `src/Persistence/Widgets/Repositories/WidgetRepository.cs` — `internal sealed`, inject `INpgsqlConnectionFactory`, call `WidgetSql.*`. **No manual DI** (Scrutor registers `*Repository`). Do **not** inject `AppDbContext` for CRUD.
9. `src/Persistence/Configurations/WidgetConfiguration.cs` — table, keys, indexes, FKs (EF schema only).
10. Add `DbSet<Widget>` on `AppDbContext` so migrations see the entity.
11. New EF migration (also seed permissions + role grants):

```bash
dotnet ef migrations add CreateWidgetsAndSeedWidgetPermissions \
  --project src/Persistence --startup-project src/WebApi --output-dir Migrations
```

   - Prefer `HasData` / migration `InsertData` for permission rows (stable Guids).
   - Assign to roles (`admin`, and `user` if appropriate).
   - **Do not** only edit an already-applied migration.

#### Step 4 — WebApi

12. `src/WebApi/Features/Widgets/WidgetsModule.cs` — `ICarterModule`, group route, `MapEndpoint<…>()` for each endpoint.
13. Endpoints under `Endpoints/`:
    - `CreateWidget` — `MapPost("/")`, `RequirePermission(WidgetPermissions.Create)`, `ToCreated(...)`
    - `ListWidgets` / `GetWidgetById` — `ToOk()`
    - `UpdateWidget` — route id + uniquely named body → command
    - `DeleteWidget` — `ToNoContent()`

Endpoint pattern: `internal sealed class X : IEndpoint` with `static void Map(...)`, invoke via `IMessageBus.InvokeAsync<ErrorOr<T>>(...)`.

#### Step 5 — Verify

14. Update `README.md` feature table if you document APIs there.
15. Run migrations / start the API — `PermissionConsistencyCheck` must pass (every `PermissionRegistry` name must exist in DB).

### What is auto-wired

| Concern | Mechanism |
| ------- | --------- |
| Repositories | Scrutor in Persistence DI |
| Handlers | Wolverine assembly scan |
| Validators | FluentValidation `includeInternalTypes` |
| Carter modules | `AddCarter` / `MapCarter` |
| Permission policies | `PermissionPolicyProvider` on `.RequirePermission(...)` |

### What you must touch manually

- `PermissionRegistry`
- EF model config + migration (including permission seed)
- `*Module.MapEndpoint<T>()`

---

## 2. New endpoint on an existing feature

References:

- **CompleteTodo** — extra action; reuses `update:todo`
- **AssignRoleToUser** — extra Users action; reuses `update:user`

### Checklist (example: `ArchiveTodo`)

1. **Domain (if needed)** — new fields on the entity. Skip if existing fields suffice. Handlers assign properties directly.
2. **Application** — new folder:
   - `Application/Todos/Commands/ArchiveTodo/ArchiveTodoCommand.cs`
   - `ArchiveTodoCommandHandler.cs`
   - `ArchiveTodoResponse.cs` (when the endpoint returns a DTO)
   - `ArchiveTodoValidator.cs` (if input needs rules)
   - Reuse repository and access helpers.
3. **Persistence** — only if new columns; otherwise reuse `UpdateAsync`. New columns → EF configuration + Dapper SQL updates + `dotnet ef migrations add`.
4. **Permissions** — usually reuse an existing constant (`TodoPermissions.Update`). New permission only if product requires it → constant + `All` + **additive** migration (registry group already listed).
5. **WebApi** — `Features/Todos/Endpoints/ArchiveTodo.cs` implementing `IEndpoint`.
6. **Module** — `.MapEndpoint<ArchiveTodo>()` in `TodosModule.cs`.
7. **OpenAPI** — if the route has `{id}` and a body, use a unique `ArchiveTodoBody` record in the endpoint file (same pattern as `CompleteTodoBody` / `UpdateTodoBody`).

### Minimal file set (no schema change)

Command + Handler + Response (+ Validator) + Endpoint + one `MapEndpoint` line in the feature module.

---

## Order at a glance

```
1. Entity + IRepository + *Permissions + PermissionRegistry
2. Access (optional)
3. Commands & Queries (+ Handlers + Responses + Validators)
4. Dapper Repository + `{Entity}Sql` + IEntityTypeConfiguration
5. EF migration (table + permission seed)
6. Carter Module + Endpoints
7. README (optional)
```

**New feature:** full chain above.  
**New endpoint:** usually Application + endpoint + module line; Domain / Persistence / permissions only when needed.

---

## Related

- [README](../README.md) — architecture overview, auth, migrations
- Template CRUD: `src/**/Products/**`
- Extra action without new permission: `CompleteTodo`, `AssignRoleToUser`
