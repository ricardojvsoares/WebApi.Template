namespace Application.Authentication;

public sealed record AuthenticationResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    string TokenType = "Bearer");
