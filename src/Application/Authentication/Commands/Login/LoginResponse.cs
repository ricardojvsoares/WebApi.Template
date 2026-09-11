namespace Application.Authentication.Commands.Login;

public sealed record LoginResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    string TokenType = "Bearer");
