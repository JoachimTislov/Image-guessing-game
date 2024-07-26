using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.UserContext.Pipelines;

public class GetUserByName
{
    public record Request(string UserName) : IRequest<User>;

    public class Handler : IRequestHandler<Request, User>
    {
        private readonly GameContext _db;
        public Handler(GameContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
      
        public async Task<User> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _db.Users
                .Where(s => s.UserName == request.UserName)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new Exception("User not found");

            return user;
        }
    }
}