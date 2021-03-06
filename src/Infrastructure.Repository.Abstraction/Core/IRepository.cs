namespace Infrastructure.Repository.Abstraction.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRepository<TEntity, TKey> : IRepositoryReadOnly<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        Task AddAsync(TEntity entity);

        Task AddAsync(IEnumerable<TEntity> entities);

        Task RemoveAsync(TEntity entity);

        Task RemoveAsync(IEnumerable<TEntity> entities);
    }
}
