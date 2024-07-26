using Core.Domain.OracleContext;
using Core.Domain.OracleContext.Services;
using Core.Domain.UserContext;

namespace LauBjuTizVezBra.Tests;

public class UnitTests
{
    private readonly IOracleService _oracleService = null!;

    [Fact]
    public void OracleAI_WithInt()
    {
        var OracleAI = _oracleService.CreateRandomNumbersAI(45);

        Assert.NotNull(OracleAI.NumbersForImagePieces);
        Assert.Equal(45, OracleAI.NumbersForImagePieces.Length);
    }

    [Fact]
    public void OracleAI_WithZero()
    {
        var OracleAI = _oracleService.CreateRandomNumbersAI(0);

        Assert.Empty(OracleAI.NumbersForImagePieces);
    }

    [Fact]
    public void GenericOracleAI()
    {
        var OracleAI = _oracleService.CreateRandomNumbersAI(45);

        var GenericOracle = new GenericOracle<RandomNumbersAI>(OracleAI);
        
        Assert.Equal(OracleAI, GenericOracle.Oracle);
        Assert.Equal(OracleAI.NumbersForImagePieces, GenericOracle.Oracle.NumbersForImagePieces);
    }

    [Fact]
    public void GenericOracleAI_WithNull()
    {
        var GenericOracle = new GenericOracle<RandomNumbersAI>(null);

        Assert.Null(GenericOracle.Oracle);
    }


    [Fact]
    public void GenericOracleUser()
    {
        var User = new User("Toby");

        var GenericOracle = new GenericOracle<User>(User);

        Assert.Equal(User, GenericOracle.Oracle);
        Assert.Equal(User.UserName, GenericOracle.Oracle.UserName);
    }

    [Fact]
    public void GenericOracleUser_WithNull()
    {
        var GenericOracle = new GenericOracle<User>(null);

        Assert.Null(GenericOracle.Oracle);
    }

    [Fact]
    public void OracleAI_ShuffleArray()
    {
        var OracleAI = _oracleService.CreateRandomNumbersAI(45);
        
        var originalArrayCopy = OracleAI.NumbersForImagePieces.ToArray();

        var shuffledArray = ShuffleArray(originalArrayCopy);


        static int[] ShuffleArray(int[] ArrayOfNumbers)
        {   
            Random random = new();
            int LengthOfArray = ArrayOfNumbers.Length;

            while (LengthOfArray > 1)
            {
                int randomNumber = random.Next(LengthOfArray--);

                (ArrayOfNumbers[randomNumber], ArrayOfNumbers[LengthOfArray]) = (ArrayOfNumbers[LengthOfArray], ArrayOfNumbers[randomNumber]);
            }
            return ArrayOfNumbers;
        }

        Assert.Equal(OracleAI.NumbersForImagePieces.Length, shuffledArray.Length);
        Assert.NotEqual(OracleAI.NumbersForImagePieces, originalArrayCopy);
        Assert.NotEqual(OracleAI.NumbersForImagePieces, shuffledArray);
    }
}