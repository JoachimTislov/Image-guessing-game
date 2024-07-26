using Core.Domain.GameManagementContext.Events;
using Core.Domain.StatisticsContext.Events;
using Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.StatisticsContext.Handlers;

// TODO: This handler has not been tested.
public class PlayerGuessedCorrectlyHandler : INotificationHandler<PlayerGuessedCorrectly>
{
    private readonly IMediator _mediator;
    private readonly UserManager<User> _userManager;

    public PlayerGuessedCorrectlyHandler(IMediator mediator, UserManager<User> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }
    public async Task Handle(PlayerGuessedCorrectly notification, CancellationToken cancellationToken)
    {
        /*if(notification.NumberOfGames <= 0)
        {
            switch(notification.GameMode)
            {
                case "FreeForAll":
                    await _mediator.Publish(new NewLeaderboardEntry(notification.GameMode, session.Options.RandomPictureMode, game.Score/notification.NumberOfGames, user.UserName, oracle.UserName), cancellationToken);
                    await _mediator.Publish(new NewRecentGameEntry(notification.GameMode, session.Options.RandomPictureMode, game.Score, guessers, oracle.UserName), cancellationToken);
                    break;
                case "Duo":
                    await _mediator.Publish(new NewLeaderboardEntry(notification.GameMode, session.Options.RandomPictureMode, game.Score/notification.NumberOfGames, user.UserName, oracle.UserName), cancellationToken);
                await _mediator.Publish(new NewRecentGameEntry(notification.GameMode, session.Options.RandomPictureMode, game.Score, guessers, oracle.UserName), cancellationToken);
                    break;
                case "SinglePlayer":
                    await _mediator.Publish(new NewLeaderboardEntry(notification.GameMode, session.Options.RandomPictureMode, game.Score/notification.NumberOfGames, user.UserName, null), cancellationToken);
                    await _mediator.Publish(new NewRecentGameEntry(notification.GameMode, session.Options.RandomPictureMode, game.Score, guessers, "AI"), cancellationToken);
                    break;
            }
        }*/
    }
}
            