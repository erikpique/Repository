namespace Infrastructure.Repository.EFCore.Repositories.Base
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Repository.Abstraction.Core;
    using Infrastructure.Repository.EFCore.Services;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public abstract class UnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {
        private readonly TContext _dbContext;
        private readonly IUserService _userService;
        private readonly IMediator _mediator;
        private readonly ILogger<UnitOfWork<TContext>> _logger;
        private bool _disposed;

        protected UnitOfWork(
            TContext dbContext,
            IUserService userService,
            IMediator mediator,
            ILogger<UnitOfWork<TContext>> logger)
        {
            _dbContext = dbContext;
            _userService = userService;
            _mediator = mediator;
            _logger = logger;
        }

        public virtual async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Begin transaction...");

            await DispatchDomainEventsAsync();

            SetAudition();

            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("End transaction. Total affected records: {result}", result);

            return result;
        }

        private async Task DispatchDomainEventsAsync()
        {
            var domainEntities = _dbContext.ChangeTracker.Entries()
                .Select(track => track.Entity)
                .Where(entity => ((IAggregateRoot)entity).Notifications.Any());

            var domainEvents = domainEntities
                .SelectMany(entity => ((IAggregateRoot)entity).Notifications)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => ((IAggregateRoot)entity).Notifications.Clear());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }

        private void SetAudition()
        {
            var datetime = DateTimeOffset.UtcNow;

            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        ((AuditableEntity)entry.Entity).CreatedBy = _userService.UserName;
                        ((AuditableEntity)entry.Entity).Created = datetime;
                        break;
                    case EntityState.Modified:
                        ((AuditableEntity)entry.Entity).LastModifiedBy = _userService.UserName;
                        ((AuditableEntity)entry.Entity).LastModified = datetime;
                        break;
                    case EntityState.Deleted:
                        if (entry is ISoftDeletable)
                        {
                            ((AuditableEntity)entry.Entity).LastModifiedBy = _userService.UserName;
                            ((AuditableEntity)entry.Entity).LastModified = datetime;
                            ((ISoftDeletable)entry.Entity).IsDeleted = true;
                            entry.State = EntityState.Unchanged;
                        }

                        break;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    _dbContext.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
