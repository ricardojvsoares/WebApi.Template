using Application.Todos;
using Application.Todos.Commands.CompleteTodo;
using Domain.Authorization;
using ErrorOr;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Todos.Endpoints;

/// <summary>
/// Completion is its own endpoint rather than a field on <see cref="UpdateTodo" />, so
/// marking a todo done does not require sending the whole record back.
/// </summary>
internal sealed class CompleteTodo
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/completion", HandleAsync)
            .HasApiVersion(1)
            .WithName("CompleteTodo")
            .WithSummary("Complete or reopen todo")
            .RequirePermission(TodoPermissions.Update)
            .Produces<TodoResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        CompleteTodoBody body,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        CompleteTodoCommand command = new(
            id,
            body.IsCompleted);

        var result = await bus.InvokeAsync<ErrorOr<TodoResponse>>(
            command,
            cancellationToken);

        return result.ToOk();
    }
}

internal sealed record CompleteTodoBody(
    bool IsCompleted);
