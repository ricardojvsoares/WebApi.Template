using Asp.Versioning;
using Carter;
using WebApi.Extensions;
using WebApi.Features.Todos.Endpoints;

namespace WebApi.Features.Todos;

public sealed class TodosModule
    : ICarterModule
{
    private const string ModuleName = "Todos";
    private const string RoutePrefix = "api/v{apiVersion:apiVersion}/todos";

    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .Build();

        var group = app.MapGroup(RoutePrefix)
            .WithApiVersionSet(versionSet)
            .WithTags(ModuleName);

        group
            .MapEndpoint<CreateTodo>()
            .MapEndpoint<ListTodos>()
            .MapEndpoint<GetTodoById>()
            .MapEndpoint<UpdateTodo>()
            .MapEndpoint<CompleteTodo>()
            .MapEndpoint<DeleteTodo>();
    }
}
