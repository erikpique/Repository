using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Abstraction.Core
{
    public interface IRepository<TEntity, TKey> : IRepositoryReadOnly<TEntity, TKey>, IUnitOfWork
        where TEntity : AggregateRoot<TKey>
    {
        Task AddAsync(TEntity entity);

        Task AddAsync(IEnumerable<TEntity> entities);

        Task RemoveAsync(TEntity entity);

        Task RemoveAsync(IEnumerable<TEntity> entities);
    }
}
