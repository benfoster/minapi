using System.Data;
using Dapper;
using MediatR;

namespace MinApi.Todos;

public class GetTodo
{
    public record Request : IRequest<IResult>, IIdRequest
    {
        public long Id { get; init; }
    }

    public class Handler : IRequestHandler<Request, IResult>
    {
        private readonly IDbConnection _db;

        public Handler(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IResult> Handle(Request query, CancellationToken cancellationToken)
        {
            string sql = "SELECT * FROM todos WHERE id = @Id";

            return await _db.QueryFirstOrDefaultAsync<Todo>(sql, query) is Todo todo
                ? Results.Ok(todo)
                : Results.NotFound();
        }
    }
}
