using Loloca_BE.Business.Models.TourHighlightView;
using Loloca_BE.Business.Models.TourItineraryView;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface ITourItineraryService
    {
        Task<TourItineraryDTO> GetTourItineraryByIdAsync(int tourItineraryId);
        Task<int> CreateTourItineraryAsync(TourItineraryDTO tourItinerary);
        Task UpdateTourItineraryAsync(TourItineraryDTO tourItinerary);
        Task DeleteTourItineraryAsync(int tourItineraryId);


        Task<List<TourItineraryDTO>> GetAllTourItineraryAsync();
      
    }
}
