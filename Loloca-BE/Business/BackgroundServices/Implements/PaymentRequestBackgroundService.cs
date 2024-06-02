using Hangfire;
using Loloca_BE.Business.BackgroundServices.Interfaces;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.BackgroundServices.Implements
{
    public class PaymentRequestBackgroundService : IPaymentRequestBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentRequestBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RejectExpiredPaymentRequest()
        {
            throw new NotImplementedException();
        }
    }
}
