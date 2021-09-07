using System.Data;
using Dapper;
using MediatR;

namespace MinApi.Todos;

public class ListTodos
{
    public record Query(bool IncludeCompleted, int Page, int PageSize) : IRequest<IResult>;

    public class Handler : IRequestHandler<Query, IResult>
    {
        private readonly IDbConnection _db;

        public Handler(IDbConnection db)
        {
            _db = db;
        }        
        
        public async Task<IResult> Handle(Query query, CancellationToken cancellationToken)
        {
            string sql = @"
                SELECT * FROM todos 
                WHERE (@IncludeCompleted = true OR completed = false)
                ORDER BY created_on ASC
                LIMIT @PageSize OFFSET ((@Page - 1) * @PageSize)
            ";

            var todos = await _db.QueryAsync<Todo>(sql, query);
            return Results.Ok(todos);
        }
    }
}
