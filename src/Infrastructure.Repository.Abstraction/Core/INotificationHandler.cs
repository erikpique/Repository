using System.Threading.Tasks;

namespace Infrastructure.Repository.Abstraction.Core
{
    public interface INotificationHandler<TNotification>
        where TNotification : INotification
    {
        Task HandlerAsync(TNotification notification);
    }
}
