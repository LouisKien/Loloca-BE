using Loloca_BE.Business.Models.CustomerView;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface ICustomerService
    {
        Task UpdateCustomerInfo(int customerId, UpdateProfile model);
        Task<bool> ChangeCustomerPassword(int customerId, ChangePassword model);
        Task UploadAvatarAsync(IFormFile file, int CustomerId);
        Task<List<GetCustomersView>> GetCustomers(int page, int pageSize);
        Task<int> GetTotalPage(int pageSize);
        Task<GetCustomersView> GetCustomerById(int customerId);
        Task<GetCustomersView> GetCustomerByIdPrivate(int customerId);

        Task<bool> ChangeStatusBookingTourGuideAsync(int bookingTourRequestId);
        Task<bool> ChangeStatusBookingTourAsync(int bookingTourRequestId);
    }
}
