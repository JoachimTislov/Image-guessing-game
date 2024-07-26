
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.OracleContext;

// OracleAI is a class that stores an array of integers,
// which are used to determine which image pieces/tiles the AI should reveal

[Keyless]
public class RandomNumbersAI 
{
    public int[] NumbersForImagePieces { get; set; } = null!;
    public RandomNumbersAI(){}
    public RandomNumbersAI(int[] numbersForImagePieces)
    {
        NumbersForImagePieces = numbersForImagePieces; 
    }
}

