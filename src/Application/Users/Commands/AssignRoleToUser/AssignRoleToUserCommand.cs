namespace Application.Users.Commands.AssignRoleToUser;

public sealed record AssignRoleToUserCommand(
    Guid UserId,
    string RoleName);
