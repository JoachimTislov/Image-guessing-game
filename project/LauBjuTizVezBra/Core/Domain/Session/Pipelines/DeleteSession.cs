using Core.Exceptions;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.SessionContext;

public class DeleteSession
{
    public record Request(Guid SessionId) : IRequest;

    public class Handler : IRequestHandler<Request>
    {
        private readonly GameContext _db;

        public Handler(GameContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var users = await _db.Users.Where(u => u.SessionId == request.SessionId).ToListAsync(cancellationToken: cancellationToken);
            var session = await _db.Sessions.SingleOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken) ?? throw new EntityNotFoundException("Session with ID " + request.SessionId + " could not be found");

            // Done to make sure that all the foreign keys are removed before deleting the session
            if(users != null && users.Count > 0) {
                foreach (var user in users)
                {
                    Console.WriteLine("Kicking " + user.UserName + " From DeleteSession Pipeline");
                    user.SessionId = null;
                }
                await _db.SaveChangesAsync(cancellationToken);
            } else {
                Console.WriteLine("No users to kick, continuing with deleting session");
            }
            
            _db.Sessions.Remove(session);
            await _db.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}