﻿using Loloca_BE.Business.Models.OrderView;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderModelView>> GetAllOrdersAsync();
        Task<OrderModelView?> GetOrderByIdAsync(int id);
        Task<OrderForBookingTourGuideView> CreateOrderForBookingTourGuideRequestAsync(OrderForBookingTourGuideView orderModel);
        Task<OrderForBookingTourView> CreateOrderForBookingTourRequestAsync(OrderForBookingTourView orderModel);
        Task<OrderModelView> UpdateOrderStatusAsync(UpdateOrderStatusView updateOrderStatusView);
    }
}
