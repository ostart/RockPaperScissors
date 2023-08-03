namespace RockPaperScissors.Domain
{
    public class Round
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public virtual Game Game { get; set; }
        public RockPaperScissors? Player1Response { get; set; }
        public RockPaperScissors? Player2Response { get; set; }
        public int? Result { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}
