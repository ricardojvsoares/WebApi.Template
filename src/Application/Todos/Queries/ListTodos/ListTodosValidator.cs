using Application.Common;
using FluentValidation;

namespace Application.Todos.Queries.ListTodos;

internal sealed class ListTodosValidator
    : AbstractValidator<ListTodosQuery>
{
    public ListTodosValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(PageRequest.MaxPageSize);
    }
}
