using Loloca_BE.Business.Models.TourHighlightView;
using Loloca_BE.Business.Models.TourItineraryView;
using Loloca_BE.Business.Models.TourTypeView;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface ITourTypeService
    {

        Task<TourTypeDTO> GetTourTypeByIdAsync(int typeId);
        Task<int> CreateTourTypeAsync(TourTypeDTO tourType);
        Task UpdateTourTypeAsync(TourTypeDTO tourType);
        Task DeleteTourTypeAsync(int typeId);

        Task<List<TourTypeDTO>> GetAllTourTypeAsync();

    }
}
