namespace Loloca_BE.Data.Repositories
{
    public interface IUnitOfWork
    {
        Task SaveAsync();
    }
}
