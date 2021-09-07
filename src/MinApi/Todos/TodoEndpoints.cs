using MediatR;
using MinApi.Todos;

public static class TodoEndpoints
{
    public static void MapTodo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/todos", async (AddTodo.Command command, IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(command, cancellationToken)
        )
        .WithName(nameof(AddTodo));

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
