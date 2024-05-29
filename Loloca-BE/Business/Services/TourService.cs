using AutoMapper;
using Loloca_BE.Business.Models.TourView;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System.Transactions;

namespace Loloca_BE.Business.Services
{
    public class TourService : ITourService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMemoryCache _cache;

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

            using (var transaction = _unitOfWork.BeginTransaction())
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

                    // Save FeedbackImages to database
                    if (tourImages.Count > 0)
                    {
                        await _unitOfWork.TourImageRepository.AddRangeAsync(tourImages);
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
                    if(tour == null)
                    {
                        throw new Exception($"Tour with ID {tourId} not found");
                    }

                    tour.Status = tourModel.Status;

                    await _unitOfWork.TourRepository.UpdateAsync(tour);
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                }catch (Exception ex)
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
    }
}
