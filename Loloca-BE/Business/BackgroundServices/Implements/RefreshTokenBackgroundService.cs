using Hangfire;
using Loloca_BE.Business.BackgroundServices.Interfaces;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.BackgroundServices.Implements
{
    public class RefreshTokenBackgroundService : IRefreshTokenBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RemoveExpiredRefreshToken()
        {
            try
            {
                var refreshTokens = await _unitOfWork.RefreshTokenRepository.GetAsync(r => r.ExpiredDate < DateTime.UtcNow);
                foreach (var refreshToken in refreshTokens)
                {
                    await _unitOfWork.RefreshTokenRepository.DeleteAsync(refreshToken);
                }
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
