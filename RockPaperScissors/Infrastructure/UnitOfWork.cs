using RockPaperScissors.Infrastructure.Interfaces;

namespace RockPaperScissors.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly RockPaperScissorsDbContext _context;

        public UnitOfWork(RockPaperScissorsDbContext context)
        {
            _context = context;
        }

        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }
    }
}
