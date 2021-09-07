using MediatR;
using MinApi.Todos;

public static class TodoEndpoints
{
    public static void MapTodo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => "Hello World!");

        endpoints.MapGet("/todos/{id}", async (long id, IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(new GetTodo.Command(id), cancellationToken)
        )
        .WithName(nameof(GetTodo));

        endpoints.MapPost("/todos", async (AddTodo.Command command, IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(command, cancellationToken)
        )
        .WithName(nameof(AddTodo));
    }
}
