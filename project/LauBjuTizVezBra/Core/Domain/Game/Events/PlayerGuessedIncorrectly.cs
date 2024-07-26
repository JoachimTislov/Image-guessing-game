using SharedKernel;

namespace Core.Domain.GameManagementContext.Events;

public record PlayerGuessedIncorrectly : BaseDomainEvent
{

    // A multiplayer game is not finished until all players have guessed correctly
    // I think 
    public PlayerGuessedIncorrectly(Guid guesserId)
    {
        GuesserId = guesserId;
    }
    public Guid GuesserId { get; }
}