using Application.Common;

namespace Application.Todos.Queries.ListTodos;

public sealed record ListTodosQuery(
    int Page = PageRequest.DefaultPage,
    int PageSize = PageRequest.DefaultPageSize,
    bool? IsCompleted = null);
