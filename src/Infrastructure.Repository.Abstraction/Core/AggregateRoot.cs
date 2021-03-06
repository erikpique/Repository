namespace Infrastructure.Repository.Abstraction.Core
{
    using System.Collections.Generic;
    using MediatR;

    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    {
        public List<INotification> Notifications { get; private set; }

        public void AddNotification(INotification eventItem)
        {
            Notifications ??= new List<INotification>();
            Notifications.Add(eventItem);
        }

        public void RemoveNotification(INotification eventItem)
        {
            Notifications?.Remove(eventItem);
        }
    }
}
