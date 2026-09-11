namespace Application.Authentication.Queries.GetCurrentUser;

public sealed record GetCurrentUserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
