using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.SessionContext.Pipelines;

public class CheckForAndDeletingEmptySessions
{
    public record Request(List<Session> Sessions) : IRequest<bool>;

    public class Handler : IRequestHandler<Request, bool>
    {
        private readonly GameContext _db;
        private readonly IMediator _mediator;

        public Handler(GameContext db, IMediator mediator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mediator = mediator;
        } 

        public async Task<bool> Handle(Request request, CancellationToken cancellationToken)
        {
            var emptySessionsCount = 0;
            foreach (var session in request.Sessions)
            {
                Console.WriteLine("SessionId: " + session.Id);
               
                var users = await _db.Users.Where(u => u.SessionId == session.Id).ToListAsync(cancellationToken: cancellationToken);

                 // Checking this instead of the sessionUsers because the list is not correctly updated for some reason
                if (users.Count == 0 /*session.SessionUsers.Count == 0*/)
                {
                    emptySessionsCount++;
                    Console.WriteLine("Session had: " + session.SessionUsers.Count + " users");
                    try
                    {
                        await _mediator.Send(new DeleteSession.Request(session.Id), cancellationToken);

                        emptySessionsCount--;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"Error deleting session with Id {session.Id}: {e.Message}");
                    }
                }
            }
            return emptySessionsCount == 0;
        }
    }
}