using Loloca_BE.Business.Models.OrderView;

namespace Loloca_BE.Business.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderModelView>> GetAllOrdersAsync();
        Task<OrderModelView?> GetOrderByIdAsync(int id);
        Task<OrderForBookingTourGuideView> CreateOrderForBookingTourGuideRequestAsync(OrderForBookingTourGuideView orderModel);
        Task<OrderForBookingTourView> CreateOrderForBookingTourRequestAsync(OrderForBookingTourView orderModel);
        Task<OrderModelView> UpdateOrderStatusAsync(int id, int status);
    }
}
