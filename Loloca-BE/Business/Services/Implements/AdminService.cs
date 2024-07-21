
using AutoMapper;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.Services.Implements
{
    public class AdminService : IAdminService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> LockAccount(int accountId)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                    if (account == null)
                    {
                        throw new Exception("Không tìm thấy tài khoản");
                    }

                    account.Status = 0; // giả định rằng 0 là trạng thái khóa

                    await _unitOfWork.AccountRepository.UpdateAsync(account);
                    await _unitOfWork.SaveAsync();

                    if (account.Role == 2)
                    {
                        var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.AccountId == accountId)).FirstOrDefault();
                        if (tourGuide == null)
                        {
                            throw new Exception("Lost information");
                        }
                        tourGuide.Status = 0;
                        await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                        await _unitOfWork.SaveAsync();
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    // Thêm logging hoặc xử lý lỗi khác nếu cần
                    await transaction.RollbackAsync();
                    throw new Exception("Lỗi khi khóa tài khoản", ex);
                }
            }
        }


    }

}
