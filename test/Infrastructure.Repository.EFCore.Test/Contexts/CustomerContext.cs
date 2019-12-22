using Infrastructure.Repository.EFCore.Test.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.EFCore.Test.Contexts
{
    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Agent> Agents { get; set; }

        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Ignore(e => e.Notifications);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Cust_Code");
                entity.ToTable("Customer", "dbo");
                entity.HasMany(e => e.Orders)
                    .WithOne(e => e.Customer)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Agent>(entity => 
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Agent_Code");
                entity.ToTable("Agent", "dbo");
                entity.HasMany(e => e.Customers)
                   .WithOne(e => e.Agent)
                   .OnDelete(DeleteBehavior.ClientCascade)
                   .IsRequired();
                entity.HasMany(e => e.Orders)
                   .WithOne(e => e.Agent)
                   .OnDelete(DeleteBehavior.ClientCascade)
                   .IsRequired();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Ord_Num");
                entity.ToTable("Order", "dbo");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
