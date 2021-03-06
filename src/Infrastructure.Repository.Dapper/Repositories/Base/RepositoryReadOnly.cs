namespace Infrastructure.Repository.Dapper.Repositories.Base
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using DapperExtensions;
    using Infrastructure.Repository.Abstraction.Core;

    public abstract class RepositoryReadOnly<TEntity, TKey> : IRepositoryReadOnly<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        protected RepositoryReadOnly(string connectionString, IsolationLevel isolationLevel, string transactionName)
        {
            ConnectionString = connectionString;
            var connection = new SqlConnection(ConnectionString);
            transactionName = string.IsNullOrWhiteSpace(transactionName) ? DateTime.UtcNow.Ticks.ToString() : transactionName;
            Transaction = connection.BeginTransaction(isolationLevel, transactionName);
        }

        protected string ConnectionString { get; private set; }

        protected SqlTransaction Transaction { get; private set; }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            return connection.Count<TEntity>(predicate, Transaction);
        }

        public async Task<IEnumerable<TEntity>> FindAndPaginateAsync(
            Expression<Func<TEntity, bool>> predicate,
            int skip = 0,
            int take = 50,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            return connection.GetPage<TEntity>(predicate, ConvertIOrderedQueryableToSort(orderBy), skip, take, Transaction);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            return connection.GetList<TEntity>(predicate, ConvertIOrderedQueryableToSort(orderBy), Transaction);
        }

        public async Task<TEntity> FindFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            return connection.GetList<TEntity>(predicate, ConvertIOrderedQueryableToSort(orderBy), Transaction).FirstOrDefault();
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            return connection.Get<TEntity>(id, Transaction);
        }

        private IList<ISort> ConvertIOrderedQueryableToSort(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> func)
        {
            if (func != null)
            {
                var expression = Expression.Lambda<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>(Expression.Call(func.Method));
                var operation = (BinaryExpression)expression.Body;
                var left = (ParameterExpression)operation.Left;

                var sort = new List<ISort>
                {
                    new Sort
                    {
                        PropertyName = left.Name,
                        Ascending = true
                    }
                };

                return sort;
            }

            return null;
        }
    }
}
