using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Core.Domain.UserContext;
using Core.Domain.SessionContext.Services;
using Core.Domain.SignalRContext.Hubs;
using Core.Domain.OracleContext.Services;
using Core.Domain.OracleContext;
using Core.Domain.OracleContext.Pipelines;
using Core.Domain.ImageContext.Services;
using Core.Domain.SignalRContext.Services;
using Core.Domain.GameManagementContext;
using Core.Domain.GameManagementContext.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => 
{
    options.IdleTimeout = TimeSpan.FromSeconds(60);
    options.Cookie.Name = "cookies";
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// Add DI to builders
builder.Services.AddTransient<IOracleService, OracleService>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<IGameService, GameService>();

builder.Services.AddSingleton<IConnectionMappingService, ConnectionMappingService>();

// Add generic DI to builders
builder.Services.AddTransient<IRequestHandler<AddOracle<User>.Request, Guid>, AddOracle<User>.Handler>();
builder.Services.AddTransient<IRequestHandler<AddOracle<RandomNumbersAI>.Request, Guid>, AddOracle<RandomNumbersAI>.Handler>();

builder.Services.AddTransient(typeof(IRequestHandler<GetOracleById<User>.Request, GenericOracle<User>>), typeof(GetOracleById<User>.Handler));
builder.Services.AddTransient(typeof(IRequestHandler<GetOracleById<RandomNumbersAI>.Request, GenericOracle<RandomNumbersAI>>), typeof(GetOracleById<RandomNumbersAI>.Handler));
// ***********************

builder.Services.AddMediatR(typeof(Program));

builder.Services.AddDbContext<GameContext>(options => {
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<GameContext>();


builder.Services.Scan(scan => scan
        .FromCallingAssembly()
        .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
        .AsImplementedInterfaces()); 


builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    if (app.Environment.IsDevelopment())
    {
        var db = scope.ServiceProvider.GetRequiredService<GameContext>();
        
        // If we add mock data to an empty database
        
        if (!db.Leaderboards.Any())
        {
            FakeLeaderboardData.Init();
            db.Leaderboards.AddRange(FakeLeaderboardData.leaderboardEntries);
            db.SaveChanges();
        }

        if (!db.RecentGames.Any())
        {
            FakeRecentGameData.Init();
            db.RecentGames.AddRange(FakeRecentGameData.RecentGames);
            db.SaveChanges();
        }
        
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();
app.MapHub<GameHub>("/gameHub");

app.Run();

public partial class Program { }
