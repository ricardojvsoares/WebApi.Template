using Domain.Todos.Entities;
using ErrorOr;

namespace Application.Todos;

/// <summary>
/// Todos are owned records. A permission grants the right to perform an action at all;
/// ownership decides which instances that action may touch. Callers therefore need both
/// the matching <c>*:todo</c> permission and ownership of the todo.
/// </summary>
internal static class TodoAccess
{
    /// <summary>
    /// Resolves a todo for the given owner, or the error to return. Reports a missing todo
    /// and someone else's todo identically, so the endpoint does not leak which ids exist.
    /// </summary>
    public static ErrorOr<Todo> ForOwner(
        Todo? todo,
        Guid ownerUserId)
    {
        if (todo is null || todo.OwnerUserId != ownerUserId)
        {
            return Error.NotFound(description: "The todo was not found.");
        }

        return todo;
    }
}
