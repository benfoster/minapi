using System.Data;
using Dapper;
using MediatR;

namespace MinApi.Todos;

public class AddTodo
{
    public record Command : IRequest<IResult>
    {
        public Command(string title, bool completed, DateTime? completedOn)
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

    public class Handler : IRequestHandler<Command, IResult>
    {
        private readonly IDbConnection _db;

        public Handler(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IResult> Handle(Command command, CancellationToken cancellationToken)
        {
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
