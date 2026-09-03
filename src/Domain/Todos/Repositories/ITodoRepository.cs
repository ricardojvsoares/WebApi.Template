using Domain.Todos.Entities;

namespace Domain.Todos.Repositories;

public interface ITodoRepository
{
    Task<Todo?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a page of todos, optionally narrowed to a single owner or completion state.
    /// </summary>
    Task<IReadOnlyList<Todo>> ListAsync(
        Guid? ownerUserId,
        bool? isCompleted,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Guid? ownerUserId,
        bool? isCompleted,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Todo todo,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Todo todo,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
