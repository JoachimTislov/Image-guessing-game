using Core.Domain.SessionContext;
using Core.Domain.UserContext;

namespace Core.Domain.OracleContext.Services
{
    public interface IOracleService
    {
        RandomNumbersAI CreateRandomNumbersAI(int PieceCountPerImage);

        Task<Guid> CreateOracle(bool OracleIsAI, string ImageIdentifier, 
                        int NumberOfRounds, User ChosenOracle, string GameMode);
        
        public record Response(bool IsGuessCorrect, string WinnerText);
        Task<Response> CheckGuess(string Guess, string ImageIdentifier, User User, Session Session);
    }
}