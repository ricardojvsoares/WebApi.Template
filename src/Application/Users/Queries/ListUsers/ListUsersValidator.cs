using Application.Common;
using FluentValidation;

namespace Application.Users.Queries.ListUsers;

internal sealed class ListUsersValidator
    : AbstractValidator<ListUsersQuery>
{
    public ListUsersValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(PageRequest.MaxPageSize);
    }
}
