namespace Application.Todos.Queries.ListTodos;

public sealed record ListTodosItem(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDateUtc,
    DateTime? CompletedAtUtc,
    Guid OwnerUserId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed record ListTodosResponse(
    IReadOnlyList<ListTodosItem> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => PageSize <= 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
