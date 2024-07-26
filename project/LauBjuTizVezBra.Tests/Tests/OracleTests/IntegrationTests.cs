using Core.Domain.OracleContext;
using Core.Domain.OracleContext.Services;
using Core.Domain.UserContext;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LauBjuTizVezBra.Tests;

public class IntegrationTests
{        
    public IMediator Mediator { get; } = null!;

    public IOracleService OracleService { get; } = null!;
    
    
    [Fact]
    public async void CreatingAndAddingGenericOracleToDatabaseAndReadingOracle()
    {
        var options = new DbContextOptionsBuilder<GameContext>()
            .UseInMemoryDatabase(databaseName: "GameContext")
            .Options;

        using var _db = new GameContext(options, Mediator);

        var OracleAI = OracleService.CreateRandomNumbersAI(45);
        
        var GenericOracle = new GenericOracle<RandomNumbersAI>(OracleAI);

        var serialize = JsonConvert.SerializeObject(GenericOracle);
        var Oracle1 = JsonConvert.DeserializeObject<GenericOracle<RandomNumbersAI>>(serialize);

        _db.Oracles.Add(Oracle1);
        await _db.SaveChangesAsync();

        var Oracle = await _db.Oracles
        .Where(uo => uo.Id == GenericOracle.Id)
        .Select(uo => new
        {
            NumbersForImagePieces = ((GenericOracle<RandomNumbersAI>)uo).Oracle.NumbersForImagePieces,
            UserName = ((GenericOracle<User>)uo).Oracle.UserName
        }).SingleOrDefaultAsync();

        Assert.NotNull(Oracle);

        Assert.Null(Oracle.UserName);

        Console.WriteLine(Oracle.UserName);
        Console.WriteLine(Oracle.NumbersForImagePieces);
        Console.WriteLine(OracleAI.NumbersForImagePieces);

        Assert.Equal(OracleAI.NumbersForImagePieces, Oracle.NumbersForImagePieces);
    }
}