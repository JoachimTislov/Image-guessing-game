
using Core.Domain.GameManagementContext.Events;
using Core.Domain.UserContext;

namespace Core.Domain.GameManagementContext.Services;

public class GameService : IGameService 
{
    public Task<Game> CreateGame(Guid SessionId,List<Guesser> listOfGuessers, string GameMode, 
                                            int numberOfRounds, Guid OracleId, bool UseAI)
    {  
        var game = new Game(SessionId, listOfGuessers, GameMode, numberOfRounds, OracleId, UseAI);
       
        return Task.FromResult(game);
    }

    public Task<List<Guesser>> CreateListOfGuessers(List<User> Users, Guid? OracleId, string GameMode, bool OracleIsAI)
    {
        var guessers = new List<Guesser>();

        if(OracleIsAI)
        {
            foreach (var user in Users)
            {
                var guesser = new Guesser(user);
                guessers.Add(guesser);
            }
            return Task.FromResult(guessers);
        }
        else
        {
            foreach (var user in Users)
            {
                if(user.Id != OracleId)
                {
                    var guesser = new Guesser(user);
                    guessers.Add(guesser);
                }
            }
            return Task.FromResult(guessers);
        }
    }
}

  
