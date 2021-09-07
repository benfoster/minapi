using MinApi.Todos;

public static class TodoEndpoints
{
    public static void MapTodo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/todos", Dispatch.Command<AddTodo.Command>)
            .WithName(nameof(AddTodo));

        endpoints.MapGet("/todos", Dispatch.Query<ListTodos.Query>)
            .WithName(nameof(GetTodo));

        endpoints.MapGet("/todos/{id}", Dispatch.CommandById<GetTodo.Query>)
            .WithName(nameof(GetTodo));

        endpoints.MapDelete("/todos/{id}", Dispatch.CommandById<RemoveTodo.Command>)
            .WithName(nameof(RemoveTodo));

        endpoints.MapPut("/todos/{id}/complete", Dispatch.CommandById<CompleteTodo.Command>)
            .WithName(nameof(CompleteTodo));
    }
}
