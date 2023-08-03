using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Domain;
using RockPaperScissors.Services;
using RockPaperScissors.Services.Interfaces;

namespace RockPaperScissors.Controllers
{
    [Route("game")]
    [ApiController]
    public class RockPaperScissorsController : ControllerBase
    {
        private readonly IGameManager _gameManager;

        public RockPaperScissorsController(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        /// <summary>
        /// Метод создания игры для участника-инициатора игры
        /// </summary>
        /// <returns>Идентификатор игры и участника, создавшего игру</returns>
        [HttpPost("create")]
        public async Task<ActionResult<CreateNewGameDto>> CreateGame(string userName1)
        {
            if (string.IsNullOrWhiteSpace(userName1))
                return BadRequest("UserName can't be Empty");

            var dto = await _gameManager.CreateNewGame(userName1);
            return Ok(dto);
        }

        /// <summary>
        /// Метод присоединения к игре второго участника
        /// </summary>
        /// <returns>Идентификатор участника, присоединившегося к игре</returns>
        [HttpPost("{gameId}/join/{userName2}")]
        public async Task<ActionResult<Guid>> JoinGame(Guid gameId, string userName2)
        {
            var game = await _gameManager.GetGame(gameId);

            if (game == null)
                return NotFound($"Game with {gameId} not found");

            if (string.IsNullOrWhiteSpace(userName2))
                return BadRequest("UserName can't be Empty");

            if (game.Player1.Name == userName2)
                return BadRequest("Player can't play with himself");

            var player2Id = await _gameManager.JoinGame(gameId, userName2);

            return Ok(player2Id);
        }

        /// <summary>
        /// Метод выполнения хода игры одним из участников
        /// </summary>
        [HttpPost("{gameId}/user/{userId}/{turn}")]
        public async Task<ActionResult> TurnRound(Guid gameId, Guid userId, string turn)
        {
            var game = await _gameManager.GetGame(gameId);

            if (game == null)
                return NotFound($"Game with {gameId} not found");

            if (game.Player2Id == null)
                return BadRequest("Second player hasn't joined yet. Try later");

            if (game.FinishedAt != null)
                return BadRequest($"Game with {gameId} has already finished");

            if (game.Player1Id != userId && game.Player2Id.Value != userId)
                return BadRequest($"Player with {userId} doesn't participate in Game with {gameId}");

            if (!Enum.TryParse(turn, true, out RockPaperScissors.Domain.RockPaperScissors chosenTurn))
                return BadRequest($"Unknown turn action {turn}");

            await _gameManager.PlayRound(gameId, userId, chosenTurn);

            return Ok();
        }

        /// <summary>
        /// Метод получения статистики по игре
        /// </summary>
        /// <returns>Статистика завершившейся игры</returns>
        [HttpGet("{gameId}/stat")]
        public async Task<ActionResult<GameDto>> GetGameStatistics(Guid gameId)
        {
            var game = await _gameManager.GetGame(gameId);

            if (game == null)
                return NotFound($"Game with {gameId} not found");

            if (game.FinishedAt == null)
                return BadRequest($"Game with {gameId} hasn't finished yet");

            var result = game.Result > 0 ? $"Player {game.Player1.Name} won" : (game.Result < 0 ? $"Player {game.Player2.Name} won" : "Draw");

            var gameDto = new GameDto(gameId, game.Player1.Name, game.Player2.Name, result, game.StartedAt, game.FinishedAt.Value);

            return Ok(gameDto);
        }
    }

    public record GameDto(Guid Id, string user1Name, string user2Name, string Result, DateTime StartedAt, DateTime FinishedAt);
}
