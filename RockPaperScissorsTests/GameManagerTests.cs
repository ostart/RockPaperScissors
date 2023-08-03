using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Domain;
using RockPaperScissors.Infrastructure;
using RockPaperScissors.Services;

namespace RockPaperScissorsTests
{
    public class GameManagerTests
    {
        private readonly GameManager gameManager;

        public GameManagerTests()
        {
            var builder = new DbContextOptionsBuilder<RockPaperScissorsDbContext>();
            builder.UseInMemoryDatabase("TestRockPaperScissorsInMemoryDB");
            var options = builder.Options;
            var dbContext = new RockPaperScissorsDbContext(options);
            dbContext.Database.EnsureCreated();
            var uow = new UnitOfWork(dbContext);
            var roundRepository = new Repository<Round>(dbContext);
            var playerRepository = new Repository<Player>(dbContext);
            var gamerRepository = new Repository<Game>(dbContext);
            gameManager = new GameManager(roundRepository, playerRepository, gamerRepository, uow);
        }

        [Test]
        public async Task CreateNewGameSuccessfully()
        {
            var result = await gameManager.CreateNewGame("RandomName1");
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<CreateNewGameDto>());
        }

        [Test]
        public async Task JoinGameSuccessfully()
        {
            var dto = await gameManager.CreateNewGame("RandomName1");
            var result = await gameManager.JoinGame(dto.GameId, "RandomName2");
            Assert.That(result, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result, Is.TypeOf<Guid>());
        }

        [Test]
        public async Task FirstPlayerWin()
        {
            var dto = await gameManager.CreateNewGame("TestPlayer1");
            var player2Id = await gameManager.JoinGame(dto.GameId, "TestPlayer2");

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Paper);
            
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Rock);

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            var game = await gameManager.GetGame(dto.GameId);

            Assert.That(game.FinishedAt, Is.Null);
            Assert.That(game.Result, Is.Null);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            game = await gameManager.GetGame(dto.GameId);

            Assert.That(game.FinishedAt, Is.Not.Null);
            Assert.That(game.Result, Is.EqualTo(2));
        }

        [Test]
        public async Task SecondPlayerWin()
        {
            var dto = await gameManager.CreateNewGame("TestPlayer1");
            var player2Id = await gameManager.JoinGame(dto.GameId, "TestPlayer2");

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Rock);

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);

            var game = await gameManager.GetGame(dto.GameId);

            Assert.That(game.FinishedAt, Is.Null);
            Assert.That(game.Result, Is.Null);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            game = await gameManager.GetGame(dto.GameId);

            Assert.That(game.FinishedAt, Is.Not.Null);
            Assert.That(game.Result, Is.EqualTo(-2));
        }

        [Test]
        public async Task Draw()
        {
            var dto = await gameManager.CreateNewGame("TestPlayer1");
            var player2Id = await gameManager.JoinGame(dto.GameId, "TestPlayer2");

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Rock);

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            var game = await gameManager.GetGame(dto.GameId);

            Assert.That(game.FinishedAt, Is.Null);
            Assert.That(game.Result, Is.Null);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            game = await gameManager.GetGame(dto.GameId);

            Assert.That(game.FinishedAt, Is.Not.Null);
            Assert.That(game.Result, Is.EqualTo(0));
        }

        [Test]
        public async Task FirstPlayerWin_AheadOfTime()
        {
            var dto = await gameManager.CreateNewGame("TestPlayer1");
            var player2Id = await gameManager.JoinGame(dto.GameId, "TestPlayer2");

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            var game = await gameManager.GetGame(dto.GameId);

            Assert.That(game.FinishedAt, Is.Not.Null);
            Assert.That(game.Result, Is.EqualTo(3));
        }

        [Test]
        public async Task ThrowException_FirstPlayerAlreadyHasTurn()
        {
            var dto = await gameManager.CreateNewGame("TestPlayer1");
            var player2Id = await gameManager.JoinGame(dto.GameId, "TestPlayer2");

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            Assert.ThrowsAsync<Exception>(() => gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Paper));
        }

        [Test]
        public async Task ThrowException_GameHasAlreadyFinished()
        {
            var dto = await gameManager.CreateNewGame("TestPlayer1");
            var player2Id = await gameManager.JoinGame(dto.GameId, "TestPlayer2");

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Rock);

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            Assert.ThrowsAsync<Exception>(() => gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Paper));
        }

        [Test]
        public async Task ThrowException_GameHasAlreadyFinished_AheadOfTime()
        {
            var dto = await gameManager.CreateNewGame("TestPlayer1");
            var player2Id = await gameManager.JoinGame(dto.GameId, "TestPlayer2");

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Rock);
            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            await gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Scissors);
            await gameManager.PlayRound(dto.GameId, player2Id, RockPaperScissors.Domain.RockPaperScissors.Paper);

            Assert.ThrowsAsync<Exception>(() => gameManager.PlayRound(dto.GameId, dto.Player1Id, RockPaperScissors.Domain.RockPaperScissors.Paper));
        }
    }
}
