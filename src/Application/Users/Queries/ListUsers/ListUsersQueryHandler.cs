using Application.Common;
using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.ListUsers;

public static class ListUsersQueryHandler
{
    public static async Task<ErrorOr<ListUsersResponse>> HandleAsync(
        ListUsersQuery query,
        ILogger logger,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var totalCount = await userRepository.CountAsync(
                cancellationToken);

            var users = await userRepository.ListAsync(
                PageRequest.ToSkip(query.Page, query.PageSize),
                query.PageSize,
                cancellationToken);

            List<ListUsersItem> items = [];

            foreach (var user in users)
            {
                var roles = await userRepository.GetRoleNamesAsync(
                    user.Id,
                    cancellationToken);

                items.Add(new ListUsersItem(
                    user.Id,
                    user.Email,
                    user.DisplayName,
                    user.IsActive,
                    roles,
                    user.CreatedAtUtc,
                    user.UpdatedAtUtc));
            }

            return new ListUsersResponse(
                items,
                query.Page,
                query.PageSize,
                totalCount);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(ListUsersQueryHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
