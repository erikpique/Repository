using Infrastructure.Repository.EFCore.Test.Contexts;
using Infrastructure.Repository.EFCore.Test.Entities;

namespace Infrastructure.Repository.EFCore.Test.Models
{
    public class CustomerRepository : RepositoryReadOnlyBase<CustomerContext, Customer, int>
    {
        public CustomerRepository(CustomerContext context) : base(context)
        {
        }
    }
}
