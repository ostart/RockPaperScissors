namespace RockPaperScissors.Domain
{
    public class RoundChecker
    {
        private static readonly Dictionary<(RockPaperScissors, RockPaperScissors), int> TruthTable = new()
        {
            {(RockPaperScissors.Rock, RockPaperScissors.Rock), 0 },
            {(RockPaperScissors.Rock, RockPaperScissors.Paper), -1 },
            {(RockPaperScissors.Rock, RockPaperScissors.Scissors), 1 },
            {(RockPaperScissors.Paper, RockPaperScissors.Rock), 1 },
            {(RockPaperScissors.Paper, RockPaperScissors.Paper), 0 },
            {(RockPaperScissors.Paper, RockPaperScissors.Scissors), -1 },
            {(RockPaperScissors.Scissors, RockPaperScissors.Rock), -1 },
            {(RockPaperScissors.Scissors, RockPaperScissors.Paper), 1 },
            {(RockPaperScissors.Scissors, RockPaperScissors.Scissors), 0 }
        };

        public static int CheckRoundWinner(RockPaperScissors player1, RockPaperScissors player2)
        {
            return TruthTable[(player1, player2)];
        }
    }
}
