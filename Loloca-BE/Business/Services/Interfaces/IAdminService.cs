namespace Loloca_BE.Business.Services.Interfaces
{
    public interface IAdminService
    {
        Task<bool> LockAccount(int accountId);
    }
}
