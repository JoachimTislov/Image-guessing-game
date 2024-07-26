using Microsoft.EntityFrameworkCore;

namespace Core.Domain.SessionContext;


[Owned]
public class Options
{
    public Options() { }
    
    public int NumberOfRounds { get; set; } = 1;
    public int LobbySize { get; set; } = 1;
    public GameMode GameMode { get; set; } = GameMode.SinglePlayer;
    public bool RandomPictureMode { get; set; } = true;
    public bool RandomOracle { get; set; } = false;
    public bool UseAI { get; set; } = true;
}