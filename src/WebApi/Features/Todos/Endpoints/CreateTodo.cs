using Application.Todos;
using Application.Todos.Commands.CreateTodo;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Todos.Endpoints;

internal sealed class CreateTodo
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandleAsync)
            .HasApiVersion(1)
            .WithName("CreateTodo")
            .WithSummary("Create todo")
            .RequirePermission(TodoPermissions.Create)
            .Produces<TodoResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        CreateTodoCommand command,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<ErrorOr<TodoResponse>>(
            command,
            cancellationToken);

        return result.ToCreated(todo => $"/api/v1/todos/{todo.Id}");
    }
}
