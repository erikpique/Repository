using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Infrastructure.Repository.EFCore.Test.Contexts;
using Infrastructure.Repository.EFCore.Test.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Infrastructure.Repository.EFCore.Test
{
    public class SeedData : IBeginSeed, IIsSqlReady, ISeed
    {
        private ServiceProvider _serviceProvider;
        private const int MAXRETRY = 5;

        public IIsSqlReady Configure(ServiceProvider serviceProvider)
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

        public ISeed IsSqlReady(string connectionstring)
        {
            bool ready = false;
            var retry = 0;

            do
            {
                try
                {
                    using (var conn = new SqlConnection(connectionstring))
                    {
                        conn.Open();
                        ready = true;
                    }
                }
                catch
                {
                    if(++retry > MAXRETRY)
                    {
                        throw new Exception("Exceeded maximum number of retries to connect to the database");
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
            while (!ready);

            return this;
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
        IIsSqlReady Configure(ServiceProvider serviceProvider);
    }

    public interface IIsSqlReady
    {
        ISeed IsSqlReady(string connectionstring);
    }

    public interface ISeed
    {
        ISeed CustomerSeed();
        void CleanSeed();
    }
}
