using RockPaperScissors.Domain;

namespace RockPaperScissors.Services.Interfaces
{
    public interface IGameManager
    {
        Task<CreateNewGameDto> CreateNewGame(string playerName, int numberOfRounds = 5);

        Task<Guid> JoinGame(Guid gameId, string playerName);

        Task PlayRound(Guid gameId, Guid playerId, RockPaperScissors.Domain.RockPaperScissors turn);

        Task<Game> GetGame(Guid gameId);
    }
}
