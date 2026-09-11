namespace Application.Authentication;

internal sealed record TokenIssueResult(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken);
