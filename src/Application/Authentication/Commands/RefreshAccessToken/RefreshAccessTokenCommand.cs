namespace Application.Authentication.Commands.RefreshAccessToken;

public sealed record RefreshAccessTokenCommand(
    string RefreshToken);
