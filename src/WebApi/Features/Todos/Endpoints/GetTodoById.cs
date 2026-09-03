using Application.Todos;
using Application.Todos.Queries.GetTodoById;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Todos.Endpoints;

internal sealed class GetTodoById
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", HandleAsync)
            .HasApiVersion(1)
            .WithName("GetTodoById")
            .WithSummary("Get todo")
            .RequirePermission(TodoPermissions.Read)
            .Produces<TodoResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        GetTodoByIdQuery query = new(id);

        var result = await bus.InvokeAsync<ErrorOr<TodoResponse>>(
            query,
            cancellationToken);

        return result.ToOk();
    }
}
