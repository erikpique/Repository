namespace Infrastructure.Repository.EFCore.Repositories.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Infrastructure.Repository.Abstraction.Core;
    using Microsoft.EntityFrameworkCore;

    public abstract class RepositoryReadOnly<TContext, TEntity, TKey> : IRepositoryReadOnly<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
        where TContext : DbContext
    {
        protected readonly DbContext Context;

        protected RepositoryReadOnly(TContext context)
        {
            Context = context;
            Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAndPaginateAsync(
            Expression<Func<TEntity, bool>> predicate,
            int skip = 0,
            int take = 50,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties)
        {
            var query = Context.Set<TEntity>().Where(predicate);

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take > 0)
            {
                query = query.Take(take);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties)
        {
            var query = Context.Set<TEntity>().Where(predicate);

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<TEntity> FindFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties)
        {
            var query = Context.Set<TEntity>().Where(predicate);

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).FirstOrDefaultAsync();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetByIdAsync(TKey id) => await Context.Set<TEntity>().FindAsync(id);
    }
}
