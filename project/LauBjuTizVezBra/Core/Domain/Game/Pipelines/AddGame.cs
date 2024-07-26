using Infrastructure.Data;
using MediatR;

namespace Core.Domain.GameManagementContext.Pipelines;

public class AddGame
{
    public record Request(Game Game) : IRequest<Response>;

    public record Response(bool Success, Game CreatedGame, string[] Errors);

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly GameContext _db;
    
        public Handler(GameContext db)
            => _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            _db.Games.Add(request.Game);
            await _db.SaveChangesAsync(cancellationToken);

            return new Response(true, request.Game, Array.Empty<string>());
        }
    }
}