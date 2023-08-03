using RockPaperScissors.Domain;
using RockPaperScissors.Infrastructure.Interfaces;
using RockPaperScissors.Services.Interfaces;
using System.Xml;

namespace RockPaperScissors.Services
{
    public class GameManager : IGameManager
    {
        private readonly IRepository<Round> _roundRepository;
        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<Game> _gameRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GameManager(IRepository<Round> roundRepository, IRepository<Player> playerRepository,
            IRepository<Game> gameRepository, IUnitOfWork unitOfWork)
        {
            _roundRepository = roundRepository;
            _playerRepository = playerRepository;
            _gameRepository = gameRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateNewGameDto> CreateNewGame(string playerName, int numberOfRounds = 5)
        {
            if (string.IsNullOrWhiteSpace(playerName)) throw new ArgumentNullException(nameof(playerName));
            if (numberOfRounds <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRounds));

            var user = await _playerRepository.Get(x => x.Name == playerName);
            if (user == null)
            {
                user = CreateNewPlayer(playerName);
                await _playerRepository.Add(user);
            }
            var game = CreateNewGame(user.Id, numberOfRounds);
            await _gameRepository.Add(game);

            await _unitOfWork.Commit();

            return new CreateNewGameDto(game.Id, user.Id);
        }

        public async Task<Guid> JoinGame(Guid gameId, string playerName)
        {
            var game = await _gameRepository.GetById(gameId);
            if (game == null) throw new ArgumentNullException(nameof(game));
            if (game.FinishedAt != null) throw new Exception($"Game with {gameId} has already finished");

            var user = await _playerRepository.Get(x => x.Name == playerName);
            if (user == null)
            {
                user = CreateNewPlayer(playerName);
                await _playerRepository.Add(user);
            }

            game.Player2Id = user.Id;

            await _unitOfWork.Commit();

            return user.Id;
        }

        public async Task PlayRound(Guid gameId, Guid playerId, RockPaperScissors.Domain.RockPaperScissors turn)
        {
            var game = await _gameRepository.GetById(gameId);
            if (game == null) throw new ArgumentNullException(nameof(game));
            if (game.FinishedAt != null) throw new Exception($"Game with {gameId} has already finished");

            var player = await _playerRepository.GetById(playerId);
            if (player == null) throw new ArgumentNullException(nameof(player));

            var actualRounds = await _roundRepository.GetAll(x => x.GameId == gameId);
            var lastRound = actualRounds.OrderByDescending(x => x.StartedAt).FirstOrDefault();

            var isOpponentAttempt = lastRound != null && lastRound.FinishedAt == null;
            if (isOpponentAttempt)
            {
                if (IsFirstPlayer(game, playerId))
                {
                    if (lastRound.Player1Response != null) throw new Exception($"Player with {playerId} has already made his turn");
                    lastRound.Player1Response = turn;
                }
                else
                {
                    if (lastRound.Player2Response != null) throw new Exception($"Player with {playerId} has already made his turn");
                    lastRound.Player2Response = turn;
                }

                PlayRound(lastRound);

                UpdateGameStatus(game, actualRounds);
            }
            else
            {
                var round = IsFirstPlayer(game, playerId) ? CreaNewRoundPlayer1(gameId, turn) : CreaNewRoundPlayer2(gameId, turn);
                await _roundRepository.Add(round);
            }            

            await _unitOfWork.Commit();
        }

        public async Task<Game> GetGame(Guid gameId)
        {
            return await _gameRepository.Get(x => x.Id == gameId, x => x.Player1, x => x.Player2);
        }

        private static bool IsFirstPlayer(Game game, Guid playerId)
        {
            var firstPlayer = game.Player1Id == playerId;
            var secondPlayer = game.Player2Id == playerId;
            if (firstPlayer == false && secondPlayer == false) throw new Exception($"Player with id {playerId} isn't from this game");

            return firstPlayer ? true : false;
        }

        private static Player CreateNewPlayer(string playerName)
        {
            return new Player { Id = Guid.NewGuid(), Name = playerName };
        }

        private static Game CreateNewGame(Guid player1Id, int numberOfRounds)
        {
            return new Game
            {
                Id = Guid.NewGuid(),
                NumberOfRounds = numberOfRounds,
                Player1Id = player1Id,
                Player2Id = null,
                Result = null,
                StartedAt = DateTime.UtcNow,
                FinishedAt = null
            };
        }

        private static void PlayRound(Round round)
        {
            var result = RoundChecker.CheckRoundWinner(round.Player1Response.Value, round.Player2Response.Value);
            round.Result = result;
            round.FinishedAt = DateTime.UtcNow;
        }

        private static Round CreaNewRoundPlayer1(Guid gameId, Domain.RockPaperScissors turn)
        {
            return new Round
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                Player1Response = turn,
                Player2Response = null,
                Result = null,
                StartedAt = DateTime.UtcNow,
                FinishedAt = null
            };
        }

        private static Round CreaNewRoundPlayer2(Guid gameId, Domain.RockPaperScissors turn)
        {
            return new Round
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                Player1Response = null,
                Player2Response = turn,
                Result = null,
                StartedAt = DateTime.UtcNow,
                FinishedAt = null
            };
        }

        private static void UpdateGameStatus(Game game, IEnumerable<Round> actualRounds)
        {
            var resultSum = actualRounds.Select(x => x.Result.Value).Aggregate((a, b) => a + b);

            var isAheadOfTimeWinner = Math.Abs(resultSum) > (game.NumberOfRounds / 2);

            if (actualRounds.Count() == game.NumberOfRounds || isAheadOfTimeWinner)
            {
                game.Result = resultSum;
                game.FinishedAt = DateTime.UtcNow;
            }
        }
    }

    public record CreateNewGameDto(Guid GameId, Guid Player1Id);
}
