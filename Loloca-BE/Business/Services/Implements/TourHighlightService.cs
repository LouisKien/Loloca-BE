using AutoMapper;
using Loloca_BE.Business.Models.TourHighlightView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Loloca_BE.Business.Services.Implements
{
    public class TourHighlightService : ITourHighlightService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TourHighlightService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        public async Task<TourHighlightDTO> GetTourHighlightByIdAsync(int highlightId)
        {
            var highlightEntity = await _unitOfWork.TourHighlightRepository.GetByIDAsync(highlightId);
            if (highlightEntity == null)
            {
                return null; // or throw an exception, depending on your business logic
            }

            return _mapper.Map<TourHighlightDTO>(highlightEntity);
        }

        public async Task<int> CreateTourHighlightAsync(TourHighlightDTO tourHighlight)
        {
            var highlightEntity = new TourHighlight
            {
                TourId = tourHighlight.TourId,
                HighlightDetail = tourHighlight.HighlightDetail
            };

            await _unitOfWork.TourHighlightRepository.InsertAsync(highlightEntity);
            await _unitOfWork.SaveAsync();

            return highlightEntity.HighlightId;
        }


        public async Task UpdateTourHighlightAsync(TourHighlightDTO tourHighlight)
        {
            // Lấy entity hiện tại từ repository dựa trên highlightId
            var existingHighlight = await _unitOfWork.TourHighlightRepository.GetByIDAsync(tourHighlight.HighlightId);
            if (existingHighlight == null)
            {
                throw new Exception($"Tour highlight with ID {tourHighlight.HighlightId} not found");
            }

            // Cập nhật các thuộc tính của entity hiện tại từ DTO
            existingHighlight.TourId = tourHighlight.TourId;
            existingHighlight.HighlightDetail = tourHighlight.HighlightDetail;

            await _unitOfWork.TourHighlightRepository.UpdateAsync(existingHighlight);
            await _unitOfWork.SaveAsync();
        }



        public async Task DeleteTourHighlightAsync(int highlightId)
        {
            var existingHighlight = await _unitOfWork.TourHighlightRepository.GetByIDAsync(highlightId);
            if (existingHighlight == null)
            {
                throw new Exception($"Tour highlight with ID {highlightId} not found");
            }

            await _unitOfWork.TourHighlightRepository.DeleteAsync(existingHighlight);
            await _unitOfWork.SaveAsync();
        }


        public async Task<List<TourHighlightDTO>> GetAllTourHighlightsAsync()
        {
            var highlightEntities = await _unitOfWork.TourHighlightRepository.GetAllAsync();
            return _mapper.Map<List<TourHighlightDTO>>(highlightEntities);
        }
    }
}
