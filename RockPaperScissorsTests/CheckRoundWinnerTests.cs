using RockPaperScissors.Domain;

namespace RockPaperScissorsTests
{
    public class CheckRoundWinnerTests
    {
        [TestCase(RockPaperScissors.Domain.RockPaperScissors.Rock, RockPaperScissors.Domain.RockPaperScissors.Rock, 0)]
        [TestCase(RockPaperScissors.Domain.RockPaperScissors.Scissors, RockPaperScissors.Domain.RockPaperScissors.Scissors, 0)]
        [TestCase(RockPaperScissors.Domain.RockPaperScissors.Paper, RockPaperScissors.Domain.RockPaperScissors.Paper, 0)]
        [TestCase(RockPaperScissors.Domain.RockPaperScissors.Rock, RockPaperScissors.Domain.RockPaperScissors.Scissors, 1)]
        [TestCase(RockPaperScissors.Domain.RockPaperScissors.Scissors, RockPaperScissors.Domain.RockPaperScissors.Paper, 1)]
        [TestCase(RockPaperScissors.Domain.RockPaperScissors.Paper, RockPaperScissors.Domain.RockPaperScissors.Rock, 1)]
        [TestCase(RockPaperScissors.Domain.RockPaperScissors.Rock, RockPaperScissors.Domain.RockPaperScissors.Paper, -1)]
        [TestCase(RockPaperScissors.Domain.RockPaperScissors.Paper, RockPaperScissors.Domain.RockPaperScissors.Scissors, -1)]
        [TestCase(RockPaperScissors.Domain.RockPaperScissors.Scissors, RockPaperScissors.Domain.RockPaperScissors.Rock, -1)]
        public void CheckRoundWinner_AllInputCombinations(RockPaperScissors.Domain.RockPaperScissors player1, RockPaperScissors.Domain.RockPaperScissors player2, int expectedResult)
        {
            var result = RoundChecker.CheckRoundWinner(player1, player2);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}