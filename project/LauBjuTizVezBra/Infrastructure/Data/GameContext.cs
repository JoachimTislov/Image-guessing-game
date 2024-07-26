using SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Core.Domain.UserContext;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Core.Domain.StatisticsContext;
using Core.Domain.SessionContext;
using Core.Domain.GameManagementContext;
using Core.Domain.ImageContext;
using Core.Domain.OracleContext;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Core.Domain.UserContext.Pipelines;

namespace Infrastructure.Data;

public class GameContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    private readonly IMediator _mediator;

    public GameContext(DbContextOptions<GameContext> configuration, IMediator mediator) : base(configuration)
    {
        _mediator = mediator;
    }

    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<Guesser> Guessers { get; set; } = null!;
    public DbSet<BaseOracle> Oracles { get; set; } = null!;
    public DbSet<ImageData> ImageRecords { get; set; } = null!;
    public DbSet<LeaderboardEntry> Leaderboards { get; set; } = null!;
    public DbSet<RecentGameEntry> RecentGames { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder Builder)
    { 
        // Creates a TPH type hierarchy for the Oracle table
        Builder.Entity<BaseOracle>()
            .HasDiscriminator<string>("OracleType")
            .HasValue<GenericOracle<User>>("User")
            .HasValue<GenericOracle<RandomNumbersAI>>("RandomNumbersAI");
        
        //Json converter for the different types of Oracles
        //Needed to store the data in the Oracle table
        Builder.Entity<GenericOracle<RandomNumbersAI>>()
        .Property(o => o.Oracle)
        .HasColumnName("RandomNumbersAI")
        .IsRequired(false)
        .HasConversion(
            v => JsonConvert.SerializeObject(v.NumbersForImagePieces),
            v => new RandomNumbersAI()
            {
                NumbersForImagePieces = JsonConvert.DeserializeObject<int[]>(v)
            });

        Builder.Entity<GenericOracle<User>>()
            .Property(o => o.Oracle)
            .IsRequired(false)
            .HasColumnName("UserInfo")
            .HasConversion(
            v => JsonConvert.SerializeObject(new { v.SessionId, v.Id, v.UserName }),
            v => JsonConvert.DeserializeObject<User>(v));

        base.OnModelCreating(Builder);
    }

   
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (_mediator == null) return result;

        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.Events.Any())
            .ToArray();
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.Events.ToArray();
            entity.Events.Clear();
            foreach (var domainEvent in events)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
        return result;
    }

    public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

}

internal class FakeLeaderboardData
{
    public static LeaderboardEntry[] leaderboardEntries { get; private set; } = Array.Empty<LeaderboardEntry>();

    internal static void Init()
    {
        var userNames = new List<string> { "Karl", "Jarle", "Frank", "Lars", "Connor", "Laila", "Emilie", "Lisa", "Harvey", "Mike", "Joey", "Natalie", "Jonas", "Henriette"};
        var gameModes = new List<string> { "SinglePlayer", "SinglePlayerRandom", "Duo", "DuoRandom" };
        var random = new Bogus.Randomizer();
        leaderboardEntries = Enumerable.Range(1, 80).Select(i => 
        {
            var guesserIndex = random.Number(0, userNames.Count - 1);
            var entry = new LeaderboardEntry()
            {
                Guesser = userNames[guesserIndex],
                GameMode = gameModes[random.Number(0, gameModes.Count - 1)]
            };
            if (entry.GameMode != "SinglePlayerRandom" && entry.GameMode != "SinglePlayer")
            {
                var oracleIndex = random.Number(0, userNames.Count - 1);
                if (oracleIndex == guesserIndex)
                {
                    oracleIndex = (oracleIndex == userNames.Count - 1) ? oracleIndex - 1 : oracleIndex + 1;
                }
                entry.Oracle = userNames[oracleIndex];
            }
            entry.Score = random.Number(350, 1000);
            return entry;

        }).ToArray();
    }
}

internal class FakeRecentGameData
{
    public static RecentGameEntry[] RecentGames { get; private set; } = Array.Empty<RecentGameEntry>();

    internal static void Init()
    {
        var userNames = new List<string> { "Connor", "Lisa", "Harvey", "Mike", "Natalie", "Jonas", "Henriette"};
        var gameModes = new List<string> { "SinglePlayer", "SinglePlayerRandom", "Duo", "DuoRandom" };
        var random = new Bogus.Randomizer();        

        var userName = "Karl";
        // This is staticly Karl's Guid if the Database change, this Guid is invalid and will cause problems,
        // Should not be a problem in production
        var userId = Guid.Parse("AD6FC1DD-FDCD-4431-BDC6-8D5333CD6EDF");
        
        RecentGames = Enumerable.Range(1, 80).Select(i =>
        {
            var recentGame = new RecentGameEntry()
            {
                GuesserId = userId,
                Guesser = userName,
                GameMode = gameModes[random.Number(0, gameModes.Count - 1)],
                Score = random.Number(350, 1000)
            };

            if (recentGame.GameMode != "SinglePlayerRandom" && recentGame.GameMode != "SinglePlayer")
            {
                var oracleIndex = random.Number(0, userNames.Count - 1);
                recentGame.Oracle = userNames[oracleIndex];

                // Guid.NewGuid() is used strictly for the mock data
                // All names used for testing accounts have been removed to not cause conflicts with accounts having the same UserName
                recentGame.OracleId = Guid.NewGuid();
            }
            return recentGame;
        }).ToArray();
    }
}