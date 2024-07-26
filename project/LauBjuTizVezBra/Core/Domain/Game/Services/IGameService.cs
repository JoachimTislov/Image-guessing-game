
using Core.Domain.UserContext;

namespace Core.Domain.GameManagementContext;

public interface IGameService 
{
    Task<Game> CreateGame(Guid SessionId, List<Guesser> listOfGuessers, string GameMode, 
                                            int numberOfRounds, Guid OracleId, bool UseAI);  

    Task<List<Guesser>> CreateListOfGuessers(List<User> Users, Guid? OracleId, string GameMode, bool OracleIsAI);
}