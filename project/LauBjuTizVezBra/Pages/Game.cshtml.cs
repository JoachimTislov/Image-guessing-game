using Core.Domain.GameManagementContext;
using Core.Domain.GameManagementContext.Pipelines;
using Core.Domain.ImageContext;
using Core.Domain.ImageContext.Pipelines;
using Core.Domain.OracleContext;
using Core.Domain.OracleContext.Pipelines;
using Core.Domain.SessionContext.Pipelines;
using Core.Domain.SessionContext;
using Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Domain.ImageContext.Services;
using Newtonsoft.Json;

namespace LauBjuTizVezBra.Pages;

[Authorize]
public class GameModel : PageModel 
{
    private readonly IMediator _mediator;
    private readonly UserManager<User> _userManager;
    private readonly IImageService _imageService;
    private readonly IWebHostEnvironment _hostingEnvironment;
    
    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;
    public User? Player { get; set; } 
    public Guesser Guesser { get; set; } = null!;
    public Session Session { get; set; } = null!;
    public ImageData ImageData { get; set; } = null!;
    public GenericOracle<RandomNumbersAI>? OracleAI { get; set; }
    public GenericOracle<User>? UserOracle { get; set; } 
    public BaseOracle BaseOracle { get; set; } = null!;

    //Check if this thing is used correctly
    public List<string> ImagePieceList { get; set; } = new List<string>();
    public string InitialImage { get; set; } = null!;

    public GameModel(IMediator mediator, UserManager<User> userManager, 
            IWebHostEnvironment hostingEnvironment, IImageService imageService)
    {
        _mediator = mediator;
        _userManager = userManager;
        _imageService = imageService;
        _hostingEnvironment = hostingEnvironment;
    }

    public async Task<IActionResult> OnGetAsync()
    {   
        var UserIdentity = User.Identity;
        if(UserIdentity != null)
        if(UserIdentity.IsAuthenticated)
        {
            Console.WriteLine("Enters here during OnGet");
            var UserId = _userManager.GetUserId(User);

            if(UserId != null)
            Player = await _userManager.FindByIdAsync(UserId);

            Game = await _mediator.Send(new GetGameById.Request(GameId));

            Session = await _mediator.Send(new GetSessionById.Request(Game.SessionId));
        
            if(Player != null && Player.UserName != null)
            Guesser = await _mediator.Send(new GetGuesserByUserId.Request(Player.Id, GameId));

            if(Game.OracleIsAI)
            {
                OracleAI = await _mediator.Send(new GetOracleById<RandomNumbersAI>.Request(Game.OracleId));
            }
            else
            {
                UserOracle = await _mediator.Send(new GetOracleById<User>.Request(Game.OracleId));
            }

            // This is made to change common values for generic oracles
            // I do not have to check if it is AI or User when I want to change the values
            BaseOracle = await _mediator.Send(new GetBaseOracleById.Request(Game.OracleId));
            
            //Like the Identifier for chosen or RandomImage
            ImageData = await _mediator.Send(new GetImageDataByIdentifier.Request(BaseOracle.ImageIdentifier));

            ImagePieceList = _imageService.GetFileNameOfImagePieces(ImageData.Identifier);

            ViewData["ImagePieceList"] = JsonConvert.SerializeObject(ImagePieceList);

            var imagePiecesFolderPath = Path.Combine(_hostingEnvironment.ContentRootPath, ImageData.FolderWithImagePiecesLink);
            var _imageCoordinates = _imageService.GetCoordinatesForImagePieces(imagePiecesFolderPath, ImagePieceList);

            // Used in gamePage.js, linked through scripts file in game.cshtml
            ViewData["ImagePiecesData"] = JsonConvert.SerializeObject(_imageCoordinates);
            
            if(OracleAI != null)
            {
                ViewData["oracleAI_Array_NumbersForImagePieces"] = JsonConvert.SerializeObject(OracleAI.Oracle.NumbersForImagePieces); 
            }
            else
            {
                ViewData["oracleAI_Array_NumbersForImagePieces"] = JsonConvert.SerializeObject(null);
            }
            
          
           
            return Page(); 
        }
        return RedirectToPage("/Index");   
    }
}
