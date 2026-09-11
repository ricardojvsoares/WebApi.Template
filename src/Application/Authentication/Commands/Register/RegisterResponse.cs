namespace Application.Authentication.Commands.Register;

public sealed record RegisterResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    string TokenType = "Bearer");
