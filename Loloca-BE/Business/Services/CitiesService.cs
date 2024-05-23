using AutoMapper;
using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories;

namespace Loloca_BE.Business.Services
{
    public class CitiesService : ICitiesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CitiesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CityView>> GetAllCitiesAsync()
        {
            try
            {
                var citiesList = await _unitOfWork.CityRepository.GetAsync();
                return _mapper.Map<IEnumerable<CityView>>(citiesList);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the cities.", ex);
            }
        }

        public async Task<CityView> GetCityByIdAsync(int id)
        {
            try
            {
                var city = await _unitOfWork.CityRepository.GetByIDAsync(id);
                if (city == null)
                {
                    return null;
                }
                return _mapper.Map<CityView>(city);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the city.", ex);
            }
        }

        public async Task<CityView> AddCityAsync(CityView cityView)
        {
            try
            {
                var city = _mapper.Map<City>(cityView);
                await _unitOfWork.CityRepository.InsertAsync(city);
                await _unitOfWork.SaveAsync();
                return _mapper.Map<CityView>(city);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the city. Error message: {ex.Message}");
            }
        }

        public async Task<CityView?> UpdateCityAsync(int id, CityView cityView)
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

                return _mapper.Map<CityView>(existingCity);
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

                await _unitOfWork.CityRepository.DeleteAsync(city);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the city. Error message: {ex.Message}");
            }
        }

    }
}
