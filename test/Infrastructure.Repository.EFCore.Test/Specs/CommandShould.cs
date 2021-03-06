namespace Infrastructure.Repository.EFCore.Test.Specs
{
    using global::Infrastructure.Repository.EFCore.Test.Fixtures;
    using System.Threading.Tasks;
    using Xunit;

    public class CommandShould : Fixture
    {
        public CommandShould(ApplicationFactory fixture)
           : base(fixture)
        {
        }

        [Fact]
        public async Task Create_NewCustomer_ReturnSuccess()
        {
            
        }

        [Fact]
        public async Task Delete_Customer_ReturnSuccess()
        {

        }

        [Fact]
        public async Task Update_Customer_ReturnSuccess()
        {

        }
    }
}
