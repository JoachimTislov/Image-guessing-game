using Core.Domain.ImageContext.Pipelines;
using Core.Domain.StatisticsContext;
using Core.Domain.StatisticsContext.Pipelines;
using Core.Domain.SessionContext;
using Core.Domain.SessionContext.Pipelines;
using Core.Domain.SessionContext.Services;
using Core.Domain.SignalRContext.Pipelines;
using Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LauBjuTizVezBra.Pages;

public class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ISessionService _sessionService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    public record Leaderboard(string GameMode, List<LeaderboardEntry> Entries);
    public List<LeaderboardEntry> SinglesLeaderboards = new();
    public List<LeaderboardEntry> SinglesRandomLeaderboards = new();
    public List<LeaderboardEntry> DuoLeaderboards = new();
    public List<LeaderboardEntry> DuoRandomLeaderboards = new();
    public List<Leaderboard> Leaderboards = new();
    public List<RecentGameEntry> RecentGames = new();

    [BindProperty]
    public string Username { get; set; } = null!;

    [BindProperty]
    public string Password { get; set; } = null!;

    [BindProperty]
    public bool RememberMe { get; set; }

    [BindProperty]
    public User? GameUser { get; set; } = null!;

    public string[] Errors { get; set; } = Array.Empty<string>();
    
    public IndexModel(IMediator mediator, SignInManager<User> signInManager, 
        ISessionService sessionService, UserManager<User> userManager)
    {
        _mediator = mediator;
        _signInManager = signInManager;
        _sessionService = sessionService;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGet()
    {
        if (User.Identity.IsAuthenticated) 
        {
            string? gameUserId = _userManager.GetUserId(User);

            if(gameUserId != null)
            GameUser = await _userManager.FindByIdAsync(gameUserId);

            // Checking if the user is already in a session with a group and removing them from that group
            if(GameUser != null)
            {
                await _mediator.Send(new RemovePlayersOnDisconnectAction.Request(GameUser.Id, GameUser));
            }
        }

        GetLeaderboards();

        Leaderboards.Add(new Leaderboard("SinglePlayer", SinglesLeaderboards));
        Leaderboards.Add(new Leaderboard("SinglePlayerRandom", SinglesRandomLeaderboards));
        Leaderboards.Add(new Leaderboard("Duo", DuoLeaderboards));
        Leaderboards.Add(new Leaderboard("DuoRandom", DuoRandomLeaderboards));

        return Page();
    }

    public async void GetLeaderboards() 
    {
        SinglesLeaderboards = await _mediator.Send(new GetLeaderboard.Request(GameModeEnum.SinglePlayer));
        SinglesRandomLeaderboards = await _mediator.Send(new GetLeaderboard.Request(GameModeEnum.SinglePlayerRandom));
        DuoLeaderboards = await _mediator.Send(new GetLeaderboard.Request(GameModeEnum.Duo));
        DuoRandomLeaderboards = await _mediator.Send(new GetLeaderboard.Request(GameModeEnum.DuoRandom));
    }
    
    public async Task<IActionResult> OnPostLogoutAsync()
    {
        // This is needed to load the leaderboard, RIGHT ?
        GetLeaderboards();
        await _signInManager.SignOutAsync();
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        GetLeaderboards();
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(Username, Password, RememberMe, lockoutOnFailure: false);

            if (result.Succeeded) return RedirectToPage("Index");
            
            Errors = new string[] { "Invalid Password or Username" };
        }
        return Page();
    }

    public async Task<IActionResult> OnPostCreateGameAsync()
    {
        var gameUser = new User();

        var UserIdentity = User.Identity;
        if(UserIdentity != null)
        if (UserIdentity.IsAuthenticated)  
        {
            string? gameUserId = _userManager.GetUserId(User);

            if(gameUserId != null)
            gameUser = await _userManager.FindByIdAsync(gameUserId);
            
            if(gameUser != null) {
                var result = await _sessionService.CreateSession(gameUser);

                if (result.Success) 
                {
                    return RedirectToPage("/Lobby", new { result.CreatedSession.Id });
                }
            }
        }

        return RedirectToPage("Index");
    }

    public IActionResult OnPostJoinGameAsync()
    {
        var UserIdentity = User.Identity;
        if(UserIdentity != null)
        if (UserIdentity.IsAuthenticated) 
        {
            return RedirectToPage("/LobbyList");
        }

        return RedirectToPage("Index");
    }
}
