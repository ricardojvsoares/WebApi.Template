using System.Text;
using Application;
using Application.Abstractions.Authentication;
using Application.Todos.Events;
using Asp.Versioning;
using Carter;
using Infrastructure;
using Infrastructure.Authentication;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using Scalar.AspNetCore;
using Serilog;
using WebApi.Authorization;
using WebApi.Middlewares;
using WebApi.OpenApi;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (context, configuration) =>
        configuration.ReadFrom.Configuration(
            context.Configuration));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
        options.AssumeDefaultVersionWhenUnspecified = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// Read eagerly so a missing or too-short signing key fails startup rather than the
// first login attempt.
var jwtOptions = builder.Configuration.ReadValidatedJwtOptions();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Keep the short claim names the token was issued with instead of rewriting them
        // to the long WS-Federation URIs.
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = AuthenticationClaims.Name,
            RoleClaimType = AuthenticationClaims.Role
        };
    });

builder.Services.AddAuthorization();

// Policies for `action:resource` names are built on demand, so a new permission needs no
// registration here.
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

var rabbitMqOptions = builder.Configuration
    .GetSection(RabbitMqOptions.SectionName)
    .Get<RabbitMqOptions>() ?? new RabbitMqOptions();

builder.Host.UseWolverine(options =>
{
    options.Discovery.IncludeAssembly(Application.AssemblyReference.Assembly);

    // Activates the FluentValidation validators, which previously existed but never ran.
    // ExplicitRegistration because AddApplication already registers them from the
    // assembly; the default would scan and register a second copy of each.
    options.UseFluentValidation(RegistrationBehavior.ExplicitRegistration);

    // Integration events travel through the broker rather than being handled in the same
    // process that published them. Left on, conventional local routing would also invoke
    // the consumer inline and every event would be handled twice.
    options.Policies.DisableConventionalLocalRouting();

    if (rabbitMqOptions.Enabled)
    {
        options
            .UseRabbitMq(factory =>
            {
                factory.HostName = rabbitMqOptions.Host;
                factory.Port = rabbitMqOptions.Port;
                factory.UserName = rabbitMqOptions.Username;
                factory.Password = rabbitMqOptions.Password;
                factory.VirtualHost = rabbitMqOptions.VirtualHost;
            })
            .AutoProvision();

        options
            .PublishMessage<TodoCreatedEvent>()
            .ToRabbitExchange(
                rabbitMqOptions.TodosExchange,
                exchange => exchange.BindQueue(rabbitMqOptions.TodosQueue, "todo-created"));

        options.ListenToRabbitQueue(rabbitMqOptions.TodosQueue);
    }
});

builder.Services
    .AddPersistence(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddCarter();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<UnhandledExceptionHandler>();

var app = builder.Build();

// Applies pending migrations when enabled, seeds the bootstrap administrator, and
// verifies the seeded permissions still match the constants in code.
await DatabaseStartup.InitializeAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/{documentName}.json");

    app.MapScalarApiReference("/docs", options =>
    {
        options.WithTitle("WebApi Reference");
        options.WithTheme(ScalarTheme.DeepSpace);
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
    });
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();

await app.RunAsync();
