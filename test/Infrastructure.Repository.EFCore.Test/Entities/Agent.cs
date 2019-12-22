using System.Collections.Generic;
using Infrastructure.Repository.Abstraction.Core;

namespace Infrastructure.Repository.EFCore.Test.Entities
{
    public class Agent : IEntity<string>
    {
        public string Id { get; set; }

        public string AgentName { get; set; }

        public string WorkingName { get; set; }

        public decimal Commission { get; set; }

        public string PhoneNo { get; set; }

        public string Country { get; set; }

        public IEnumerable<Customer> Customers { get; set; }

        public IEnumerable<Order> Orders { get; set; }
    }
}
