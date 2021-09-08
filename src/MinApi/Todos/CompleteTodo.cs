using System.Data;
using Dapper;
using MediatR;

namespace MinApi.Todos;

public class CompleteTodo
{
    public record Request : IRequest<IResult>, IIdRequest
    {
        public long Id { get; init; }
        public DateTime CompletedOn { get; } = DateTime.UtcNow;
    }

    public class Handler : IRequestHandler<Request, IResult>
    {
        private readonly IDbConnection _db;

        public Handler(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IResult> Handle(Request command, CancellationToken cancellationToken)
        {
            string sql = @"UPDATE todos SET completed = true, completed_on = @CompletedOn WHERE id = @Id";

            return await _db.ExecuteAsync(sql, command) == 1
                ? Results.NoContent()
                : Results.NotFound();
        }
    }
}
