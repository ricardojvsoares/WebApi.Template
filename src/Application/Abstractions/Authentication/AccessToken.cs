namespace Application.Abstractions.Authentication;

public sealed record AccessToken(
    string Token,
    DateTime ExpiresAtUtc);
