using System;
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
        private readonly IUserService _userService;

        private bool _disposed;

        protected RepositoryBase(TContext context, IUserService userService)
            : base(context)
        {
            _userService = userService;
        }

        public async Task AddAsync(TEntity entity) => await Context.Set<TEntity>().AddAsync(entity);

        public async Task AddAsync(IEnumerable<TEntity> entities) => await Context.Set<TEntity>().AddRangeAsync(entities);

        public async Task<bool> CommitAsync()
        {
            var entities = GetEntitiesWithNotifications();

            await DispatchNotifications(entities);

            SetAudition();

            await Context.SaveChangesAsync();

            return true;
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
                    Context.Dispose();
                }

                _disposed = true;
            }
        }

        private void SetAudition()
        {
            var datetime = DateTime.UtcNow;

            foreach (var entry in Context.ChangeTracker.Entries<TEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _userService.UserName;
                        entry.Entity.Created = datetime;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _userService.UserName;
                        entry.Entity.LastModified = datetime;
                        break;
                    case EntityState.Deleted:
                        if (entry is ISoftDeletable)
                        {
                            entry.Entity.LastModifiedBy = _userService.UserName;
                            entry.Entity.LastModified = datetime;
                            ((ISoftDeletable)entry.Entity).IsDeleted = true;
                            entry.State = EntityState.Unchanged;
                        }

                        break;
                }
            }
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
