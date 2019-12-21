using System;
using System.Collections.Generic;
using System.IO;
using Infrastructure.Repository.EFCore.Test.Contexts;
using Infrastructure.Repository.EFCore.Test.Entities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Infrastructure.Repository.EFCore.Test
{
    public class SeedData : IBeginSeed, ISeed
    {
        private ServiceProvider _serviceProvider;
        private const int MAXRETRY = 5;

        public ISeed Configure(ServiceProvider serviceProvider)
        {
            if(serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;

            return this;
        }

        public ISeed CustomerSeed()
        {
            if(_serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(_serviceProvider));
            }

            var context = _serviceProvider.GetRequiredService<CustomerContext>();

            context.Database.EnsureCreated();

            var customers = LoadJsonFileToObject<Customer>(@"Data\customers");

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return this;
        }

        public void CleanSeed()
        {
            var context = _serviceProvider.GetRequiredService<CustomerContext>();

            context.Database.EnsureDeleted();
        }

        private IEnumerable<T> LoadJsonFileToObject<T>(string fileName)
        {
            using (var reader = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}{fileName}.json"))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<IEnumerable<T>>(json);
            }
        }
    }

    public interface IBeginSeed
    {
        ISeed Configure(ServiceProvider serviceProvider);
    }

    public interface ISeed
    {
        ISeed CustomerSeed();
        void CleanSeed();
    }
}
