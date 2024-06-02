using AutoMapper;
using Loloca_BE.Business.Models.TourView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Loloca_BE.Business.Services.Implements
{
    public class TourService : ITourService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMemoryCache _cache;
        private static readonly Random _random = new Random();

        public TourService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper, IGoogleDriveService googleDriveService, IMemoryCache cache)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _googleDriveService = googleDriveService;
            _cache = cache;
        }

        public async Task UploadTourImageAsync(TourModelView tourModel, List<IFormFile> images)
        {
            string parentFolderId = "1j6R0VaaZXFbruE553kdGyUrboAxfVw3o";

            using (var transaction =  _unitOfWork.BeginTransaction())
            {
                try
                {
                    // Kiểm tra tất cả các file trước khi bắt đầu upload
                    foreach (var image in images)
                    {
                        if (!image.ContentType.StartsWith("image/"))
                        {
                            throw new InvalidDataException("Only image files are allowed.");
                        }
                    }

                    // Create Tour entity
                    var tour = _mapper.Map<Tour>(tourModel);
                    tour.Status = 0; // Thiết lập trạng thái mặc định

                    // Save Tour to database
                    await _unitOfWork.TourRepository.InsertAsync(tour);
                    await _unitOfWork.SaveAsync();

                    var tourId = tour.TourId;
                    var tourImages = new List<TourImage>();

                    // Upload each image to Google Drive
                    foreach (var image in images)
                    {
                        string fileName = $"Tour_{tourId}_{Guid.NewGuid()}";

                        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                        {
                            Name = fileName,
                            Parents = new List<string>() { parentFolderId },
                            MimeType = image.ContentType
                        };

                        using (var stream = image.OpenReadStream())
                        {
                            await _googleDriveService.UploadFileAsync(stream, fileMetadata);
                        }

                        // Create TourImage entity
                        var tourImage = new TourImage
                        {
                            TourId = tourId,
                            ImagePath = fileName,
                            UploadDate = DateTime.Now
                        };

                        tourImages.Add(tourImage);
                    }

                    // Save TourImages to database
                    if (tourImages.Count > 0)
                    {
                        await _unitOfWork.TourImageRepository.AddRangeAsync(tourImages);
                        await _unitOfWork.SaveAsync();
                    }

                    // Create and save notifications for admins
                    var adminAccounts = await _unitOfWork.AccountRepository.GetAllAsync(a => a.Role == 1 && a.Status == 1);

                    var notifications = adminAccounts.Select(admin => new Notification
                    {
                        UserId = admin.AccountId,
                        UserType = "Admin",
                        Title = "Tour mới đã được tạo",
                        Message = $"Chuyến đi mới đã được tạo chờ bạn xác nhận tên chuyến đi: {tour.Name}.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    }).ToList();

                    if (notifications.Any())
                    {
                        await _unitOfWork.NotificationRepository.AddRangeAsync(notifications);
                        await _unitOfWork.SaveAsync();
                    }

                    // Commit the transaction
                    await transaction.CommitAsync();
                }
                catch (InvalidDataException)
                {
                    // Nếu có InvalidDataException, rollback transaction và không upload bất kỳ file nào
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of error
                    await transaction.RollbackAsync();
                    throw new Exception("Cannot upload tour", ex);
                }
            }
        }



        public async Task UpdateTourAsync(int tourId, TourInfoView tourModel)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    // Lấy tour từ database
                    var tour = await _unitOfWork.TourRepository.GetByIDAsync(tourId);
                    if (tour == null)
                    {
                        throw new Exception($"Tour with ID {tourId} not found");
                    }

                    // Cập nhật thông tin tour
                    tour.CityId = tourModel.CityId;
                    tour.TourGuideId = tourModel.TourGuideId;
                    tour.Name = tourModel.Name;
                    tour.Description = tourModel.Description;
                    tour.Duration = tourModel.Duration;
                    // Cập nhật tour trong database
                    await _unitOfWork.TourRepository.UpdateAsync(tour);
                    await _unitOfWork.SaveAsync();

                    // Commit transaction
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Cannot update tour", ex);
                }
            }
        }

        public async Task UpdateTourStatusAsync(int tourId, TourStatusView tourModel)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var tour = await _unitOfWork.TourRepository.GetByIDAsync(tourId);
                    if (tour == null)
                    {
                        throw new Exception($"Tour with ID {tourId} not found");
                    }

                    tour.Status = tourModel.Status;

                    await _unitOfWork.TourRepository.UpdateAsync(tour);
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Cannot update status tour", ex);
                }
            }
        }

        public async Task DeleteTourAsync(int tourId)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var tour = await _unitOfWork.TourRepository.GetByIDAsync(tourId);
                    if (tour == null)
                    {
                        throw new Exception($"Tour with ID {tourId} not found");
                    }

                    // Lấy tất cả các ảnh liên quan đến tour
                    var tourImages = await _unitOfWork.TourImageRepository.GetAllAsync(ti => ti.TourId == tourId);

                    // Xóa các ảnh từ Google Drive
                    foreach (var tourImage in tourImages)
                    {
                        await _googleDriveService.DeleteFileAsync(tourImage.ImagePath, "1j6R0VaaZXFbruE553kdGyUrboAxfVw3o"); // Thay thế bằng parentFolderId thực tế
                    }

                    // Xóa các bản ghi TourImage từ cơ sở dữ liệu
                    await _unitOfWork.TourImageRepository.DeleteRangeAsync(tourImages);

                    // Xóa tour từ cơ sở dữ liệu
                    await _unitOfWork.TourRepository.DeleteAsync(tour);

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.SaveAsync();

                    // Commit transaction
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await transaction.RollbackAsync();
                    throw new Exception("Cannot delete tour", ex);
                }
            }
        }

        public async Task<List<AllToursView>> GetRandomToursAsync(string sessionId, int page, int pageSize)
        {
            try
            {
                var cacheKey = $"Tour_{sessionId}";

                if (!_cache.TryGetValue(cacheKey, out List<AllToursView> shuffledItems))
                {
                    var tours = (await _unitOfWork.TourRepository.GetAllAsync(filter: t => t.Status == 1, includeProperties: "TourGuide,City")).ToList();
                    shuffledItems = await GenerateShuffledTourList(tours);
                    _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(5));
                }

                var pagedItems = shuffledItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return pagedItems.Select(item => new AllToursView
                {
                    CityName = item.CityName,
                    Description = item.Description,
                    Duration = item.Duration,
                    Name = item.Name,
                    ThumbnailTourImage = item.ThumbnailTourImage,
                    CityId = item.CityId,
                    TourGuideId = item.TourGuideId,
                    TourId = item.TourId,
                    TourGuideName = item.Name
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AllToursView>> GetRandomToursInCityAsync(string sessionId, int CityId, int page, int pageSize)
        {
            try
            {
                var cacheKey = $"Tour_CityId:{CityId}_{sessionId}";

                if (!_cache.TryGetValue(cacheKey, out List<AllToursView> shuffledItems))
                {
                    var tours = (await _unitOfWork.TourRepository.GetAllAsync(filter: t => t.Status == 1 && t.CityId == CityId, includeProperties: "TourGuide,City")).ToList();
                    shuffledItems = await GenerateShuffledTourList(tours);
                    _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(5));
                }

                var pagedItems = shuffledItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return pagedItems.Select(item => new AllToursView
                {
                    CityName = item.CityName,
                    Description = item.Description,
                    Duration = item.Duration,
                    Name = item.Name,
                    ThumbnailTourImage = item.ThumbnailTourImage,
                    CityId = item.CityId,
                    TourGuideId = item.TourGuideId,
                    TourId = item.TourId,
                    TourGuideName = item.Name
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<AllToursView>> GenerateShuffledTourList(List<Tour> tours)
        {
            var items = new List<AllToursView>();

            foreach (var tour in tours)
            {
                var tourImage = (await _unitOfWork.TourImageRepository.FindAsync(t => t.TourId == tour.TourId)).FirstOrDefault();

                var item = new AllToursView
                {
                    CityName = tour.City.Name,
                    Description = tour.Description,
                    Duration = tour.Duration,
                    Name = tour.Name,
                    ThumbnailTourImage = tourImage == null ? null : await _googleDriveService.GetImageFromCacheOrDriveAsync(tourImage.ImagePath, "1j6R0VaaZXFbruE553kdGyUrboAxfVw3o"),
                    CityId = tour.CityId,
                    TourGuideId = tour.TourGuideId,
                    TourId = tour.TourId,
                    TourGuideName = $"{tour.TourGuide.LastName} {tour.TourGuide.FirstName}"
                };

                items.Add(item);
            }

            return items.OrderBy(x => _random.Next()).ToList();
        }

        public async Task<int> GetTotalPage(int pageSize, int? cityId, string sessionId)
        {
            try
            {
                string cacheKey;
                if (cityId == null)
                {
                    cacheKey = $"Tour_{sessionId}";
                }
                else
                {
                    cacheKey = $"Tour_CityId:{cityId}_{sessionId}";
                }

                if (_cache.TryGetValue(cacheKey, out List<AllToursView> shuffledItems))
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

        public async Task<int> GetTotalPageByTourGuide(int TourGuideId, int pageSize)
        {
            try
            {
                var totalPage = (await _unitOfWork.TourRepository.GetAsync(filter: t => t.TourGuideId == TourGuideId)).Count();
                return totalPage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetTourByIdView?> GetTourByIdAsync(int tourId)
        {
            try
            {
                var tour = await _unitOfWork.TourRepository.GetByIDAsync(tourId);

                if (tour == null)
                {
                    // Xử lý trường hợp không tìm thấy tour
                    return null;
                }

                // Load TourImages explicitly
                await _unitOfWork.TourRepository.LoadCollectionAsync(tour, f => f.TourImages);

                var tourView = new GetTourByIdView
                {
                    TourId = tour.TourId,
                    CityId = tour.CityId,
                    TourGuideId = tour.TourGuideId,
                    Name = tour.Name,
                    Description = tour.Description,
                    Duration = tour.Duration,
                    Status = tour.Status,
                };

                tourView.tourImgViewList = new List<TourImageView>();

                foreach (var image in tour.TourImages)
                {
                    var imageView = new TourImageView
                    {
                        ImagePath = await _googleDriveService.GetImageFromCacheOrDriveAsync(image.ImagePath, "1j6R0VaaZXFbruE553kdGyUrboAxfVw3o"),
                        UploadDate = image.UploadDate
                    };

                    tourView.tourImgViewList.Add(imageView);
                }

                return tourView;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AllToursView>> GetToursByTourGuideAsync(int TourGuideId, int page, int pageSize)
        {
            try
            {
                List<AllToursView> toursViews = new List<AllToursView>();
                var tours = await _unitOfWork.TourRepository.GetAsync(filter: t => t.TourGuideId == TourGuideId, includeProperties: "City,TourGuide", pageIndex: page, pageSize: pageSize);
                if (tours.Any())
                {
                    foreach (var tour in tours)
                    {
                        var thumbnail = (await _unitOfWork.TourImageRepository.GetAsync(t => t.TourId == tour.TourId)).FirstOrDefault();
                        var tourView = new AllToursView
                        {
                            CityId = tour.CityId,
                            TourGuideId = tour.TourGuideId,
                            CityName = tour.City.Name,
                            Name = tour.Name,
                            Description = tour.Description,
                            Duration = tour.Duration,
                            ThumbnailTourImage = thumbnail == null ? null : await _googleDriveService.GetImageFromCacheOrDriveAsync(thumbnail.ImagePath, "1j6R0VaaZXFbruE553kdGyUrboAxfVw3o"),
                            TourId = tour.TourId,
                            TourGuideName = $"{tour.TourGuide.LastName} {tour.TourGuide.FirstName}"
                        };
                        toursViews.Add(tourView);
                    }
                }
                return toursViews;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GetTourByStatusView>> GetToursByStatusAsync(int status)
        {
            try
            {
                // Lấy danh sách tour dựa trên trạng thái
                var tours = await _unitOfWork.TourRepository.GetAllAsync(
                    filter: t => t.Status == status,
                    includeProperties: "TourGuide,City,TourImages"
                );

                var tourViews = new List<GetTourByStatusView>();

                foreach (var tour in tours)
                {
                    var tourView = new GetTourByStatusView
                    {
                        TourId = tour.TourId,
                        CityId = tour.CityId,
                        CityName = tour.City?.Name,
                        TourGuideId = tour.TourGuideId,
                        TourGuideName = tour.TourGuide != null ? $"{tour.TourGuide.LastName} {tour.TourGuide.FirstName}" : string.Empty,
                        Name = tour.Name,
                        Description = tour.Description,
                        Duration = tour.Duration,
                        Status = tour.Status,
                        tourImgViewList = new List<TourImageView>()
                    };

                    foreach (var image in tour.TourImages)
                    {
                        var imageView = new TourImageView
                        {
                            ImagePath = await _googleDriveService.GetImageFromCacheOrDriveAsync(image.ImagePath, "1j6R0VaaZXFbruE553kdGyUrboAxfVw3o"),
                            UploadDate = image.UploadDate
                        };

                        tourView.tourImgViewList.Add(imageView);
                    }

                    // Thêm tour vào danh sách
                    tourViews.Add(tourView);
                }

                return tourViews;
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot get tours by status", ex);
            }
        }
    }
}
