using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Repository.Abstraction.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.EFCore
{
    public abstract class RepositoryBase<TContext, TEntity, TKey> : RepositoryReadOnlyBase<TContext, TEntity, TKey>, IRepository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
        where TContext : DbContext
    {
        protected RepositoryBase(TContext context)
            : base(context)
        {
        }

        public async Task AddAsync(TEntity entity) => await Context.Set<TEntity>().AddAsync(entity);

        public async Task AddAsync(IEnumerable<TEntity> entities) => await Context.Set<TEntity>().AddRangeAsync(entities);

        public async Task CommitAsync()
        {
            var entities = GetEntitiesWithNotifications();

            await DispatchNotifications(entities);

            await Context.SaveChangesAsync();
        }

        public Task RemoveAsync(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);

            return Task.CompletedTask;
        }

        private TEntity[] GetEntitiesWithNotifications() => Context.ChangeTracker.Entries<TEntity>()
                .Select(track => track.Entity)
                .Where(entity => entity.Notifications.Any())
                .ToArray();

        private async Task DispatchNotifications(TEntity[] entities)
        {
            var tasks = new List<Task>();

            foreach (var entity in entities)
            {
                var notifications = entity.Notifications.ToArray();

                entity.Notifications.Clear();

                foreach (var notification in notifications)
                {
                    tasks.Add(NotificationDispatcher.DispatchAsync(notification));
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
