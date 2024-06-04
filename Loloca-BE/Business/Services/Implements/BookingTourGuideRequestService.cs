using AutoMapper;
using Loloca_BE.Business.Models.BookingTourGuideRequestModelView;
using Loloca_BE.Business.Models.BookingTourRequestModelView;
using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.Services.Implements
{
    public class BookingTourGuideRequestService : IBookingTourGuideRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public BookingTourGuideRequestService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }
        public async Task<BookingTourGuideRequest> AddBookingTourGuideRequestAsync(BookingTourGuideRequestView model)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var bookingRequest = _mapper.Map<BookingTourGuideRequest>(model);
                    bookingRequest.RequestDate = DateTime.Now;
                    bookingRequest.RequestTimeOut = bookingRequest.RequestDate.AddDays(7); // Cộng thêm 20 phút từ RequestDate
                    bookingRequest.Status = 1;
                    await _unitOfWork.BookingTourGuideRepository.InsertAsync(bookingRequest);
                    await _unitOfWork.SaveAsync();

                    // Tạo thông báo cho khách hàng
                    var notificationToCustomer = new Notification
                    {
                        UserId = bookingRequest.CustomerId,
                        UserType = "Customer",
                        Title = "Yêu cầu đặt Tour Guide mới",
                        Message = "Yêu cầu đặt Tour Guide của bạn đã được tạo thành công. Chờ bên B xác nhận",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    await _unitOfWork.NotificationRepository.InsertAsync(notificationToCustomer);

                    // Tạo thông báo cho TourGuide
                    var notificationToTourGuide = new Notification
                    {
                        UserId = bookingRequest.TourGuideId,
                        UserType = "TourGuide",
                        Title = "Bạn có một yêu cầu đặt Tour Guide mới",
                        Message = "Bạn đã được đặt để hướng dẫn một tour mới. Vui lòng kiểm tra và xác nhận yêu cầu.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    await _unitOfWork.NotificationRepository.InsertAsync(notificationToTourGuide);

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                    return bookingRequest;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<GetBookingTourGuideRequestView>> GetAllBookingTourGuideRequestAsync()
        {
            try
            {
                var BookingTourGuideList = await _unitOfWork.BookingTourGuideRepository.GetAsync();
                return _mapper.Map<IEnumerable<GetBookingTourGuideRequestView>>(BookingTourGuideList);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while get booking request.", ex);
            }
        }

        public async Task<GetBookingTourGuideRequestView> GetBookingTourGuideRequestByIdAsync(int id)
        {
            try
            {
                var tourGuide = await _unitOfWork.BookingTourGuideRepository.GetByIDAsync(id);
                if (tourGuide == null)
                {
                    return null;
                }
                return _mapper.Map<GetBookingTourGuideRequestView>(tourGuide);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while get booking request.", ex);
            }
        }



        public async Task<IEnumerable<GetBookingTourGuideRequestView>> GetBookingTourGuideRequestByCustomerId(int customerId)
        {
            try
            {
                var requests = await _unitOfWork.BookingTourGuideRepository.GetAsync(
                    r => r.CustomerId == customerId);
                return _mapper.Map<IEnumerable<GetBookingTourGuideRequestView>>(requests);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting booking tour guide requests by customer ID.", ex);
            }
        }

        public async Task<IEnumerable<GetBookingTourGuideRequestView>> GetBookingTourGuideRequestByTourGuideId(int tourGuideId)
        {
            try
            {
                var requests = await _unitOfWork.BookingTourGuideRepository.GetAsync(
                    r => r.TourGuideId == tourGuideId);
                return _mapper.Map<IEnumerable<GetBookingTourGuideRequestView>>(requests);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting booking tour guide requests by tour guide ID.", ex);
            }
        }
    }
}
