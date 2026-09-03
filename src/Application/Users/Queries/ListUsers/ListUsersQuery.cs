using Application.Common;

namespace Application.Users.Queries.ListUsers;

public sealed record ListUsersQuery(
    int Page = PageRequest.DefaultPage,
    int PageSize = PageRequest.DefaultPageSize);
