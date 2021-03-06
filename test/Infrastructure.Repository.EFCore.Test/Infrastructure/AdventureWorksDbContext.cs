namespace Infrastructure.Repository.EFCore.Test.Infrastructure
{
    using Microsoft.EntityFrameworkCore;

    public class AdventureWorksDbContext : DbContext
    {
        public AdventureWorksDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
