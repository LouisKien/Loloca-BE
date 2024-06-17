using AutoMapper;
using Loloca_BE.Business.Models.TourExcludeView;
using Loloca_BE.Business.Models.TourHighlightView;
using Loloca_BE.Business.Models.TourTypeView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.Services.Implements
{
    public class TourExcludeService : ITourExcludeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TourExcludeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        public async Task<TourExcludeDTO> GetTourExcludeByIdAsync(int tourExcludeId)
        {
            var Entity = await _unitOfWork.TourExcludeRepository.GetByIDAsync(tourExcludeId);
            if (Entity == null)
            {
                return null; // or throw an exception, depending on your business logic
            }

            return _mapper.Map<TourExcludeDTO>(Entity);
        }

        public async Task<int> CreateTourExcludeAsync(TourExcludeDTO tourExclude)
        {
            var Entity = new TourExclude
            {
                TourId = tourExclude.TourId,
                ExcludeDetail = tourExclude.ExcludeDetail
            };

            await _unitOfWork.TourExcludeRepository.InsertAsync(Entity);
            await _unitOfWork.SaveAsync();

            return Entity.ExcludeId;
        }


        public async Task UpdateTourExcludeAsync(TourExcludeDTO tourExclude)
        {
            // Lấy entity hiện tại từ repository dựa trên highlightId
            var existing = await _unitOfWork.TourExcludeRepository.GetByIDAsync(tourExclude.ExcludeId);
            if (existing == null)
            {
                throw new Exception($"Tour exclude with ID {tourExclude.ExcludeId} not found");
            }

            // Cập nhật các thuộc tính của entity hiện tại từ DTO
            existing.TourId = tourExclude.TourId;
            existing.ExcludeDetail = tourExclude.ExcludeDetail;

            await _unitOfWork.TourExcludeRepository.UpdateAsync(existing);
            await _unitOfWork.SaveAsync();
        }



        public async Task DeleteTourExcludeAsync(int tourExcludeId)
        {
            var existing = await _unitOfWork.TourExcludeRepository.GetByIDAsync(tourExcludeId);
            if (existing == null)
            {
                throw new Exception($"Tour exclude with ID {tourExcludeId} not found");
            }

            await _unitOfWork.TourExcludeRepository.DeleteAsync(existing);
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<TourExcludeDTO>> GetAllTourExcludeAsync()
        {
            var highlightEntities = await _unitOfWork.TourExcludeRepository.GetAllAsync();
            return _mapper.Map<List<TourExcludeDTO>>(highlightEntities);
        }
    }
}
