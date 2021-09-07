using System.Data;
using Dapper;
using MediatR;

namespace MinApi.Todos;

public class GetTodo
{
    public record Query(long Id) : IRequest<IResult>, IIdCommand<Query>
    {
        public static Query Create(long id) => new(id);
    }

    public class Handler : IRequestHandler<Query, IResult>
    {
        private readonly IDbConnection _db;

        public Handler(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IResult> Handle(Query query, CancellationToken cancellationToken)
        {
            string sql = "SELECT * FROM todos WHERE id = @Id";

            return await _db.QueryFirstOrDefaultAsync<Todo>(sql, query) is Todo todo
                ? Results.Ok(todo)
                : Results.NotFound();
        }
    }
}
