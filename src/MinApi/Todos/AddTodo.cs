using System.Data;
using Dapper;
using MediatR;

namespace MinApi.Todos;

public class AddTodo
{
    public record Command(string Title, bool Completed) : IRequest<IResult>;

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
                INSERT INTO todos(title, completed) Values(@title, @completed) RETURNING * 
            ";

            var todoId = await _db.ExecuteScalarAsync<long>(sql, new
            {
                title = command.Title,
                completed = command.Completed
            });

            var response = new {
                id = todoId
            };

            return Results.CreatedAtRoute(nameof(GetTodo), response, response);
        }
    }
}
