﻿
using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Hangfire;
using Loloca_BE.Business.Models.TourGuideView;
using Loloca_BE.Business.Models.TourView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Loloca_BE.Business.Services.Implements
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
        private readonly IBackgroundJobClient _backgroundJobs;

        public TourGuideService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper, IGoogleDriveService googleDriveService, IAuthService authService, IMemoryCache cache)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _googleDriveService = googleDriveService;
            _authService = authService;
            _cache = cache;

            RecurringJob.AddOrUpdate<TourGuideService>(
                x => x.RefreshTourGuideCache(),
                Cron.MinuteInterval(2)
            );
            RecurringJob.AddOrUpdate<TourGuideService>(
                x => x.RefreshTourGuideInCityCache(),
                Cron.MinuteInterval(2)
            );
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

                string guid = Guid.NewGuid().ToString();
                string fileName = $"Avatar_TourGuide_{TourGuideId}_{guid}";

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
            }
            catch (Exception ex)
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

                string guid = Guid.NewGuid().ToString();
                string fileName = $"Cover_TourGuide_{TourGuideId}_{guid}";

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
            }
            catch (Exception ex)
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
            try
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GetTourGuide>> GetRandomTourGuidesAsync(string sessionId, int page, int pageSize)
        {
            try
            {
                var cacheKey = $"TourGuide_{sessionId}";

                if (!_cache.TryGetValue(cacheKey, out List<GetTourGuide> shuffledItems))
                {
                    var tourGuides = (await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1)).ToList();
                    shuffledItems = await GenerateShuffledTourList(tourGuides);
                    _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(2));
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GetTourGuide>> GetRandomTourGuidesInCityAsync(string sessionId, int CityId, int page, int pageSize)
        {
            try
            {
                var cacheKey = $"TourGuide_CityId:{CityId}_{sessionId}";

                if (!_cache.TryGetValue(cacheKey, out List<GetTourGuide> shuffledItems))
                {
                    var tourGuides = (await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1 && t.CityId == CityId)).ToList();
                    shuffledItems = await GenerateShuffledTourList(tourGuides);
                    _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(2));
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalPage(int pageSize, int? cityId, string sessionId)
        {
            try
            {
                string cacheKey;
                if (cityId == null)
                {
                    cacheKey = $"TourGuide_{sessionId}";
                }
                else
                {
                    cacheKey = $"TourGuide_CityId:{cityId}_{sessionId}";
                }

                if (_cache.TryGetValue(cacheKey, out List<GetTourGuide> shuffledItems))
                {
                    int totalPages = (int)Math.Ceiling(shuffledItems.Count / (double)pageSize);
                    return totalPages;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task RefreshTourGuideCache()
        {
            var latestTourGuides = await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1);
            var items = new List<GetTourGuide>();
            foreach (var tourGuide in latestTourGuides)
            {
                var item = new GetTourGuide
                {
                    Avatar = string.IsNullOrEmpty(tourGuide.AvatarPath) ? null : await _googleDriveService.GetImageFromCacheOrDriveAsync(tourGuide.AvatarPath, "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn"),
                    AvatarUploadedTime = string.IsNullOrEmpty(tourGuide.AvatarPath) ? null : tourGuide.AvatarUploadDate,
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

            var activeSessionIds = _cache.Get<List<string>>("ActiveSessions") ?? new List<string>();

            foreach (var sessionId in activeSessionIds)
            {
                var cacheKey = $"TourGuide_{sessionId}";
                var shuffledItems = items.OrderBy(x => _random.Next()).ToList();
                _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(2));
            }
        }

        public async Task RefreshTourGuideInCityCache()
        {
            var latestTourGuides = await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1);
            var tourGuidesByCity = latestTourGuides.GroupBy(t => t.CityId).ToDictionary(g => g.Key, g => g.ToList());

            var activeSessionIds = _cache.Get<List<string>>("ActiveSessions") ?? new List<string>();
            foreach (var sessionId in activeSessionIds)
            {
                foreach (var cityGroup in tourGuidesByCity)
                {
                    var cityId = cityGroup.Key;
                    var tourGuidesInCity = cityGroup.Value;

                    var cacheKey = $"TourGuide_CityId:{cityId}_{sessionId}";
                    var shuffledItems = await GenerateShuffledTourList(tourGuidesInCity);
                    _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(2));
                }
            }
        }

        private async Task<List<GetTourGuide>> GenerateShuffledTourList(List<TourGuide> tourGuides)
        {
            var items = new List<GetTourGuide>();

            foreach (var tourGuide in tourGuides)
            {

                var item = new GetTourGuide
                {
                    Avatar = string.IsNullOrEmpty(tourGuide.AvatarPath) ? null : await _googleDriveService.GetImageFromCacheOrDriveAsync(tourGuide.AvatarPath, "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn"),
                    AvatarUploadedTime = string.IsNullOrEmpty(tourGuide.AvatarPath) ? null : tourGuide.AvatarUploadDate,
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

            return items.OrderBy(x => _random.Next()).ToList();
        }
        public async Task<bool> AcceptRequestBookingTourGuideRequest(int bookingRequestId)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var bookingRequest = await _unitOfWork.BookingTourGuideRepository.GetByIDAsync(bookingRequestId);
                    if (bookingRequest == null)
                    {
                        throw new Exception($"Booking request with ID {bookingRequestId} does not exist.");
                    }

                    // Kiểm tra xem BookingTourGuideRequest có trạng thái là 1 không (đã gửi yêu cầu)
                    if (bookingRequest.Status != 1)
                    {
                        throw new Exception("Booking request is not in pending status.");
                    }

                    // Cập nhật trạng thái của BookingTourGuideRequest thành đã chấp nhận
                    bookingRequest.Status = 2;
                    await _unitOfWork.BookingTourGuideRepository.UpdateAsync(bookingRequest);
                    await _unitOfWork.SaveAsync();

                    // Tạo thông báo cho khách hàng
                    var notificationToCustomer = new Notification
                    {
                        UserId = bookingRequest.CustomerId,
                        UserType = "Customer",
                        Title = "Yêu cầu booking đã được chấp nhận",
                        Message = "Your booking request has been accepted by the tour guide.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.NotificationRepository.InsertAsync(notificationToCustomer);

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Error accepting booking request: {ex.Message}");
                }
            }
        }

        public async Task<bool> RejectRequestBookingTourGuideRequest(int bookingRequestId)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var bookingRequest = await _unitOfWork.BookingTourGuideRepository.GetByIDAsync(bookingRequestId);
                    if (bookingRequest == null)
                    {
                        throw new Exception($"Booking request with ID {bookingRequestId} does not exist.");
                    }

                    // Kiểm tra xem BookingTourGuideRequest có trạng thái là 1 không (đã gửi yêu cầu)
                    if (bookingRequest.Status != 1)
                    {
                        throw new Exception("Booking request is not in pending status.");
                    }

                    // Cập nhật trạng thái của BookingTourGuideRequest thành đã từ chối
                    bookingRequest.Status = 3;
                    await _unitOfWork.BookingTourGuideRepository.UpdateAsync(bookingRequest);
                    await _unitOfWork.SaveAsync();

                    // Tạo thông báo cho khách hàng
                    var notificationToCustomer = new Notification
                    {
                        UserId = bookingRequest.CustomerId,
                        UserType = "Customer",
                        Title = "Yêu cầu booking đã bị hủy",
                        Message = "Your booking request has been rejected by the tour guide.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.NotificationRepository.InsertAsync(notificationToCustomer);

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Error accepting booking request: {ex.Message}");
                }
            }
        }

        public async Task<bool> AcceptRequestBookingTourRequest(int bookingTourRequestId)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var bookingRequest = await _unitOfWork.BookingTourRequestRepository.GetByIDAsync(bookingTourRequestId);
                    if (bookingRequest == null)
                    {
                        throw new Exception($"Booking request with ID {bookingTourRequestId} does not exist.");
                    }

                    // Kiểm tra xem BookingTourRequest có trạng thái là 1 không (đã gửi yêu cầu)
                    if (bookingRequest.Status != 1)
                    {
                        throw new Exception("Booking request is not in pending status.");
                    }

                    // Cập nhật trạng thái của BookingTourRequest thành đã chấp nhận
                    bookingRequest.Status = 2;
                    await _unitOfWork.BookingTourRequestRepository.UpdateAsync(bookingRequest);
                    await _unitOfWork.SaveAsync();

                    // Tạo thông báo cho khách hàng
                    var notificationToCustomer = new Notification
                    {
                        UserId = bookingRequest.CustomerId,
                        UserType = "Customer",
                        Title = "Yêu cầu booking đã được chấp nhận",
                        Message = "Your booking request has been accepted by the tour guide.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.NotificationRepository.InsertAsync(notificationToCustomer);

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Error accepting booking request: {ex.Message}");
                }
            }
        }

        public async Task<bool> RejectRequestBookingTourRequest(int bookingTourRequestId)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var bookingRequest = await _unitOfWork.BookingTourRequestRepository.GetByIDAsync(bookingTourRequestId);
                    if (bookingRequest == null)
                    {
                        throw new Exception($"Booking request with ID {bookingTourRequestId} does not exist.");
                    }

                    // Kiểm tra xem BookingTourGuideRequest có trạng thái là 1 không (đã gửi yêu cầu)
                    if (bookingRequest.Status != 1)
                    {
                        throw new Exception("Booking request is not in pending status.");
                    }

                    // Cập nhật trạng thái của BookingTourGuideRequest thành đã từ chối
                    bookingRequest.Status = 3;
                    await _unitOfWork.BookingTourRequestRepository.UpdateAsync(bookingRequest);
                    await _unitOfWork.SaveAsync();

                    // Tạo thông báo cho khách hàng
                    var notificationToCustomer = new Notification
                    {
                        UserId = bookingRequest.CustomerId,
                        UserType = "Customer",
                        Title = "Yêu cầu booking đã bị hủy",
                        Message = "Your booking request has been rejected by the tour guide.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.NotificationRepository.InsertAsync(notificationToCustomer);

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Error accepting booking request: {ex.Message}");
                }
            }
        }
    }
}
