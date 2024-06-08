using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface ICitiesService
    {
        Task<IEnumerable<CityView>> GetAllCitiesAsync();
        Task<CityView> GetCityByIdAsync(int CityId);
        Task<CreateCity> AddCityAsync(CreateCity cityView);
        Task<UpdateCityView> UpdateCityAsync(int id, UpdateCityView cityView);
        Task<bool> DeleteCityAsync(int id);

        Task UploadCityBannerAsync(IFormFile file, int CityId);
        Task UploadCityThumbnailAsync(IFormFile file, int CityId);
    }
}
