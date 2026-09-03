using Application.Todos.Commands.DeleteTodo;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Todos.Endpoints;

internal sealed class DeleteTodo
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", HandleAsync)
            .HasApiVersion(1)
            .WithName("DeleteTodo")
            .WithSummary("Delete todo")
            .RequirePermission(TodoPermissions.Delete)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        DeleteTodoCommand command = new(id);

        var result = await bus.InvokeAsync<ErrorOr<Deleted>>(
            command,
            cancellationToken);

        return result.ToNoContent();
    }
}
