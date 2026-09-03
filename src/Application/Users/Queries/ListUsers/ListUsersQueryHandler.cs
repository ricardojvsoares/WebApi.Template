using Application.Common;
using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.ListUsers;

public static class ListUsersQueryHandler
{
    public static async Task<ErrorOr<PagedResponse<UserResponse>>> HandleAsync(
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

            List<UserResponse> items = [];

            foreach (var user in users)
            {
                var roles = await userRepository.GetRoleNamesAsync(
                    user.Id,
                    cancellationToken);

                items.Add(user.ToResponse(roles));
            }

            return new PagedResponse<UserResponse>(
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
