using System.Security.Claims;
using Application.Abstractions.Authentication;
using Infrastructure.Authentication;

namespace WebApi.Authorization;

/// <summary>
/// Exposes the authenticated caller to application handlers. Lives in the API layer
/// because it is the only place that legitimately knows about <see cref="HttpContext" />.
/// </summary>
internal sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor)
    : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var subject = httpContextAccessor.HttpContext?.User
                .FindFirstValue(AuthenticationClaims.Subject);

            return Guid.TryParse(subject, out var userId)
                ? userId
                : null;
        }
    }

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public bool HasPermission(
        string permission)
    {
        return httpContextAccessor.HttpContext?.User
            .HasClaim(AuthenticationClaims.Permission, permission) ?? false;
    }
}
