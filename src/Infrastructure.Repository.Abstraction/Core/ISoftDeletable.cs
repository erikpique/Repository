namespace Infrastructure.Repository.Abstraction.Core
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}
