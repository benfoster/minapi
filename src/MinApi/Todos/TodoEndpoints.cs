using MediatR;
using MinApi.Todos;

public static class TodoEndpoints
{
    public static void MapTodo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/todos", Dispatch.Command<AddTodo.Command>)
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

        endpoints.MapGet("/todos/{id}", async (long id, IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(new GetTodo.Query(id), cancellationToken)
        )
        .WithName(nameof(GetTodo));

        endpoints.MapDelete("/todos/{id}", async (long id, IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(new RemoveTodo.Command(id), cancellationToken)
        )
        .WithName(nameof(RemoveTodo));

        endpoints.MapPut("/todos/{id}/complete", async (long id, IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(new CompleteTodo.Command(id), cancellationToken)
        )
        .WithName(nameof(CompleteTodo));
    }
}
