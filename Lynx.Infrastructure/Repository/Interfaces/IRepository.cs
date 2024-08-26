using System.Linq.Expressions;

namespace Lynx.Infrastructure.Repository.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

    Task<T> GetAsync(Expression<Func<T, bool>>? filter, CancellationToken cancellationToken);

    Task Add(T entity);

    Task Delete(T entity);

}