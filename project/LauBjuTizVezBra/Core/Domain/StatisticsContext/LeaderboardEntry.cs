using Core.Domain.UserContext;
using SharedKernel;

namespace Core.Domain.StatisticsContext;

public class LeaderboardEntry : BaseEntity
{
    public LeaderboardEntry () { }
    public LeaderboardEntry (string gameMode, int score, string guesser, string oracle)
    {
        GameMode = gameMode;
        Score = score;
        Guesser = guesser;
        Oracle = oracle;
    }

    public Guid Id { get; protected set; }
    public string GameMode { get; set; } = null!;
    public int Score { get; set; }
    public string Guesser { get; set; } = null!;
    public string? Oracle { get; set; }
}