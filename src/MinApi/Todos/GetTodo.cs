using System.Data;
using Dapper;
using MediatR;

namespace MinApi.Todos;

public class GetTodo
{
    public record Command(long Id) : IRequest<IResult>;

    public class Handler : IRequestHandler<Command, IResult>
    {
        private readonly IDbConnection _db;

        public Handler(IDbConnection db)
        {
            _db = db;
        }        
        
        public async Task<IResult> Handle(Command command, CancellationToken cancellationToken)
        {
            string sql = "SELECT * FROM todos WHERE id = @id";

            return await _db.QueryFirstAsync<Todo>(sql, new { id = command.Id }) is Todo todo
                ? Results.Ok(todo)
                : Results.NotFound();
        }
    }
}
