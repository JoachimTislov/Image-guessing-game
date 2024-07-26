using Core.Domain.UserContext.Pipelines;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.GameManagementContext.Pipelines;

public class GetGuesserByUserId
{
    public record Request(Guid Id, Guid GameId) : IRequest<Guesser>;

    public class Handler : IRequestHandler<Request, Guesser>
    {
        private readonly GameContext _db;
        private readonly IMediator _mediator;

        public Handler(GameContext db, IMediator mediator) {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Guesser> Handle(Request request, CancellationToken cancellationToken)
        {
            //*********GuesserId and GameId forms a unique key*********////
            var guesser = await _db.Guessers
                .Where(g => g.User.Id == request.Id && g.GameId == request.GameId)
                .SingleOrDefaultAsync(cancellationToken);
            
            if(guesser == null)
            {
                var user = await _mediator.Send(new GetUserById.Request(request.Id), cancellationToken);
                var NewGuesser = await _mediator.Send(new CreateAndAddGuesserToDatabase.Request(user, request.GameId), cancellationToken);
                return NewGuesser;
            }
            
            return guesser;
        }
    }
}