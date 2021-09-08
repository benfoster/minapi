using System.Data;
using System.Net;
using Dapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace MinApi.Todos;

public class AddTodo
{
    public record Request : IRequest<IResult>
    {
        public Request(string title, bool completed, DateTime? completedOn)
        {
            Title = title;
            Completed = completed || completedOn.HasValue;

            if (Completed)
            {
                CompletedOn = completedOn ?? DateTime.UtcNow;
            }

            CreatedOn = DateTime.UtcNow;
        }

        public string Title { get; }
        public bool Completed { get; }
        public DateTime? CompletedOn { get; }
        public DateTime CreatedOn { get; }
    }

    public class Validator : AbstractValidator<AddTodo.Request>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("title_required");
        }
    }

    public class Handler : IRequestHandler<Request, IResult>
    {
        private readonly IValidator<Request> _validator;
        private readonly IDbConnection _db;

        public Handler(IValidator<Request> validator, IDbConnection db)
        {
            _validator = validator;
            _db = db;
        }

        public async Task<IResult> Handle(Request command, CancellationToken cancellationToken)
        {
            ValidationResult result = _validator.Validate(command);

            if (!result.IsValid)
            {
                return Results.ValidationProblem(
                    errors: result.ToDictionary(),
                    title: "Invalid Request",
                    statusCode: (int)HttpStatusCode.UnprocessableEntity,
                    type: "https://httpstatuses.com/422"
                );
            }
            
            string sql = @"
                INSERT INTO todos(title, completed, completed_on, created_on) 
                Values(@Title, @Completed, @CompletedOn, @CreatedOn) RETURNING * 
            ";

            var todoId = await _db.ExecuteScalarAsync<long>(sql, command);

            var response = new
            {
                id = todoId
            };

            return Results.CreatedAtRoute(nameof(GetTodo), response, response);
        }
    }
}
