namespace Application.Abstractions.Authentication;

public interface ICurrentUser
{
    /// <summary>
    /// Identifier of the authenticated caller, or null on an anonymous request.
    /// </summary>
    Guid? UserId { get; }

    bool IsAuthenticated { get; }

    bool HasPermission(
        string permission);
}
