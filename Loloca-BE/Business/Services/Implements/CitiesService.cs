using AutoMapper;
using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Business.Models.TourGuideView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Loloca_BE.Business.Services.Implements
{
    public class CitiesService : ICitiesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IGoogleDriveService _googleDriveService;
        public CitiesService(IUnitOfWork unitOfWork, IMapper mapper, IGoogleDriveService googleDriveService, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _googleDriveService = googleDriveService;
            _cache = cache;

        }

        public async Task<IEnumerable<CityView>> GetAllCitiesAsync()
        {
            try
            {
                var citiesList = await _unitOfWork.CityRepository.GetAsync();

                // Lọc các thành phố có Status là false
                var inactiveCitiesList = citiesList.Where(city => city.Status == true).ToList();

                var cityViews = new List<CityView>();

                foreach (var city in inactiveCitiesList)
                {
                    byte[]? bannerContent = await _googleDriveService.GetImageFromCacheOrDriveAsync(city.CityBanner, "1kdBJlFETXvGNGSSHLNi59M7LkCIOZAyl");
                    byte[]? thumbnailContent = await _googleDriveService.GetImageFromCacheOrDriveAsync(city.CityThumbnail, "13znRfA2TpsdO5Ro-_cWPFZ63stCoy6av");

                    cityViews.Add(new CityView
                    {
                        CityId = city.CityId,
                        Name = city.Name,
                        CityBanner = bannerContent,
                        CityBannerUploadDate = city.CityBannerUploadDate,
                        CityThumbnail = thumbnailContent,
                        CityThumbnailUploadDate = city.CityThumbnailUploadDate,
                        Status = city.Status,
                    });
                }

                return cityViews;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the cities.", ex);
            }
        }


        public async Task<CityView> GetCityByIdAsync(int CityId)
        {
            try
            {
                var city = await _unitOfWork.CityRepository.GetByIDAsync(CityId);
                if (city == null)
                {
                    throw new Exception("City not found.");
                }

                // Kiểm tra trạng thái của thành phố
                if (city.Status == false)
                {
                    throw new Exception("City is inactive.");
                }

                byte[]? bannerContent = await _googleDriveService.GetImageFromCacheOrDriveAsync(city.CityBanner, "1kdBJlFETXvGNGSSHLNi59M7LkCIOZAyl");
                byte[]? thumbnailContent = await _googleDriveService.GetImageFromCacheOrDriveAsync(city.CityThumbnail, "13znRfA2TpsdO5Ro-_cWPFZ63stCoy6av");

                return new CityView
                {
                    CityId = CityId,
                    Name = city.Name,
                    CityBanner = bannerContent,
                    CityBannerUploadDate = city.CityBannerUploadDate,
                    CityThumbnail = thumbnailContent,
                    CityThumbnailUploadDate = city.CityThumbnailUploadDate,
                    Status = city.Status,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<CreateCity> AddCityAsync(CreateCity cityView)
        {
            try
            {
                var city = _mapper.Map<City>(cityView);
                city.Status = true;

                await _unitOfWork.CityRepository.InsertAsync(city);
                await _unitOfWork.SaveAsync();
                return _mapper.Map<CreateCity>(city);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the city. Error message: {ex.Message}");
            }
        }

        public async Task<UpdateCityView?> UpdateCityAsync(int id, UpdateCityView cityView)
        {
            try
            {
                var existingCity = await _unitOfWork.CityRepository.GetByIDAsync(id);
                if (existingCity == null)
                {
                    return null;
                }

                _mapper.Map(cityView, existingCity);
                await _unitOfWork.CityRepository.UpdateAsync(existingCity);
                await _unitOfWork.SaveAsync();

                return _mapper.Map<UpdateCityView>(existingCity);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the city. Error message: {ex.Message}");
            }
        }

        public async Task<bool> DeleteCityAsync(int id)
        {
            try
            {
                var city = await _unitOfWork.CityRepository.GetByIDAsync(id);
                if (city == null)
                {
                    return false;
                }
                // Thay đổi trạng thái của thành phố thành false
                city.Status = false;

                // Cập nhật thành phố trong cơ sở dữ liệu
                await _unitOfWork.CityRepository.UpdateAsync(city);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Ghi lại chi tiết lỗi nội bộ nếu có
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception($"An error occurred while deleting the city. Error message: {innerExceptionMessage}");
            }
        }



        public async Task UploadCityBannerAsync(IFormFile file, int CityId)
        {
            string parentFolderId = "1kdBJlFETXvGNGSSHLNi59M7LkCIOZAyl";

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new InvalidDataException("Only image files are allowed.");
            }

            var city = await _unitOfWork.CityRepository.GetByIDAsync(CityId);
            if (city == null)
            {
                throw new Exception($"City with CityId {CityId} doesn't exist");
            }

            try
            {
                // If an avatar exists, delete the old one
                if (!city.CityBanner.IsNullOrEmpty() && city.CityBannerUploadDate.HasValue)
                {
                    await _googleDriveService.DeleteFileAsync(city.CityBanner, parentFolderId);
                }

                string guid = Guid.NewGuid().ToString();
                string fileName = $"CityBanner_City_{CityId}_{guid}";

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

                city.CityBanner = fileName;
                city.CityBannerUploadDate = DateTime.Now;

                await _unitOfWork.CityRepository.UpdateAsync(city);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot update city");
            }
        }

        public async Task UploadCityThumbnailAsync(IFormFile file, int CityId)
        {
            string parentFolderId = "13znRfA2TpsdO5Ro-_cWPFZ63stCoy6av";

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new InvalidDataException("Only image files are allowed.");
            }

            var city = await _unitOfWork.CityRepository.GetByIDAsync(CityId);
            if (city == null)
            {
                throw new Exception($"City with CityId {CityId} doesn't exist");
            }

            try
            {
                // If an avatar exists, delete the old one
                if (!city.CityThumbnail.IsNullOrEmpty() && city.CityThumbnailUploadDate.HasValue)
                {
                    await _googleDriveService.DeleteFileAsync(city.CityThumbnail, parentFolderId);
                }

                string guid = Guid.NewGuid().ToString();
                string fileName = $"CityThumbnail_City_{CityId}_{guid}";

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

                city.CityThumbnail = fileName;
                city.CityThumbnailUploadDate = DateTime.Now;

                await _unitOfWork.CityRepository.UpdateAsync(city);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot update city");
            }
        }
    }
}
