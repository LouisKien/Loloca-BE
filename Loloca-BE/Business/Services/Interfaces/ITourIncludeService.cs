using Loloca_BE.Business.Models.TourHighlightView;
using Loloca_BE.Business.Models.TourIncludeView;
using Loloca_BE.Business.Models.TourTypeView;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface ITourIncludeService
    {

        Task<TourIncludeDTO> GetTourIncludeByIdAsync(int tourIncludeId);
        Task<int> CreateTourIncludeAsync(TourIncludeDTO tourInclude);
        Task UpdateTourIncludeAsync(TourIncludeDTO tourInclude);
        Task DeleteTourIncludeAsync(int tourIncludeId);
        Task<List<TourIncludeDTO>> GetAllTourIncludeAsync();
    }
}
