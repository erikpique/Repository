using System.Threading.Tasks;
using Infrastructure.Repository.Abstraction.Core;
using Infrastructure.Repository.EFCore.Test.Contexts;
using Infrastructure.Repository.EFCore.Test.Entities;
using Infrastructure.Repository.EFCore.Test.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Infrastructure.Repository.EFCore.Test
{
    [TestFixture]
    public class CustomerRepositoryTest
    {
        private ServiceProvider _serviceProvider;
        private const string CONNECTIONSTRING = "Data Source=localhost,3433;Initial Catalog=DbTest;Persist Security Info=True;User ID=sa;Password=Password1234!";
        private ISeed _seed;

        [OneTimeSetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddTransient<IBeginSeed, SeedData>()
                .AddTransient<IRepositoryReadOnly<Customer, int>, CustomerRepository>()
                .AddDbContext<CustomerContext>(conf => conf.UseSqlServer(CONNECTIONSTRING));

            _serviceProvider = services.BuildServiceProvider();

            _seed = _serviceProvider.GetRequiredService<IBeginSeed>()
                .Configure(_serviceProvider)
                .CustomerSeed();
        }

        [TestCase("test1", 1)]
        [TestCase("test2", 2)]
        [TestCase("test3", 3)]
        public async Task GetCustomer(string expected, int id)
        {
            var customer = await _serviceProvider.GetRequiredService<IRepositoryReadOnly<Customer, int>>()
                .GetByIdAsync(id);

            Assert.AreEqual(expected, customer.Name);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _seed.CleanSeed();
        }
    }
}