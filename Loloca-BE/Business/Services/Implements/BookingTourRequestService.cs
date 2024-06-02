using AutoMapper;
using Loloca_BE.Business.Models.BookingTourGuideRequestModelView;
using Loloca_BE.Business.Models.BookingTourRequestModelView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.Services.Implements
{
    public class BookingTourRequestService : IBookingTourRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingTourRequestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<BookingTourRequest> AddBookingTourRequestAsync(BookingTourRequestView model)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var bookingRequest = _mapper.Map<BookingTourRequest>(model);
                    bookingRequest.RequestDate = DateTime.Now;
                    bookingRequest.RequestTimeOut = bookingRequest.RequestDate.AddMinutes(20); // Add 20 minutes to RequestDate
                    bookingRequest.Status = 1;

                    await _unitOfWork.BookingTourRequestRepository.InsertAsync(bookingRequest);
                    await _unitOfWork.SaveAsync();

                    // Retrieve the Tour entity along with TourGuide using includeProperties
                    var tours = await _unitOfWork.TourRepository.GetAllAsync(
                        t => t.TourId == model.TourId,
                        null,
                        "TourGuide"
                    );

                    var tour = tours.FirstOrDefault();
                    if (tour == null)
                    {
                        throw new Exception("Tour not found");
                    }

                    // Create notification for the customer
                    var notificationToCustomer = new Notification
                    {
                        UserId = bookingRequest.CustomerId,
                        UserType = "Customer",
                        Title = "Yêu cầu đặt Tour mới",
                        Message = "Yêu cầu đặt Tour của bạn đã được tạo thành công. Chờ bên B xác nhận",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    await _unitOfWork.NotificationRepository.InsertAsync(notificationToCustomer);

                    // Create notification for the TourGuide
                    var notificationToTourGuide = new Notification
                    {
                        UserId = tour.TourGuideId,  // Use tour.TourGuideId
                        UserType = "TourGuide",
                        Title = "Bạn có một yêu cầu đặt Tour mới",
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

        public async Task<IEnumerable<GetBookingTourRequestView>> GetAllBookingTourRequestAsync()
        {
            try
            {
                var BookingTourGuideList = await _unitOfWork.BookingTourRequestRepository.GetAsync();
                return _mapper.Map<IEnumerable<GetBookingTourRequestView>>(BookingTourGuideList);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while get booking request.", ex);
            }
        }

        public async Task<GetBookingTourRequestView> GetBookingTourRequestByIdAsync(int id)
        {
            try
            {
                var tourGuide = await _unitOfWork.BookingTourRequestRepository.GetByIDAsync(id);
                if (tourGuide == null)
                {
                    return null;
                }
                return _mapper.Map<GetBookingTourRequestView>(tourGuide);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while get booking request.", ex);
            }
        }

        public async Task<IEnumerable<GetBookingTourRequestView>> GetBookingTourRequestByCustomerId(int customerId)
        {
            try
            {
                var requests = await _unitOfWork.BookingTourRequestRepository.GetAsync(
                    r => r.CustomerId == customerId);
                return _mapper.Map<IEnumerable<GetBookingTourRequestView>>(requests);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting booking tour requests by customer ID.", ex);
            }
        }

        public async Task<IEnumerable<GetBookingTourRequestView>> GetBookingTourRequestByTourGuideId(int tourGuideId)
        {
            try
            {
                var requests = await _unitOfWork.BookingTourRequestRepository.GetAsync(
                    r => r.Tour.TourGuideId == tourGuideId);
                return _mapper.Map<IEnumerable<GetBookingTourRequestView>>(requests);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting booking tour requests by tour guide ID.", ex);
            }
        }
    }
}
