using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WebApi.OpenApi;

/// <summary>
/// Declares the bearer scheme on the OpenAPI document so Scalar shows an Authorize box
/// and sends the token on subsequent calls.
/// </summary>
internal sealed class BearerSecuritySchemeTransformer
    : IOpenApiDocumentTransformer
{
    private const string SchemeName = "Bearer";

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>(StringComparer.Ordinal);

        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Paste the access token returned by /api/v1/auth/login."
        };

        return Task.CompletedTask;
    }
}
