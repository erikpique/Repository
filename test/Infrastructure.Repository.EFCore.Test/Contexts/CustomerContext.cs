using Infrastructure.Repository.EFCore.Test.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.EFCore.Test.Contexts
{
    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(c =>
            {
                c.Ignore(c => c.Notifications);
                c.HasKey(cs => cs.Id);
                c.ToTable("Customer", "dbo");
                c.HasMany(c => c.Orders)
                    .WithOne(o => o.Customer)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
