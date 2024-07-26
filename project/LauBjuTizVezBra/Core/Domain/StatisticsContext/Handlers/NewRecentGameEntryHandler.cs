using Core.Domain.StatisticsContext;
using Core.Domain.StatisticsContext.Events;
using Core.Domain.StatisticsContext.Pipelines;
using Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.StatisticsContext.Handlers;

// TODO: This handler has not been tested.
public class NewRecentGameEntryHandler : INotificationHandler<NewRecentGameEntry>
{
    private readonly IMediator _mediator;
    private readonly UserManager<User> _userManager;

    public NewRecentGameEntryHandler(IMediator mediator, UserManager<User> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }
    public async Task Handle(NewRecentGameEntry notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handling New Recent Game Entry");
        Console.WriteLine(notification.GuesserIds.Length);
        foreach (var guesserId in notification.GuesserIds)
        {
            var guesser = await _userManager.FindByIdAsync(guesserId.ToString());

            var recentGameEntry = new RecentGameEntry()
            {
                Guesser = guesser.UserName,
                GuesserId = guesserId,
                GameMode = notification.GameMode,
                Score = notification.Score
            };
            if (notification.GameMode == "Duo" || notification.GameMode == "DuoRandom" || notification.GameMode == "FreeForAll")
            {
                recentGameEntry.Oracle = notification.Oracle;
            }

            await _mediator.Send(new AddRecentGameEntry.Request(recentGameEntry), cancellationToken);
        }

    }
}