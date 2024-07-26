using SharedKernel;

namespace Core.Domain.StatisticsContext.Events;

public record NewRecentGameEntry : BaseDomainEvent
{
    public NewRecentGameEntry(string gameMode, bool randomPicture, int score, Guid[] guesserIds, string oracle)
    {
        GameMode = gameMode;
        if (randomPicture)
        {
            GameMode = (gameMode == "FreeForAll") ? gameMode : gameMode + "Random";
        }
        Score = score;
        Oracle = oracle;
        GuesserIds = guesserIds;
    }

    public string GameMode { get; set; }
    public int Score { get; set; }
    public Guid[] GuesserIds { get; set; }
    public string Oracle { get; set; }
}