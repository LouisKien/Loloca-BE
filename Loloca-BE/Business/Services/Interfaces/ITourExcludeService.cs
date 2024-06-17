using Loloca_BE.Business.Models.TourExcludeView;
using Loloca_BE.Business.Models.TourHighlightView;
using Loloca_BE.Business.Models.TourTypeView;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface ITourExcludeService
    {

        Task<TourExcludeDTO> GetTourExcludeByIdAsync(int tourExcludeId);
        Task<int> CreateTourExcludeAsync(TourExcludeDTO tourExclude);
        Task UpdateTourExcludeAsync(TourExcludeDTO tourExclude);
        Task DeleteTourExcludeAsync(int tourExcludeId);

        Task<List<TourExcludeDTO>> GetAllTourExcludeAsync();

    }
}
