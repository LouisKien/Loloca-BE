using AutoMapper;
using Loloca_BE.Business.Models.TourExcludeView;
using Loloca_BE.Business.Models.TourHighlightView;
using Loloca_BE.Business.Models.TourIncludeView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.Services.Implements
{
    public class TourIncludeService : ITourIncludeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TourIncludeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        public async Task<TourIncludeDTO> GetTourIncludeByIdAsync(int tourIncludeId)
        {
            var Entity = await _unitOfWork.TourIncludeRepository.GetByIDAsync(tourIncludeId);
            if (Entity == null)
            {
                return null; // or throw an exception, depending on your business logic
            }

            return _mapper.Map<TourIncludeDTO>(Entity);
        }

        public async Task<int> CreateTourIncludeAsync(TourIncludeDTO tourInclude)
        {
            var Entity = new TourInclude
            {
                TourId = tourInclude.TourId,
                IncludeDetail = tourInclude.IncludeDetail
            };

            await _unitOfWork.TourIncludeRepository.InsertAsync(Entity);
            await _unitOfWork.SaveAsync();

            return Entity.IncludeId;

        }
        public async Task UpdateTourIncludeAsync(TourIncludeDTO tourInclude)
        {
            // Lấy entity hiện tại từ repository dựa trên highlightId
            var existing = await _unitOfWork.TourIncludeRepository.GetByIDAsync(tourInclude.IncludeId);
            if (existing == null)
            {
                throw new Exception($"Tour include with ID {tourInclude.IncludeId} not found");
            }

            // Cập nhật các thuộc tính của entity hiện tại từ DTO
            existing.TourId = tourInclude.TourId;
            existing.IncludeDetail = tourInclude.IncludeDetail;

            await _unitOfWork.TourIncludeRepository.UpdateAsync(existing);
            await _unitOfWork.SaveAsync();
        }



        public async Task DeleteTourIncludeAsync(int tourIncludeId)
        {
            var existing = await _unitOfWork.TourIncludeRepository.GetByIDAsync(tourIncludeId);
            if (existing == null)
            {
                throw new Exception($"Tour include with ID {tourIncludeId} not found");
            }

            await _unitOfWork.TourIncludeRepository.DeleteAsync(existing);
            await _unitOfWork.SaveAsync();
        }


        public async Task<List<TourIncludeDTO>> GetAllTourIncludeAsync()
        {
            var highlightEntities = await _unitOfWork.TourIncludeRepository.GetAllAsync();
            return _mapper.Map<List<TourIncludeDTO>>(highlightEntities);
        }
    }
}
