using System.Data;
using System.Reflection;
using Dapper;
using FluentValidation;
using MediatR;
using MinApi.Validation;

namespace MinApi.Todos;

public class ListTodos
{
    public record Request(bool IncludeCompleted, int Page, int PageSize)
        : IRequest<IResult>, IExtensionBinder<Request>, IValidateable
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            bool.TryParse(context.Request.Query["include-completed"], out bool includeCompleted);
            int.TryParse(context.Request.Query["page"], out int page);
            int.TryParse(context.Request.Query["page-size"], out int pageSize);
            
            return ValueTask.FromResult<ListTodos.Request?>(new(
                includeCompleted,
                page == 0 ? 1 : page,
                pageSize == 0 ? 10 : pageSize
            ));
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Page).GreaterThan(0).WithMessage("page_invalid");
            RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("page_size_invalid");
        }
    }

    public class Handler : IRequestHandler<Request, IResult>
    {
        private readonly IDbConnection _db;

        public Handler(IDbConnection db)
        {
            _db = db.NotNull(nameof(db));
        }        
        
        public async Task<IResult> Handle(Request query, CancellationToken cancellationToken)
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
