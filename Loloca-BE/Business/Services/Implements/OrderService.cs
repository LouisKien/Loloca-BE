using AutoMapper;
using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Business.Models.OrderView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.Services.Implements
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
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIDAsync(id);
                return _mapper.Map<OrderModelView>(order);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<OrderForBookingTourGuideView> CreateOrderForBookingTourGuideRequestAsync(OrderForBookingTourGuideView orderModel)
        {
            try
            {
                // Get the booking tour guide request to retrieve the TotalPrice
                var bookingTourGuideRequest = await _unitOfWork.BookingTourGuideRepository.GetByIDAsync(orderModel.BookingTourGuideRequestId);
                if (bookingTourGuideRequest == null)
                {
                    throw new Exception("Không tìm thấy thông tin yêu cầu đặt hướng dẫn viên.");
                }

                // Validate that the booking tour guide request status is 1
                if (bookingTourGuideRequest.Status != 1)
                {
                    throw new Exception("Yêu cầu đặt hướng dẫn viên không hợp lệ. Trạng thái phải là 1.");
                }

                // Map OrderForBookingTourGuideView to Order and set OrderPrice and OrderCode
                var order = _mapper.Map<Order>(orderModel);
                order.OrderPrice = bookingTourGuideRequest.TotalPrice; // Assign TotalPrice to OrderPrice
                order.OrderCode = Guid.NewGuid().ToString(); // Assign a new GUID to OrderCode
                order.CreateAt = DateTime.Now; // Set CreatedAt to DateTime.Now
                order.CustomerId = bookingTourGuideRequest.CustomerId; // Assign CustomerId from BookingTourGuideRequest

                using (var unitOfWork = _unitOfWork.BeginTransaction())
                {
                    // Get customer information
                    var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(order.CustomerId);
                    if (customer == null)
                    {
                        throw new Exception("Không tìm thấy thông tin khách hàng.");
                    }

                    var orderPrice = bookingTourGuideRequest.TotalPrice;

                    // Check if the customer has enough balance
                    if (customer.Balance == null)
                    {
                        customer.Balance = 0;
                    }

                    if (customer.Balance >= orderPrice)
                    {
                        // Deduct the order price from customer's balance
                        customer.Balance -= orderPrice;
                        order.Status = 1; // Accepted
                    }
                    else
                    {
                        order.Status = 0; // Rejected due to insufficient balance
                    }

                    await _unitOfWork.OrderRepository.InsertAsync(order);
                    await _unitOfWork.SaveAsync();

                    // Update customer's balance
                    await _unitOfWork.CustomerRepository.UpdateAsync(customer);

                    // Create a notification for the customer
                    var notification = new Notification
                    {
                        UserId = customer.CustomerId,
                        UserType = "Customer",
                        Title = order.Status == 1 ? "Yêu cầu đặt thành công" : "Yêu cầu đặt thất bại",
                        Message = order.Status == 1
                            ? $"Đơn của bạn với mã {order.OrderCode} đã được chấp nhận. Số tiền {orderPrice} đã được trừ vào số dư của bạn."
                            : $"Đơn của bạn với mã {order.OrderCode} đã vào trạng thái chờ do số dư không đủ.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.NotificationRepository.InsertAsync(notification);

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
                // Get the booking tour guide request to retrieve the TotalPrice
                var bookingTourRequest = await _unitOfWork.BookingTourRequestRepository.GetByIDAsync(orderModel.BookingTourRequestsId);
                if (bookingTourRequest == null)
                {
                    throw new Exception("Không tìm thấy thông tin yêu cầu đặt chuyến du lịch.");
                }

                // Validate that the booking tour guide request status is 1
                if (bookingTourRequest.Status != 1)
                {
                    throw new Exception("Yêu cầu đặt hướng dẫn viên không hợp lệ. Trạng thái phải là 1.");
                }


                var order = _mapper.Map<Order>(orderModel);
                order.OrderPrice = bookingTourRequest.TotalPrice; // Assign TotalPrice to OrderPrice
                order.OrderCode = Guid.NewGuid().ToString(); // Assign a new GUID to OrderCode
                order.CreateAt = DateTime.Now; // Set CreatedAt to DateTime.Now
                order.CustomerId = bookingTourRequest.CustomerId; 

                using (var unitOfWork = _unitOfWork.BeginTransaction())
                {
                    // Get customer information
                    var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(order.CustomerId);
                    if (customer == null)
                    {
                        throw new Exception("Không tìm thấy thông tin khách hàng.");
                    }

                    var orderPrice = bookingTourRequest.TotalPrice;

                    // Check if the customer has enough balance
                    if (customer.Balance == null)
                    {
                        customer.Balance = 0;
                    }

                    if (customer.Balance >= orderPrice)
                    {
                        // Deduct the order price from customer's balance
                        customer.Balance -= orderPrice;
                        order.Status = 1; // Accepted
                    }
                    else
                    {
                        order.Status = 0; // Rejected due to insufficient balance
                    }

                    await _unitOfWork.OrderRepository.InsertAsync(order);
                    await _unitOfWork.SaveAsync();

                    // Update customer's balance
                    await _unitOfWork.CustomerRepository.UpdateAsync(customer);

                    // Create a notification for the customer
                    var notification = new Notification
                    {
                        UserId = customer.CustomerId,
                        UserType = "Customer",
                        Title = order.Status == 1 ? "Yêu cầu đặt thành công" : "Yêu cầu đặt thất bại",
                        Message = order.Status == 1
                            ? $"Đơn của bạn với mã {order.OrderCode} đã được chấp nhận. Số tiền {orderPrice} đã được trừ vào số dư của bạn."
                            : $"Đơn của bạn với mã {order.OrderCode} đã vào trạng thái chờ do số dư không đủ.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.NotificationRepository.InsertAsync(notification);

                    await unitOfWork.CommitAsync();
                }

                return _mapper.Map<OrderForBookingTourView>(order);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create order.", ex);
            }
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
