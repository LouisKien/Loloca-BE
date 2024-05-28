
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
using System;
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
        private static readonly Random _random = new Random();

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

            byte[]? avatarContent = await _googleDriveService.GetImageFromCacheOrDriveAsync(tourGuide.AvatarPath, "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn");
            byte[]? coverContent = await _googleDriveService.GetImageFromCacheOrDriveAsync(tourGuide.CoverPath, "1s642kdPTeuccQ0bcXXPXkEdAVCWDItmH");

            return new GetTourGuideInfo
            {
                FirstName = tourGuide.FirstName,
                LastName = tourGuide.LastName,
                DateOfBirth = tourGuide.DateOfBirth,
                Gender = tourGuide.Gender,
                PhoneNumber = tourGuide.PhoneNumber,
                Description = tourGuide.Description,
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

        

        public async Task<List<GetTourGuide>> GetRandomTourGuidesAsync(string sessionId, int page, int pageSize, int? lastFetchId)
        {
            var lastTourGuideAddedId = await GetLastTourGuideAddedIdAsync();
            var cacheKey = $"TourGuide_{sessionId}";
            if (!_cache.TryGetValue(cacheKey, out List<GetTourGuide> shuffledItems))
            {
                var tourGuides = (await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1)).ToList();
                List<GetTourGuide> items = new List<GetTourGuide>();
                foreach (var tourGuide in tourGuides)
                {
                    byte[]? avatarContent = await _googleDriveService.GetImageFromCacheOrDriveAsync(tourGuide.AvatarPath, "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn");
                    var item = new GetTourGuide
                    {
                        Avatar = avatarContent,
                        AvatarUploadedTime = tourGuide.AvatarUploadDate,
                        DateOfBirth = tourGuide.DateOfBirth,
                        Description = tourGuide.Description,
                        FirstName = tourGuide.FirstName,
                        Gender = tourGuide.Gender,
                        Id = tourGuide.TourGuideId,
                        LastName = tourGuide.LastName,
                        PricePerDay = tourGuide.PricePerDay
                    };
                    items.Add(item);
                }
                shuffledItems = items.OrderBy(x => _random.Next()).ToList();
                _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(30)); // Cache for 30 minutes
            }
            else if (lastFetchId == null || lastTourGuideAddedId > lastFetchId)
            {
                var newTourGuides = await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1);
                var newTourGuide = newTourGuides.FirstOrDefault(i => i.TourGuideId > lastFetchId);
                if (newTourGuide != null)
                {
                    var item = new GetTourGuide
                    {
                        Avatar = await _googleDriveService.GetImageFromCacheOrDriveAsync(newTourGuide.AvatarPath, "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn"),
                        AvatarUploadedTime = newTourGuide.AvatarUploadDate,
                        DateOfBirth = newTourGuide.DateOfBirth,
                        Description = newTourGuide.Description,
                        FirstName = newTourGuide.FirstName,
                        Gender = newTourGuide.Gender,
                        Id = newTourGuide.TourGuideId,
                        LastName = newTourGuide.LastName,
                        PricePerDay = newTourGuide.PricePerDay
                    };
                    shuffledItems.Add(item); // Add the new item to the existing shuffled list
                    _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(30));
                }
            }

            var pagedItems = shuffledItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return pagedItems.Select(item => new GetTourGuide
            {
                Avatar = item.Avatar,
                AvatarUploadedTime = item.AvatarUploadedTime,
                DateOfBirth = item.DateOfBirth,
                Description = item.Description,
                FirstName = item.FirstName,
                Gender = item.Gender,
                Id = item.Id,
                LastName = item.LastName,
                PricePerDay = item.PricePerDay
            }).ToList();
        }

        public async Task<int?> GetLastTourGuideAddedIdAsync()
        {
            var items = await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1); // Await the task to get the result
            var lastItem = items.OrderByDescending(i => i.TourGuideId).FirstOrDefault(); // Assuming you have a CreatedDate property
            return lastItem?.TourGuideId;
        }

        public async Task<List<GetTourGuide>> GetRandomTourGuidesInCityAsync(string sessionId, int CityId, int page, int pageSize, int? lastFetchId)
        {
            var lastTourGuideAddedId = await GetLastTourGuideAddedIdInCityAsync(CityId);
            var cacheKey = $"City_{CityId}_TourGuide_{sessionId}";
            if (!_cache.TryGetValue(cacheKey, out List<GetTourGuide> shuffledItems))
            {
                var tourGuides = (await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1 && t.CityId == CityId)).ToList();
                List<GetTourGuide> items = new List<GetTourGuide>();
                foreach (var tourGuide in tourGuides)
                {
                    byte[]? avatarContent = await _googleDriveService.GetImageFromCacheOrDriveAsync(tourGuide.AvatarPath, "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn");
                    var item = new GetTourGuide
                    {
                        Avatar = avatarContent,
                        AvatarUploadedTime = tourGuide.AvatarUploadDate,
                        DateOfBirth = tourGuide.DateOfBirth,
                        Description = tourGuide.Description,
                        FirstName = tourGuide.FirstName,
                        Gender = tourGuide.Gender,
                        Id = tourGuide.TourGuideId,
                        LastName = tourGuide.LastName,
                        PricePerDay = tourGuide.PricePerDay
                    };
                    items.Add(item);
                }
                shuffledItems = items.OrderBy(x => _random.Next()).ToList();
                _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(30)); // Cache for 30 minutes
            }
            else if (lastFetchId == null || lastTourGuideAddedId > lastFetchId)
            {
                var newTourGuides = await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1 && t.CityId == CityId);
                var newTourGuide = newTourGuides.FirstOrDefault(i => i.TourGuideId > lastFetchId);
                if (newTourGuide != null)
                {
                    var item = new GetTourGuide
                    {
                        Avatar = await _googleDriveService.GetImageFromCacheOrDriveAsync(newTourGuide.AvatarPath, "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn"),
                        AvatarUploadedTime = newTourGuide.AvatarUploadDate,
                        DateOfBirth = newTourGuide.DateOfBirth,
                        Description = newTourGuide.Description,
                        FirstName = newTourGuide.FirstName,
                        Gender = newTourGuide.Gender,
                        Id = newTourGuide.TourGuideId,
                        LastName = newTourGuide.LastName,
                        PricePerDay = newTourGuide.PricePerDay
                    };
                    shuffledItems.Add(item); // Add the new item to the existing shuffled list
                    _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(30));
                }
            }

            var pagedItems = shuffledItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return pagedItems.Select(item => new GetTourGuide
            {
                Avatar = item.Avatar,
                AvatarUploadedTime = item.AvatarUploadedTime,
                DateOfBirth = item.DateOfBirth,
                Description = item.Description,
                FirstName = item.FirstName,
                Gender = item.Gender,
                Id = item.Id,
                LastName = item.LastName,
                PricePerDay = item.PricePerDay
            }).ToList();
        }

        public async Task<int?> GetLastTourGuideAddedIdInCityAsync(int CityId)
        {
            var items = await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1 && t.CityId == CityId); // Await the task to get the result
            var lastItem = items.OrderByDescending(i => i.TourGuideId).FirstOrDefault(); // Assuming you have a CreatedDate property
            return lastItem?.TourGuideId;
        }

        public async Task<int> GetTotalPage(int pageSize, int? cityId)
        {
            try
            {
                int total;
                if(cityId == null)
                {
                    total = await _unitOfWork.TourGuideRepository.CountAsync();
                } else
                {
                    total = await _unitOfWork.TourGuideRepository.CountAsync(filter: t => t.CityId == cityId);
                }
                return (int)Math.Ceiling(total / (double)pageSize);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
