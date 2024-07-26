using SharedKernel;

namespace Core.Domain.OracleContext;

// Values common for all generic Oracles
public class BaseOracle : BaseEntity
{
    public Guid Id { get; set; }
    public int TotalGuesses { get; set; }
    public int NumberOfTilesRevealed { get; set; }
    public string ImageIdentifier { get; set; } = null!;
    public BaseOracle(){}
    public void AssignImageId(string imageIdentifier)
    {
        ImageIdentifier= imageIdentifier;
    }
}
