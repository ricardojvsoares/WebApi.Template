using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Persistence.Data;

/// <summary>
/// Lets <c>dotnet ef</c> build <see cref="AppDbContext" /> using the same PG_* settings as runtime.
/// </summary>
public sealed class DesignTimeAppDbContext
    : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(
        string[] args)
    {
        if (!int.TryParse(Environment.GetEnvironmentVariable("PG_PORT"), out int port))
        {
            port = 5432;
        }

        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = Environment.GetEnvironmentVariable("PG_HOST") ?? "localhost",
            Port = port,
            Database = Environment.GetEnvironmentVariable("PG_DATABASE") ?? "webapi",
            Username = Environment.GetEnvironmentVariable("PG_USERNAME") ?? "postgres",
            Password = Environment.GetEnvironmentVariable("PG_PASSWORD") ?? "postgres"
        }.ConnectionString;

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new AppDbContext(builder.Options);
    }
}
