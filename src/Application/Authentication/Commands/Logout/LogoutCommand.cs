namespace Application.Authentication.Commands.Logout;

/// <summary>
/// Revokes every refresh token held by the caller, signing them out everywhere.
/// Already-issued access tokens stay valid until they expire.
/// </summary>
public sealed record LogoutCommand;
