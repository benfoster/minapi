using MinApi.Todos;

public static class TodoEndpoints
{
    public static void MapTodo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/todos", Dispatch.Command<AddTodo.Command>)
            .WithName(nameof(AddTodo));

        endpoints.MapGet("/todos", Dispatch.Query<ListTodos.Query>)
            .WithName(nameof(GetTodo));

        endpoints.MapGet("/todos/{id}", Dispatch.QueryWithId<GetTodo.Query>)
            .WithName(nameof(GetTodo));

        endpoints.MapDelete("/todos/{id}", Dispatch.CommandWithId<RemoveTodo.Command>)
            .WithName(nameof(RemoveTodo));

        endpoints.MapPut("/todos/{id}/complete", Dispatch.CommandWithId<CompleteTodo.Command>)
            .WithName(nameof(CompleteTodo));
    }
}
