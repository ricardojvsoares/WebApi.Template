using Application.Todos;
using Application.Todos.Commands.UpdateTodo;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Todos.Endpoints;

internal sealed class UpdateTodo
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", HandleAsync)
            .HasApiVersion(1)
            .WithName("UpdateTodo")
            .WithSummary("Update todo")
            .RequirePermission(TodoPermissions.Update)
            .Produces<TodoResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdateTodoBody body,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        UpdateTodoCommand command = new(
            id,
            body.Title,
            body.Description,
            body.DueDateUtc);

        var result = await bus.InvokeAsync<ErrorOr<TodoResponse>>(
            command,
            cancellationToken);

        return result.ToOk();
    }
}

// Route supplies Id; body must not share a generic OpenAPI schema name like Request/Body.
internal sealed record UpdateTodoBody(
    string Title,
    string? Description,
    DateTime? DueDateUtc);
