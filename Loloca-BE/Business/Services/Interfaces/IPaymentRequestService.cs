using Loloca_BE.Business.Models.PaymentRequestView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface IPaymentRequestService
    {
        Task SendDepositRequest(DepositRequestView depositView);
        Task<List<GetDepositView>> GetAllDepositRequest(int? status);
        Task<List<GetDepositView>> GetDepositByCustomerId(int customerId, int? status);
        Task<List<GetDepositView>> GetDepositByTourGuideId(int tourGuideId, int? status);
    }
}
