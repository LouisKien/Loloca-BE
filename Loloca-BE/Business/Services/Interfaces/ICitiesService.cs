using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface ICitiesService
    {
        Task<IEnumerable<CityView>> GetAllCitiesAsync();
        Task<CityView> GetCityByIdAsync(int id);
        Task<CityView> AddCityAsync(CityView cityView);
        Task<CityView> UpdateCityAsync(int id, CityView cityView);
        Task<bool> DeleteCityAsync(int id);


    }
}
