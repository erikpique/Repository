namespace Infrastructure.Repository.Abstraction.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUnitOfWork : IDisposable
    {
        Task<int> CommitAsync(CancellationToken cancellationToken);
    }
}
