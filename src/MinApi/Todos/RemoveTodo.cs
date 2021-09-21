using System.Data;
using Dapper;
using MediatR;

namespace MinApi.Todos;

public class RemoveTodo
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
            _db = db.NotNull(nameof(db));
        }

        public async Task<IResult> Handle(Request command, CancellationToken cancellationToken)
        {
            string sql = @"DELETE FROM todos WHERE id = @Id";

            return await _db.ExecuteAsync(sql, command) == 1
                ? Results.NoContent()
                : Results.NotFound();
        }
    }
}
