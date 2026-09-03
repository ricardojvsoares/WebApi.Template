using Application.Common;
using Application.Todos;
using Application.Todos.Queries.ListTodos;
using Domain.Authorization;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;
using WebApi.Extensions;
using Wolverine;

namespace WebApi.Features.Todos.Endpoints;

internal sealed class ListTodos
    : IEndpoint
{
    public static void Map(
        IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandleAsync)
            .HasApiVersion(1)
            .WithName("ListTodos")
            .WithSummary("List todos")
            .RequirePermission(TodoPermissions.Read)
            .Produces<PagedResponse<TodoResponse>>()
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken,
        [FromQuery] int page = PageRequest.DefaultPage,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize,
        [FromQuery] bool? isCompleted = null)
    {
        ListTodosQuery query = new(
            page,
            pageSize,
            isCompleted);

        var result = await bus.InvokeAsync<ErrorOr<PagedResponse<TodoResponse>>>(
            query,
            cancellationToken);

        return result.ToOk();
    }
}
