namespace Application.Authentication.Commands.RefreshAccessToken;

public sealed record RefreshAccessTokenResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    string TokenType = "Bearer");
