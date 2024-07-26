using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.GameManagementContext.Pipelines;

public class GetGuesserById
{
    public record Request(Guid GuesserId) : IRequest<Guesser>;

    public class Handler : IRequestHandler<Request, Guesser>
    {
        private readonly GameContext _db;
        private readonly IMediator _mediator;
        public Handler(GameContext db, IMediator mediator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
      
        public async Task<Guesser> Handle(Request request, CancellationToken cancellationToken)
        {
            var guesser = await _db.Guessers
                .Where(s => s.Id == request.GuesserId)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new Exception("Guesser not found");

            return guesser;
        }
    }
}