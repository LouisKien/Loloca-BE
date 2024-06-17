using AutoMapper;
using Loloca_BE.Business.Models.TourHighlightView;
using Loloca_BE.Business.Models.TourTypeView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.Services.Implements
{
    public class TourTypeService : ITourTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TourTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        public async Task<TourTypeDTO> GetTourTypeByIdAsync(int typeId)
        {
            var Entity = await _unitOfWork.TourTypeRepository.GetByIDAsync(typeId);
            if (Entity == null)
            {
                return null; // or throw an exception, depending on your business logic
            }

            return _mapper.Map<TourTypeDTO>(Entity);
        }

        public async Task<int> CreateTourTypeAsync(TourTypeDTO tourType)
        {
            var Entity = new TourType
            {
                TourId = tourType.TourId,
                TypeDetail = tourType.TypeDetail
            };

            await _unitOfWork.TourTypeRepository.InsertAsync(Entity);
            await _unitOfWork.SaveAsync();

            return Entity.TypeId;
        }


        public async Task UpdateTourTypeAsync(TourTypeDTO tourType)
        {
            // Lấy entity hiện tại từ repository dựa trên highlightId
            var existing = await _unitOfWork.TourTypeRepository.GetByIDAsync(tourType.TypeId);
            if (existing == null)
            {
                throw new Exception($"Tour type with ID {tourType.TypeId} not found");
            }

            // Cập nhật các thuộc tính của entity hiện tại từ DTO
            existing.TourId = tourType.TourId;
            existing.TypeDetail = tourType.TypeDetail;

            await _unitOfWork.TourTypeRepository.UpdateAsync(existing);
            await _unitOfWork.SaveAsync();
        }



        public async Task DeleteTourTypeAsync(int typeId)
        {
            var existing = await _unitOfWork.TourTypeRepository.GetByIDAsync(typeId);
            if (existing == null)
            {
                throw new Exception($"Tour type with ID {typeId} not found");
            }

            await _unitOfWork.TourTypeRepository.DeleteAsync(existing);
            await _unitOfWork.SaveAsync();
        }


        public async Task<List<TourTypeDTO>> GetAllTourTypeAsync()
        {
            var highlightEntities = await _unitOfWork.TourTypeRepository.GetAllAsync();
            return _mapper.Map<List<TourTypeDTO>>(highlightEntities);
        }
    }
}
