namespace Infrastructure.Repository.Abstraction.Core
{
    using System.Collections.Generic;
    using MediatR;

    public interface IAggregateRoot
    {
        List<INotification> Notifications { get; }
    }
}
