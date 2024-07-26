using Core.Domain.SessionContext;
using Core.Domain.SessionContext.Pipelines;
using Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LauBjuTizVezBra.Pages;

[Authorize]
public class LobbyListModel : PageModel
{
    public Dictionary<Session, User> GameHosts = new();

    private readonly IMediator _mediator;
    private readonly UserManager<User> _userManager;

    public LobbyListModel(IMediator mediator, UserManager<User> userManager) 
    {
        _mediator = mediator;
        _userManager = userManager;
    }
 
    public async Task<IActionResult> OnGetAsync()
    {
        var sessionList = await _mediator.Send(new GetAllSessions.Request());

        foreach (var session in sessionList)
        {
            var user = await _userManager.FindByIdAsync(session.SessionHostId.ToString());
            if(user != null && session.SessionStatus == SessionStatus.Lobby) GameHosts.Add(session, user);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostJoinSessionAsync(Guid GameId)
    {
        Console.WriteLine("Joining session with Id: " + GameId);
        var UserIdentity = User.Identity;
        if(UserIdentity != null)
            if (UserIdentity.IsAuthenticated)
            {
                var gameSession = await _mediator.Send(new GetSessionById.Request(GameId));

                if(gameSession != null)
                    return RedirectToPage($"/Lobby/{gameSession.Id}");
            }
            return RedirectToPage("Index");
    }
}
