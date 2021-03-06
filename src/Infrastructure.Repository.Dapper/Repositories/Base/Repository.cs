namespace Infrastructure.Repository.Dapper.Repositories.Base
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using DapperExtensions;
    using Infrastructure.Repository.Abstraction.Core;

    public abstract class Repository<TEntity, TKey> : RepositoryReadOnly<TEntity, TKey>, IRepository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
    {
        private bool _disposed;

        protected Repository(string connectionString, IsolationLevel isolationLevel = IsolationLevel.Serializable, string transactionName = "default")
            : base(connectionString, isolationLevel, transactionName)
        {
        }

        public async Task AddAsync(TEntity entity)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            connection.Insert(entity, Transaction);
        }

        public async Task AddAsync(IEnumerable<TEntity> entities)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            connection.Insert(entities, Transaction);
        }

        public async Task RemoveAsync(TEntity entity)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            connection.Delete(entity, Transaction);
        }

        public async Task RemoveAsync(IEnumerable<TEntity> entities)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            connection.Delete(entities, Transaction);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Transaction.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
