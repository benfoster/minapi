using MediatR;

namespace MinApi.Todos;

public class GetTodo
{
    public record Command(long Id) : IRequest<IResult>;

    public class Handler : IRequestHandler<Command, IResult>
    {
        public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
        {
            return Results.Ok();
        }
    }
}
