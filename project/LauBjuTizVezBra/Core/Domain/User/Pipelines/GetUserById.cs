using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.UserContext.Pipelines;

public class GetUserById
{
    public record Request(Guid UserId) : IRequest<User>;

    public class Handler : IRequestHandler<Request, User>
    {
        private readonly GameContext _db;
        private readonly IMediator _mediator;
        public Handler(GameContext db, IMediator mediator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
      
        public async Task<User> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _db.Users
                .Where(s => s.Id == request.UserId)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new Exception("Guesser not found");

            return user;
        }
    }
}