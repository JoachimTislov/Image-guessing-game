using Core.Domain.ImageContext.Pipelines;
using Infrastructure.Data;
using MediatR;

namespace Core.Domain.SessionContext.Pipelines;

public class EditSession
{
    public record Request(
        Guid Id, 
        int NumberOfRounds, 
        int LobbySize, 
        GameMode GameMode, 
        bool RandomPictureMode, 
        bool RandomOracle, 
        bool UseAI,
        string ImageIdentifier, 
        string NewOracle,
        string ChosenImageName) : IRequest<Session?>;

    public class Handler : IRequestHandler<Request, Session?>
    {
        private readonly GameContext _db;
        private readonly IMediator _mediator;
        public Handler(GameContext db, IMediator mediator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mediator = mediator;
        }

        public async Task<Session?> Handle(Request request, CancellationToken cancellationToken)
        {
            var session = await _mediator.Send(new GetSessionById.Request(request.Id), cancellationToken);

            if(session != null) {

                // Temporarily store the old gameMode so that in case of a mismatch between player count and lobby size we can restore the old gameMode
                var OldGameMode = session.Options.GameMode;
                session.Options.GameMode = request.GameMode;

                // Switch to ensure that the count of players is in correlation with the gameMode selected
                // ----- GameMode Logic -----
                switch (request.GameMode)
                {
                    case GameMode.SinglePlayer:
                        if (session.SessionUsers.Count > 1)
                        {
                            session.Options.GameMode = OldGameMode;
                            break;
                        }
                        session.Options.LobbySize = 1;
                        break;
                    case GameMode.Duo:
                        if (session.SessionUsers.Count > 2)
                        {
                            session.Options.GameMode = OldGameMode;
                            break;
                        }
                        session.Options.LobbySize = 2;
                        session.ChosenOracle = session.SessionHostId;
                        break;
                    case GameMode.FreeForAll:
                        if (session.SessionUsers.Count > request.LobbySize)
                        {
                            session.Options.LobbySize = session.SessionUsers.Count;
                            break;
                        }
                        session.Options.LobbySize = request.LobbySize;
                        session.ChosenOracle = session.SessionHostId;
                        break;
                }

                // ----- Options -----
                session.Options.NumberOfRounds = request.NumberOfRounds;
                session.Options.RandomPictureMode = request.RandomPictureMode;
                session.Options.RandomOracle = request.RandomOracle;
                session.Options.UseAI = request.UseAI;

                // ----- Oracle Logic -----
                if (request.NewOracle != null) session.ChosenOracle = Guid.Parse(request.NewOracle);

                // ----- Image Logic -----
                if (!request.RandomPictureMode){
                    session.ImageIdentifier = request.ImageIdentifier;
                }
                else
                {
                    session.ImageIdentifier = null;
                }

                if(session.ImageIdentifier != null)
                {
                    var imageData = await _mediator.Send(new GetImageDataByIdentifier.Request(session.ImageIdentifier), cancellationToken);
                
                    session.ChosenImageName = imageData.Name;
                }
                

            }
            await _db.SaveChangesAsync(cancellationToken);

            return session;
        }
    }
}