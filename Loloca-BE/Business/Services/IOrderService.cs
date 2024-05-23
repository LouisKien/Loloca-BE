using Loloca_BE.Business.Models.OrderView;

namespace Loloca_BE.Business.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderModelView>> GetAllOrdersAsync();
        Task<OrderModelView?> GetOrderByIdAsync(int id);
        Task<OrderModelView> CreateOrderForBookingTourGuideRequestAsync(OrderModelView orderModel);
        Task<OrderModelView> CreateOrderForBookingTourRequestAsync(OrderModelView orderModel);
        Task<OrderModelView> UpdateOrderStatusAsync(int id, int status);
    }
}
