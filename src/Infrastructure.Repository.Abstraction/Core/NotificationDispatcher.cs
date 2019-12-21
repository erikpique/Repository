using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Abstraction.Core
{
    public static class NotificationDispatcher
    {
        private static readonly ConcurrentDictionary<Type, INotificationHandler<INotification>> _notifications = new ConcurrentDictionary<Type, INotificationHandler<INotification>>();

        public async static Task DispatchAsync(INotification notification)
        {
            var constructedType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());

            var handler = _notifications.GetOrAdd(constructedType, type => (INotificationHandler<INotification>)Activator.CreateInstance(constructedType, new object[] { notification }));

            await handler.HandlerAsync(notification);
        }
    }
}
