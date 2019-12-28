using System;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Abstraction.Core
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> CommitAsync();
    }
}
