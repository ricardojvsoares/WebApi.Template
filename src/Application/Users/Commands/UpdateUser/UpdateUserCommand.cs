namespace Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string DisplayName,
    bool IsActive);
