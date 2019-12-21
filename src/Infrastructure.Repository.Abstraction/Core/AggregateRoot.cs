using System.Collections.Generic;

namespace Infrastructure.Repository.Abstraction.Core
{
    public abstract class AggregateRoot<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }

        public List<INotification> Notifications { get; private set; }

        public void AddNotification(INotification eventItem)
        {
            Notifications = Notifications ?? new List<INotification>();
            Notifications.Add(eventItem);
        }

        public void RemoveNotification(INotification eventItem)
        {
            Notifications?.Remove(eventItem);
        }
    }
}
