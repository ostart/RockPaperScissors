namespace RockPaperScissors.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}
