using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.GameManagementContext.Pipelines;

public class GetGameById
{
    public record Request(Guid GameId) : IRequest<Game>;

    public class Handler : IRequestHandler<Request, Game>
    {
        private readonly GameContext _db;
        public Handler(GameContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<Game> Handle(Request request, CancellationToken cancellationToken)
        {
            var game = await _db.Games
                .Where(s => s.Id == request.GameId)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new Exception("Game not found");

            return game;
        }
    }
}