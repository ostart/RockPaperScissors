using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Domain;

namespace RockPaperScissors.Infrastructure
{
    public class RockPaperScissorsDbContext : DbContext
    {
        public RockPaperScissorsDbContext(DbContextOptions<RockPaperScissorsDbContext> options) : base(options) { }
        public DbSet<Player> Players { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Game> Games { get; set; }
    }
}
