using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Repository.Abstraction.Core;

namespace Infrastructure.Repository.Dapper
{
    public abstract class RepositoryBase<TEntity, TKey> : RepositoryReadOnlyBase<TEntity, TKey>, IRepository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        protected RepositoryBase(string connectionString) : base(connectionString)
        {
        }

        public Task AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task CommitAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }
    }
}
