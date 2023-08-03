using System.Linq.Expressions;

namespace RockPaperScissors.Infrastructure.Interfaces
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate);
        Task<T> GetById(Guid id);
        Task<T> Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
