using AutoMapper;
using Loloca_BE.Business.Models.TourHighlightView;
using Loloca_BE.Business.Models.TourItineraryView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.Services.Implements
{
    public class TourItineraryService : ITourItineraryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TourItineraryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        public async Task<TourItineraryDTO> GetTourItineraryByIdAsync(int tourItineraryId)
        {
            var Entity = await _unitOfWork.TourItineraryRepository.GetByIDAsync(tourItineraryId);
            if (Entity == null)
            {
                return null; // or throw an exception, depending on your business logic
            }

            return _mapper.Map<TourItineraryDTO>(Entity);
        }

        public async Task<int> CreateTourItineraryAsync(TourItineraryDTO tourItinerary)
        {
            var Entity = new TourItinerary
            {
                TourId = tourItinerary.TourId,
                Name = tourItinerary.Name,
                Description = tourItinerary.Description,
            };

            await _unitOfWork.TourItineraryRepository.InsertAsync(Entity);
            await _unitOfWork.SaveAsync();

            return Entity.ItineraryId;

        }
        public async Task UpdateTourItineraryAsync(TourItineraryDTO tourItinerary)
        {
            // Lấy entity hiện tại từ repository dựa trên highlightId
            var existing = await _unitOfWork.TourItineraryRepository.GetByIDAsync(tourItinerary.ItineraryId);
            if (existing == null)
            {
                throw new Exception($"Tour itinerary with ID {tourItinerary.ItineraryId} not found");
            }

            // Cập nhật các thuộc tính của entity hiện tại từ DTO
            existing.TourId = tourItinerary.TourId;
            existing.Name = tourItinerary.Name;
            existing.Description = tourItinerary.Description;
            await _unitOfWork.TourItineraryRepository.UpdateAsync(existing);
            await _unitOfWork.SaveAsync();
        }



        public async Task DeleteTourItineraryAsync(int tourItineraryId)
        {
            var existing = await _unitOfWork.TourItineraryRepository.GetByIDAsync(tourItineraryId);
            if (existing == null)
            {
                throw new Exception($"Tour Itinerary with ID {tourItineraryId} not found");
            }

            await _unitOfWork.TourItineraryRepository.DeleteAsync(existing);
            await _unitOfWork.SaveAsync();
        }


        public async Task<List<TourItineraryDTO>> GetAllTourItineraryAsync()
        {
            var highlightEntities = await _unitOfWork.TourItineraryRepository.GetAllAsync();
            return _mapper.Map<List<TourItineraryDTO>>(highlightEntities);
        }
    }
}
