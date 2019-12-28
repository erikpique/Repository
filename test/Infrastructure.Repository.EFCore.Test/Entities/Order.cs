using Infrastructure.Repository.Abstraction.Core;

namespace Infrastructure.Repository.EFCore.Test.Entities
{
    public class Order : Entity<int>
    {
        public int Id { get; set; }

        public string OrderName { get; set; }

        public Customer Customer { get; set; }

        public Agent Agent { get; set; }
    }
}
