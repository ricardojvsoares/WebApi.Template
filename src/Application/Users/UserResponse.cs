namespace Application.Users;

public sealed record UserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    bool IsActive,
    IReadOnlyList<string> Roles,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
