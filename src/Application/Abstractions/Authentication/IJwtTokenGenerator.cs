using Domain.Users.Entities;

namespace Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    /// <summary>
    /// Issues an access token carrying one permission claim per granted permission, so
    /// authorization decisions need no database round trip.
    /// </summary>
    AccessToken CreateAccessToken(
        User user,
        IReadOnlyList<string> permissions,
        IReadOnlyList<string> roles);
}
