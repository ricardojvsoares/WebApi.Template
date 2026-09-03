namespace Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    string DisplayName,
    IReadOnlyList<string>? Roles);
