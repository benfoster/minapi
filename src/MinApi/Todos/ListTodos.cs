using System.Data;
using System.Reflection;
using Dapper;
using MediatR;

namespace MinApi.Todos;

public class ListTodos
{
    public record Query(bool IncludeCompleted, int Page, int PageSize)
        : IRequest<IResult>, IExtensionBinder<Query>
    {
        public static ValueTask<Query?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            bool.TryParse(context.Request.Query["include-completed"], out bool includeCompleted);
            int.TryParse(context.Request.Query["page"], out int page);
            int.TryParse(context.Request.Query["page-size"], out int pageSize);
            
            return ValueTask.FromResult<ListTodos.Query?>(new(
                includeCompleted,
                page == 0 ? 1 : page,
                pageSize == 0 ? 10 : pageSize
            ));
        }
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
