using Core.Domain.StatisticsContext;
using Core.Domain.StatisticsContext.Events;
using Core.Domain.StatisticsContext.Pipelines;
using MediatR;

namespace Core.Domain.StatisticsContext.Handlers;

// TODO: This handler has not been tested.
public class NewLeaderboardEntryHandler : INotificationHandler<NewLeaderboardEntry>
{
    private readonly IMediator _mediator;

    public NewLeaderboardEntryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Handle(NewLeaderboardEntry notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handling new leaderboard entry");

        var leaderBoardEntry = new LeaderboardEntry
        {
            Guesser = notification.Guesser,
            GameMode = notification.GameMode,
            Score = notification.Score
        };
        if (notification.GameMode == "Duo" || notification.GameMode == "DuoRandom")
        {
            leaderBoardEntry.Oracle = notification.Oracle;
        }

        await _mediator.Send(new AddLeaderBoardEntry.Request(leaderBoardEntry));
    }
}