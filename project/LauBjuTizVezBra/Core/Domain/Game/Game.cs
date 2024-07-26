using SharedKernel;

namespace Core.Domain.GameManagementContext;

public class Game : BaseEntity 
{
    public Game() { }
    
    public Game(
        Guid sessionId,
        List<Guesser> guessers, 
        string gameMode,
        int numberOfGames,
        Guid oracleId,
        bool oracleIsAI) 
    {
        SessionId = sessionId;
        Guessers = guessers;
        GameMode = gameMode;
        NumberOfGames = numberOfGames;
        OracleId = oracleId;
        OracleIsAI = oracleIsAI;
    }
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Guid OracleId { get; set; } 
    public List<Guesser> Guessers { get; set; } = new();
    public string GameMode { get; set; } = null!;
    public int NumberOfGames { get; set; } 
    public bool OracleIsAI { get; set; } 
    public GameStatus GameStatus { get; set; } = GameStatus.Started;
    public DateTime Timer { get; set; } = DateTime.Now;
}