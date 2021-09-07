using MediatR;
using MinApi.Todos;

public static class TodoEndpoints
{
    public static void MapTodo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/todos", Dispatch.FromBody<AddTodo.Command>)
            .WithName(nameof(AddTodo));

        endpoints.MapGet("/todos", async (HttpRequest httpRequest, IMediator mediator, CancellationToken cancellationToken) =>
        {
            bool.TryParse(httpRequest.Query["include-completed"], out bool includeCompleted);
            int.TryParse(httpRequest.Query["page"], out int page);
            int.TryParse(httpRequest.Query["page-size"], out int pageSize);
            
            var query = new ListTodos.Query(
                includeCompleted,
                page == 0 ? 1 : page,
                pageSize == 0 ? 10 : pageSize
            );

            return await mediator.Send(query, cancellationToken);
        })
        .WithName(nameof(GetTodo));

        endpoints.MapGet("/todos/{id}", Dispatch.FromRouteWithId<GetTodo.Query>)
            .WithName(nameof(GetTodo));

        endpoints.MapDelete("/todos/{id}", Dispatch.FromRouteWithId<RemoveTodo.Command>)
            .WithName(nameof(RemoveTodo));

        endpoints.MapPut("/todos/{id}/complete", Dispatch.FromRouteWithId<CompleteTodo.Command>)
            .WithName(nameof(CompleteTodo));
    }
}
