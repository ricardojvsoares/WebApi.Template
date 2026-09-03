using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Seeding;
using Scrutor;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = PostgresOptions.FromConfiguration(configuration);

        services.AddSingleton(options);

        // Tables use snake_case columns while entities use PascalCase properties.
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        // Scoped to repositories and factories on purpose: migrations, the seeder and the
        // options type have no matching interface and are registered explicitly below.
        services.Scan(
            s => s
                .FromAssemblies(
                    AssemblyReference.Assembly)
                .AddClasses(
                    c => c.Where(type =>
                        type.Name.EndsWith("Repository", StringComparison.Ordinal) ||
                        type.Name.EndsWith("Factory", StringComparison.Ordinal)),
                    false)
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsMatchingInterface()
                .WithScopedLifetime());

        services.AddScoped<AdminUserSeeder>();
        services.AddScoped<PermissionConsistencyCheck>();

        services
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(options.ConnectionString)
                .ScanIn(AssemblyReference.Assembly).For.Migrations())
            .AddLogging(builder => builder.AddFluentMigratorConsole());

        return services;
    }
}
