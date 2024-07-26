using Core.Domain.GameManagementContext.Pipelines;
using Core.Domain.GameManagementContext.Events;
using Core.Domain.SignalRContext.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Core.Domain.OracleContext.Services;

namespace Core.Domain.GameManagementContext.Handlers;

public class CreateGameHandler : INotificationHandler<CreateGame>
{
    private readonly IHubContext<GameHub, IGameClient> _hubContext;
    private readonly IMediator _mediator;
    private readonly IOracleService _oracleService;

    private readonly IGameService _gameService;

    public CreateGameHandler(IMediator mediator, 
            IHubContext<GameHub, IGameClient> hubContext,
            IOracleService oracleService,
            IGameService gameService)
    {
        _mediator = mediator;
        _hubContext = hubContext;
        _oracleService = oracleService;
        _gameService = gameService;
    }

    public async Task Handle(CreateGame notification, CancellationToken cancellationToken)
    {   
        var listOfGuessers = await _gameService.CreateListOfGuessers(
                           notification.Users, notification.Oracle, 
                           notification.Options.GameMode.ToString(), 
                           notification.Options.UseAI);
        
        var numberOfRounds = notification.Options.NumberOfRounds;
        var chosenOracle = notification.Users.FirstOrDefault(u => u.Id == notification.Oracle) ?? throw new Exception("Chosen Oracle is not in the list of users");

        //******Create And Receive Id Oracle******//
        var OracleId = await _oracleService.CreateOracle(
                    notification.Options.UseAI, notification.ImageIdentifier, 
                    numberOfRounds, chosenOracle,
                    notification.Options.GameMode.ToString());
        
        var game = await _gameService.CreateGame(notification.SessionId, 
                listOfGuessers, notification.Options.GameMode.ToString(), 
                numberOfRounds, OracleId, notification.Options.UseAI);

        var result = await _mediator.Send(new AddGame.Request(game), cancellationToken);

        //***REDIRECT TO GAME PAGE***//
        if(result.Success)
            await _hubContext.Clients.Group(result.CreatedGame.SessionId.ToString()).RedirectToLink($"/Game/{result.CreatedGame.Id}");
    }
}