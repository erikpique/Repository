using System.Threading.Tasks;

namespace Infrastructure.Repository.Abstraction.Core
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();
    }
}
