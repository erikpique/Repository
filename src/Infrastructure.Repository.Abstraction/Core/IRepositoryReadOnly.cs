using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Abstraction.Core
{
    public interface IRepositoryReadOnly<TEntity, TKey> where TEntity : AggregateRoot<TKey>
    {
        Task<TEntity> GetByIdAsync(TKey id);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        Task<IEnumerable<TEntity>> FindAndPaginateAsync(Expression<Func<TEntity, bool>> predicate,
            int skip = 0,
            int take = 50,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);
    }
}
