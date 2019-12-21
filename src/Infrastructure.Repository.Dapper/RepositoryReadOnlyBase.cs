using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DapperExtensions;
using Infrastructure.Repository.Abstraction.Core;

namespace Infrastructure.Repository.Dapper
{
    public abstract class RepositoryReadOnlyBase<TEntity, TKey> : IRepositoryReadOnly<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        protected readonly string ConnectionString;

        protected RepositoryReadOnlyBase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                return connection.Count<TEntity>(predicate);
            }
        }

        public async Task<IEnumerable<TEntity>> FindAndPaginateAsync(Expression<Func<TEntity, bool>> predicate, 
            int skip = 0, 
            int take = 50, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            string includeProperties = "")
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                return connection.GetPage<TEntity>(predicate, null, skip, take);
            }
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            string includeProperties = "")
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                return connection.GetList<TEntity>(predicate);
            }
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                return connection.Get<TEntity>(id);
            }
        }
    }
}
