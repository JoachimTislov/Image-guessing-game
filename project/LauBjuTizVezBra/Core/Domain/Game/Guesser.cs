using Core.Domain.UserContext;
using SharedKernel;

namespace Core.Domain.GameManagementContext;

public class Guesser : BaseEntity
{
    public Guesser() { }

    public Guesser(User user) 
    {
        User = user;
    }
    public Guid Id  { get; set; }
    public User User { get; set; } = null!;
    public Guid GameId { get; set; }
    public int Points { get; set; }
    public TimeSpan Speed { get; set; }
    public int Guesses { get; set; }
    public int WrongGuessCounter { get; set; }
}
