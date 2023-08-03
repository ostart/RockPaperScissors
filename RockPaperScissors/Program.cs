using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Domain;
using RockPaperScissors.Infrastructure;
using RockPaperScissors.Infrastructure.Interfaces;
using RockPaperScissors.Services;
using RockPaperScissors.Services.Interfaces;
using Serilog;
using System.Xml;

var exitCode = 0;

try
{
    Environment.CurrentDirectory = AppContext.BaseDirectory;

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((context, configuration) =>
    {
        var loggerConfiguration = configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext();
    });

    AddServicesToTheContainer(builder);

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<RockPaperScissorsDbContext>();
        dbContext.Database.EnsureCreated();
    }

    app.UseSerilogRequestLogging();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while running the application. Application stopped gracefully");
    exitCode = -1;
}
finally
{
    Log.Debug("Application stopped");
    Log.CloseAndFlush();
    Environment.Exit(exitCode);
}

static void AddServicesToTheContainer(WebApplicationBuilder builder)
{
    builder.Services.AddControllers();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });
    });
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<RockPaperScissorsDbContext>(options =>
                options.UseInMemoryDatabase("RockPaperScissorsInMemoryDB"), ServiceLifetime.Singleton);
    builder.Services.AddScoped<IRepository<Round>, Repository<Round>>();
    builder.Services.AddScoped<IRepository<Player>, Repository<Player>>();
    builder.Services.AddScoped<IRepository<Game>, Repository<Game>>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<RoundChecker>();
    builder.Services.AddScoped<IGameManager, GameManager>();
}