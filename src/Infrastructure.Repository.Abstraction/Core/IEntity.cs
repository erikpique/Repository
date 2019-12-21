namespace Infrastructure.Repository.Abstraction.Core
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
