using Domain.Users.Entities;

namespace Application.Users;

internal static class UserMapping
{
    public static UserResponse ToResponse(
        this User user,
        IReadOnlyList<string> roles)
    {
        return new UserResponse(
            user.Id,
            user.Email,
            user.DisplayName,
            user.IsActive,
            roles,
            user.CreatedAtUtc,
            user.UpdatedAtUtc);
    }
}
