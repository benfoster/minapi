using System.Reflection;
using MediatR;
using MinApi.Todos;

public static class TodoEndpoints
{
    public static void MapTodo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/todos", Dispatch.Request<AddTodo.Request>)
            .WithName(nameof(AddTodo));

        endpoints.MapGet("/todos", Dispatch.Request<ListTodos.Request>)
            .WithName(nameof(GetTodo));

        endpoints.MapGet("/todos/{id}", Dispatch.RequestWithId<GetTodo.Request>)
            .WithName(nameof(GetTodo));

        endpoints.MapDelete("/todos/{id}", Dispatch.RequestWithId<RemoveTodo.Request>)
            .WithName(nameof(RemoveTodo));

        endpoints.MapPut("/todos/{id}/complete", Dispatch.RequestWithId<CompleteTodo.Request>)
            .WithName(nameof(CompleteTodo));
    }
}

public static class Dispatch
{
    public static Task<IResult> Request<TRequest>(TRequest request, IMediator mediator, CancellationToken cancellationToken)
        where TRequest : IRequest<IResult>
            => mediator.Send(request, cancellationToken);

    public static Task<IResult> RequestWithId<TRequest>(long id, IMediator mediator, CancellationToken cancellationToken)
        where TRequest : IRequest<IResult>, IIdRequest, new()
            => mediator.Send(new TRequest { Id = id }, cancellationToken);
}

/// <summary>
/// Interface for requests with an ID that should be bound from the route value
/// </summary>
public interface IIdRequest
{
    long Id { get; init; }
}

// Credit: https://github.com/DamianEdwards/MinimalApiPlayground
public interface IExtensionBinder<TSelf> where TSelf : IExtensionBinder<TSelf>
{
    static abstract ValueTask<TSelf?> BindAsync(HttpContext context, ParameterInfo parameter);
}
