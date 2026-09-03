namespace Application.Authentication.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string DisplayName);
