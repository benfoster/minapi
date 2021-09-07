using System.Data;
using Dapper;
using MediatR;

namespace MinApi.Todos;

public class RemoveTodo
{
    public record Command(long Id) : IRequest<IResult>;

    public class Handler : IRequestHandler<Command, IResult>
    {
        private readonly IDbConnection _db;

        public Handler(IDbConnection db)
        {
            _db = db;
        }        
        
        public async Task<IResult> Handle(Command command, CancellationToken cancellationToken)
        {
            string sql = @"DELETE FROM todos WHERE id = @Id";

            return await _db.ExecuteAsync(sql, command) == 1
                ? Results.NoContent()
                : Results.NotFound();
        }
    }
}
