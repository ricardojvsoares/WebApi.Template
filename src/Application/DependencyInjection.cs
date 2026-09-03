using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // includeInternalTypes because the validators are internal: they are an
        // implementation detail of each use case, not part of the layer's public surface.
        // Without this they are silently skipped and nothing is ever validated.
        services.AddValidatorsFromAssembly(
            typeof(AssemblyReference).Assembly,
            includeInternalTypes: true);

        // Handlers take the clock as a dependency rather than calling DateTime.UtcNow, so
        // time-dependent behaviour stays controllable from a test.
        services.AddSingleton(TimeProvider.System);

        services.Scan(
            s => s
                .FromAssemblies(AssemblyReference.Assembly)
                .AddClasses(false)
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsMatchingInterface()
                .WithScopedLifetime());

        return services;
    }
}
