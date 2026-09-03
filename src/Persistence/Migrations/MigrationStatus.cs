namespace Persistence.Migrations;

public sealed record MigrationStatus(
    long Version,
    string Name,
    bool IsApplied);
