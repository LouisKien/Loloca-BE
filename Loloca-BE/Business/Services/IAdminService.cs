namespace Loloca_BE.Business.Services
{
    public interface IAdminService
    {
        Task<bool> LockAccount(int accountId);
    }
}
