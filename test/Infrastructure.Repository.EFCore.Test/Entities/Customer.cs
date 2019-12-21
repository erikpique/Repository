using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Repository.Abstraction.Core;

namespace Infrastructure.Repository.EFCore.Test.Entities
{
    public class Customer : AggregateRoot<int>
    {
        public string Name { get; set; }

        public IEnumerable<Order> Orders { get; set; }
    }
}
