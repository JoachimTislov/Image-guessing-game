using SharedKernel;

namespace Core.Domain.StatisticsContext.Events;

public record NewLeaderboardEntry : BaseDomainEvent
{
    public NewLeaderboardEntry(string gameMode, bool randomPicture, int score, string guesser, string oracle)
    {
        GameMode = gameMode;
        if (randomPicture)
        {
            GameMode = gameMode + "Random";
        }
        Score = score;
        Guesser = guesser;
        Oracle = oracle;
    }

    public string GameMode { get; set; }
    public int Score { get; set; }
    public string Guesser { get; set; }
    public string Oracle { get; set; }
}