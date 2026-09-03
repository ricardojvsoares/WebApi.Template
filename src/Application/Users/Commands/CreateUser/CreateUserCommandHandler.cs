using Application.Abstractions.Security;
using Domain.Users.Entities;
using Domain.Users.Repositories;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.CreateUser;

public static class CreateUserCommandHandler
{
    public static async Task<ErrorOr<UserResponse>> HandleAsync(
        CreateUserCommand command,
        ILogger logger,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (await userRepository.EmailExistsAsync(command.Email, cancellationToken))
            {
                return Error.Conflict(description: "An account with that email already exists.");
            }

            var requestedRoles = command.Roles ?? [];
            List<Role> roles = [];

            // Resolve every requested role before writing anything, so a typo in one role
            // name does not leave a user created with a partial set of roles.
            foreach (var roleName in requestedRoles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var role = await roleRepository.GetByNameAsync(
                    roleName,
                    cancellationToken);

                if (role is null)
                {
                    return Error.Validation(
                        code: "Roles",
                        description: $"The role '{roleName}' does not exist.");
                }

                roles.Add(role);
            }

            var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

            var user = User.Create(
                command.Email.Trim(),
                passwordHasher.Hash(command.Password),
                command.DisplayName.Trim(),
                nowUtc);

            await userRepository.AddAsync(
                user,
                cancellationToken);

            foreach (var role in roles)
            {
                await roleRepository.AssignToUserAsync(
                    user.Id,
                    role.Id,
                    cancellationToken);
            }

            return user.ToResponse([.. roles.Select(r => r.Name)]);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred in '{Name}'. Error: {Message}",
                nameof(CreateUserCommandHandler),
                ex.Message);

            return Error.Unexpected(description: ex.Message);
        }
    }
}
