namespace Infrastructure.Repository.EFCore.Test.Fixtures
{
    using Xunit;

    public abstract class Fixture : IClassFixture<ApplicationFactory>
    {
        protected Fixture(ApplicationFactory fixture)
        {
        }
    }
}
