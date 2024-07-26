using Core.Domain.UserContext;
using SharedKernel;

namespace Core.Domain.StatisticsContext;

public class RecentGameEntry : BaseEntity
{
    public RecentGameEntry () { }
    public RecentGameEntry (string gameMode, int score, string guesser, string oracle, Guid guesserId)
    {
        GameMode = gameMode;
        Score = score;
        Guesser = guesser;
        GuesserId = guesserId;
        Oracle = oracle;
        Time = DateTime.UtcNow;
    }

    // GameMode GameMode { get; set; }
    public Guid Id { get; protected set; }
    public Guid? GuesserId { get; set; }
    public string Guesser { get; set; } = null!;
    public Guid? OracleId { get; set; }
    public string? Oracle { get; set; }
    public string GameMode { get; set; } = null!;
    public int Score { get; set; }
    public DateTime Time { get; set; }
}