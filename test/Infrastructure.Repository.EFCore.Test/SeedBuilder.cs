﻿using System;
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
    public class SeedBuilder : ISeedBuilder, IIsSqlReady, ISeed
    {
        private ServiceProvider _serviceProvider;
        private const int MAXRETRY = 5;

        public IIsSqlReady Configure(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            return this;
        }

        public ISeed CustomerSeed()
        {
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

        private IEnumerable<T> LoadJsonFileToObject<T>(string path)
        {
            using (var reader = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}{path}.json"))
            {
                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<IEnumerable<T>>(json);
            }
        }
    }

    public interface ISeedBuilder
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
