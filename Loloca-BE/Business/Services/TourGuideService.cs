
using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Loloca_BE.Business.Models.TourGuideView;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Loloca_BE.Business.Services
{
    public class TourGuideService : ITourGuideService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IAuthService _authService;
        private readonly IMemoryCache _cache;

        public TourGuideService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper, IGoogleDriveService googleDriveService, IAuthService authService, IMemoryCache cache)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _googleDriveService = googleDriveService;
            _authService = authService;
            _cache = cache;
        }

        public async Task UploadAvatarAsync(IFormFile file, int TourGuideId)
        {
            string parentFolderId = "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn";

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new InvalidDataException("Only image files are allowed.");
            }

            var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(TourGuideId);
            if (tourGuide == null)
            {
                throw new Exception($"TourGuide with TourGuideId {TourGuideId} doesn't exist");
            }

            try
            {
                // If an avatar exists, delete the old one
                if (!tourGuide.AvatarPath.IsNullOrEmpty() && tourGuide.AvatarUploadDate.HasValue)
                {
                    await _googleDriveService.DeleteFileAsync(tourGuide.AvatarPath, parentFolderId);
                }

                string fileName = $"Avatar_TourGuide_{TourGuideId}";

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    Parents = new List<string>() { parentFolderId },
                    MimeType = file.ContentType
                };

                // Upload Directly to Google Drive
                using (var stream = file.OpenReadStream())
                {
                    await _googleDriveService.UploadFileAsync(stream, fileMetadata);
                }

                tourGuide.AvatarPath = fileName;
                tourGuide.AvatarUploadDate = DateTime.Now;

                await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                await _unitOfWork.SaveAsync();
            } catch (Exception ex)
            {
                throw new Exception("Cannot update avatar");
            }
        }

        public async Task UploadCoverAsync(IFormFile file, int TourGuideId)
        {
            string parentFolderId = "1s642kdPTeuccQ0bcXXPXkEdAVCWDItmH";

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new InvalidDataException("Only image files are allowed.");
            }

            var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(TourGuideId);
            if (tourGuide == null)
            {
                throw new Exception($"TourGuide with TourGuideId {TourGuideId} doesn't exist");
            }

            try
            {
                // If an avatar exists, delete the old one
                if (!tourGuide.CoverPath.IsNullOrEmpty() && tourGuide.CoverUploadDate.HasValue)
                {
                    await _googleDriveService.DeleteFileAsync(tourGuide.CoverPath, parentFolderId);
                }

                string fileName = $"Cover_TourGuide_{TourGuideId}";

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    Parents = new List<string>() { parentFolderId },
                    MimeType = file.ContentType
                };

                // Upload Directly to Google Drive
                using (var stream = file.OpenReadStream())
                {
                    await _googleDriveService.UploadFileAsync(stream, fileMetadata);
                }

                tourGuide.CoverPath = fileName;
                tourGuide.CoverUploadDate = DateTime.Now;

                await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                await _unitOfWork.SaveAsync();
            } catch (Exception ex)
            {
                throw new Exception("Cannot update cover");
            }
        }

        public async Task UpdateTourGuideInfo(int tourguideId, UpdateProfileTourGuide model)
        {
            try
            {
                var tourguide = await _unitOfWork.TourGuideRepository.GetByIDAsync(tourguideId);
                if (tourguide == null)
                {
                    throw new Exception("Tour guide not found");
                }

                _mapper.Map(model, tourguide);

                await _unitOfWork.TourGuideRepository.UpdateAsync(tourguide);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                // Add logging or other error handling as needed
                throw new Exception("Error updating tour guide info", ex);
            }
        }

        public async Task<bool> ChangeTourGuidePassword(int tourguideId, ChangePasswordTourGuide model)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(tourguideId);
                if (account == null)
                {
                    throw new Exception("Không tìm thấy hướng dẫn viên");
                }

                // Kiểm tra vai trò của tài khoản
                if (account.Role != 2)
                {
                    throw new Exception("Không được phép thay đổi mật khẩu");
                }

                if (!await _authService.VerifyPassword(model.OldPassword, account.HashedPassword))
                {
                    throw new Exception("Mật khẩu hiện tại không đúng");
                }

                account.HashedPassword = await _authService.HashPassword(model.NewPassword);

                await _unitOfWork.AccountRepository.UpdateAsync(account);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Thêm logging hoặc xử lý lỗi khác nếu cần
                throw new Exception("Lỗi khi thay đổi mật khẩu hướng dẫn viên", ex);
            }
        }

        public async Task<GetTourGuideInfo> GetTourGuideInfoAsync(int tourGuideId)
        {
            var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(tourGuideId);
            if (tourGuide == null)
            {
                throw new Exception("Tour guide not found.");
            }

            byte[]? avatarContent = await GetImageFromCacheOrDriveAsync(tourGuide.AvatarPath, "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn");
            byte[]? coverContent = await GetImageFromCacheOrDriveAsync(tourGuide.CoverPath, "1s642kdPTeuccQ0bcXXPXkEdAVCWDItmH");

            return new GetTourGuideInfo
            {
                FirstName = tourGuide.FirstName,
                LastName = tourGuide.LastName,
                DateOfBirth = tourGuide.DateOfBirth,
                Gender = tourGuide.Gender,
                PhoneNumber = tourGuide.PhoneNumber,
                Address = tourGuide.Address,
                ZaloLink = tourGuide.ZaloLink,
                FacebookLink = tourGuide.FacebookLink,
                InstagramLink = tourGuide.InstagramLink,
                PricePerDay = tourGuide.PricePerDay,
                Avatar = avatarContent,
                AvatarUploadedTime = tourGuide.AvatarUploadDate,
                Cover = coverContent,
                CoverUploadedTime = tourGuide.CoverUploadDate
            };
        }

        private async Task<byte[]> GetImageFromCacheOrDriveAsync(string imagePath, string parentFolderId)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return null;
            }

            string cacheKey = $"{imagePath}";
            if (!_cache.TryGetValue(cacheKey, out byte[] imageContent))
            {
                // Image not in cache, fetch from Google Drive
                imageContent = await _googleDriveService.GetFileContentAsync(imagePath, parentFolderId);

                if (imageContent != null)
                {
                    // Store in cache for 1 hour
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                    };
                    _cache.Set(cacheKey, imageContent, cacheEntryOptions);
                }
            }

            return imageContent;
        }
    }
}
