using Loloca_BE.Business.Models.CustomerView;

namespace Loloca_BE.Business.Services
{
    public interface ICustomerService
    {
        Task UpdateCustomerInfo(int customerId, UpdateProfile model);
        Task<bool> ChangeCustomerPassword(int customerId, ChangePassword model);
    }
}
