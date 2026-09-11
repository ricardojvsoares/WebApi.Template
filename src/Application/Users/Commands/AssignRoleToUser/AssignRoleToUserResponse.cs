namespace Application.Users.Commands.AssignRoleToUser;

public sealed record AssignRoleToUserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    bool IsActive,
    IReadOnlyList<string> Roles,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
