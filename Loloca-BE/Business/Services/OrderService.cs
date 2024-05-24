using AutoMapper;
using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Business.Models.OrderView;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories;

namespace Loloca_BE.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderModelView>> GetAllOrdersAsync()
        {
            try
            {
                var orderList = await _unitOfWork.OrderRepository.GetAsync();
                return _mapper.Map<IEnumerable<OrderModelView>>(orderList);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the order.", ex);
            }
        }




        public async Task<OrderModelView?> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIDAsync(id);
            return _mapper.Map<OrderModelView>(order);
        }

        public async Task<OrderForBookingTourGuideView> CreateOrderForBookingTourGuideRequestAsync(OrderForBookingTourGuideView orderModel)
        {
            try
            {

                var order = _mapper.Map<Order>(orderModel);

                // Set CreatedAt to DateTime.Now
                order.CreateAt = DateTime.Now;

                // Use a using statement to create a new scope for the DbContext
                using (var unitOfWork = _unitOfWork.BeginTransaction())
                {
                    await _unitOfWork.OrderRepository.InsertAsync(order);
                    await _unitOfWork.SaveAsync();
                    await unitOfWork.CommitAsync();
                }

                return _mapper.Map<OrderForBookingTourGuideView>(order);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create order.", ex);
            }
        }


        public async Task<OrderForBookingTourView> CreateOrderForBookingTourRequestAsync(OrderForBookingTourView orderModel)
        {
            try
            {

                var order = _mapper.Map<Order>(orderModel);

                // Set CreatedAt to DateTime.Now
                order.CreateAt = DateTime.Now;

                // Use a using statement to create a new scope for the DbContext
                using (var unitOfWork = _unitOfWork.BeginTransaction())
                {
                    await _unitOfWork.OrderRepository.InsertAsync(order);
                    await _unitOfWork.SaveAsync();
                    await unitOfWork.CommitAsync();
                }

                return _mapper.Map<OrderForBookingTourView>(order);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create order.", ex);
            };
        }


        public async Task<OrderModelView> UpdateOrderStatusAsync(int id, int status)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var order = await _unitOfWork.OrderRepository.GetByIDAsync(id);
                    if (order == null)
                    {
                        throw new KeyNotFoundException("Order not found");
                    }

                    order.Status = status;

                    // Set UpdatedAt to DateTime.Now
                    order.CreateAt = DateTime.Now;

                   await _unitOfWork.OrderRepository.UpdateAsync(order);
                    await _unitOfWork.SaveAsync();

                    await transaction.CommitAsync();

                    return _mapper.Map<OrderModelView>(order);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Failed to update order status", ex);
                }
            }
        }

    }
}
