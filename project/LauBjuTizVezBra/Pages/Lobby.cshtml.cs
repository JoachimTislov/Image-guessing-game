using Core.Domain.SessionContext;
using Core.Domain.SessionContext.Pipelines;
using Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Domain.SignalRContext.Hubs;
using Microsoft.AspNetCore.SignalR;
using Core.Domain.GameManagementContext.Events;
using Core.Domain.ImageContext;
using Core.Domain.ImageContext.Pipelines;

namespace LauBjuTizVezBra.Pages;

[Authorize]
public class LobbyModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly UserManager<User> _userManager;
    private readonly IHubContext<GameHub, IGameClient> _hubContext;

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }
    public Session? Session { get; set; }
    public User? Player  { get; set; }

    [BindProperty]
    public int NumberOfRounds { get; set; } 

    [BindProperty]
    public int LobbySize { get; set; }

    [BindProperty]
    public GameMode GameMode { get; set; } 

    [BindProperty]
    public bool RandomPictureMode { get; set; }

    [BindProperty]
    public bool RandomOracle { get; set; } 

    [BindProperty]
    public string ImageIdentifier { get; set; } = null!;

    [BindProperty]
    public string NewOracle { get; set; } = null!;

    [BindProperty]
    public bool UseAI { get; set; }

    [BindProperty]
    public string ChosenImageName { get; set; } = null!;

    public List<ImageData> RandomImages { get; set; } = Array.Empty<ImageData>().ToList();

    public LobbyModel(IMediator mediator, UserManager<User> userManager, IHubContext<GameHub, IGameClient> hubContext)
    {
        _mediator = mediator;
        _userManager = userManager;
        _hubContext = hubContext;
    }

    public async Task OnGetAsync()
    {
        var UserIdentity = User.Identity;
        if(UserIdentity != null) 
        if (UserIdentity.IsAuthenticated && Id != Guid.Empty)  
        {
            string? UserId = _userManager.GetUserId(User);

            if(UserId != null)
            Player = await _userManager.FindByIdAsync(UserId);

            Session = await _mediator.Send(new GetSessionById.Request(Id));

            //Used to add imageData to the database
            //********REMOVE THIS LATER*********////////
            //await _mediator.Send(new DeleteImageRecords.Request());
            //await _mediator.Send(new AddAllMappedImagesToDatabase.Request());

            int amountOfImages = 9; // Decides how many images are shown in the lobby
            for(var i = 0; i < amountOfImages; i++)
            {
                RandomImages.Add(await _mediator.Send(new GetRandomImage.Request()));
            }
        }
    }

    public async Task OnPostUpdateSettingsAsync()
    {
        string? UserId = _userManager.GetUserId(User);
        
        if(UserId != null)
        Player = await _userManager.FindByIdAsync(UserId);
        
        if (Id != Guid.Empty)
        {
            // Updating the session in the database to make sure that the most recent changes are reflected
            Session = await _mediator.Send(new EditSession.Request(
                            Id, NumberOfRounds, LobbySize, GameMode, 
                            RandomPictureMode, RandomOracle, UseAI, 
                            ImageIdentifier, NewOracle, ChosenImageName));

            if(Session != null) 
            await _hubContext.Clients.Group(Session.Id.ToString()).RedirectToLink($"/Lobby/{Session.Id}");
        }
    }

    public async Task OnPostStartGameAsync()
    {
        Session = await _mediator.Send(new GetSessionById.Request(Id));
        
        // If the session is not null, publish the CreateGame event
        // This Event is then handled by the CreateGameHandler 
        // In both SessionContext and GameManagementContext
        // GameManagementContext is responsible for creating the game

        if(Session != null)
        await _mediator.Publish(new CreateGame(Session));
    }
}