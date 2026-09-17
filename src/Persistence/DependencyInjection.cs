using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;
using Persistence.Seeding;
using Persistence.TypeHandlers;
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

        // EF Core is registered for migrations / Database.Migrate only.
        // Runtime CRUD goes through Dapper repositories.
        services.AddDbContext<AppDbContext>(builder => builder
            .UseNpgsql(options.ConnectionString)
            .UseSnakeCaseNamingConvention());

        // Tables use snake_case columns while entities use PascalCase properties.
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        SqlMapper.AddTypeHandler(new UriTypeHandler());

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

        return services;
    }
}
