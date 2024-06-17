using Loloca_BE.Business.Models.TourHighlightView;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface ITourHighlightService
    {
        Task<TourHighlightDTO> GetTourHighlightByIdAsync(int highlightId);
        Task<int> CreateTourHighlightAsync(TourHighlightDTO tourHighlight);
        Task UpdateTourHighlightAsync(TourHighlightDTO tourHighlight);
        Task DeleteTourHighlightAsync(int highlightId);

        Task<List<TourHighlightDTO>> GetAllTourHighlightsAsync();
    }
}
