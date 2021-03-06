namespace Infrastructure.Repository.EFCore.Test
{
    using global::Infrastructure.Repository.EFCore.Test.Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AdventureWorksDbContext>(options => 
            {
                options.UseSqlServer(_configuration.GetConnectionString("SQLConnectionString"));
            });
        }
    }
}
