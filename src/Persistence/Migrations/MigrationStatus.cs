namespace Persistence.Migrations;

public sealed record MigrationStatus(
    string Id,
    string Name,
    bool IsApplied);
