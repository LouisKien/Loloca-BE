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
                    // Kiểm tra rằng StartDate và EndDate không bằng nhau
                    if (model.StartDate == model.EndDate)
                    {
                        throw new Exception("Ngày bắt đầu và ngày kết thúc không được trùng nhau.");
                    }

                    // Lấy tất cả các yêu cầu đặt tour đã tồn tại cho tour cụ thể này
                    var existingBookings = await _unitOfWork.BookingTourRequestRepository.GetAllAsync(
                        b => b.TourId == model.TourId && b.Status <= 1
                    );

                    var existingBookings1 = await _unitOfWork.BookingTourRequestRepository.GetAllAsync(
                     b => b.CustomerId == model.CustomerId && b.Status <= 1
                    );
                    var existingBookings2 = await _unitOfWork.BookingTourGuideRepository.GetAllAsync(
                        b => b.CustomerId == model.CustomerId && b.Status <= 1
                    );
                    var tour1 = (await _unitOfWork.TourRepository.GetAsync(filter: t => t.TourId == model.TourId, includeProperties: "TourGuide" )).FirstOrDefault();
                    var existingBookings3 = await _unitOfWork.BookingTourGuideRepository.GetAllAsync(
                        b => b.TourGuideId == tour1.TourGuide.TourGuideId && b.Status <= 1
                    );


                    // Kiểm tra xem ngày bắt đầu và ngày kết thúc của yêu cầu mới có trùng lặp với bất kỳ yêu cầu nào đã tồn tại hay không
                    foreach (var booking in existingBookings)
                    {
                        if ((model.StartDate >= booking.StartDate && model.StartDate < booking.EndDate) ||
                            (model.EndDate > booking.StartDate && model.EndDate <= booking.EndDate) ||
                            (model.StartDate <= booking.StartDate && model.EndDate >= booking.EndDate))
                        {
                            throw new Exception($"Yêu cầu mới không được trùng lặp với bất kỳ yêu cầu nào đã tồn tại. Yêu cầu đã tồn tại: {booking.StartDate.ToString("dd/MM/yyyy")} - {booking.EndDate.ToString("dd/MM/yyyy")}");
                        }
                    }

                    // Kiểm tra xem ngày bắt đầu và ngày kết thúc của yêu cầu mới có trùng lặp với bất kỳ yêu cầu nào đã tồn tại hay không
                    foreach (var booking in existingBookings1)
                    {
                        if ((model.StartDate >= booking.StartDate && model.StartDate < booking.EndDate) ||
                            (model.EndDate > booking.StartDate && model.EndDate <= booking.EndDate) ||
                            (model.StartDate <= booking.StartDate && model.EndDate >= booking.EndDate))
                        {
                            throw new Exception($"Yêu cầu mới không được trùng lặp với bất kỳ yêu cầu nào đã tồn tại. Yêu cầu đã tồn tại: {booking.StartDate.ToString("dd/MM/yyyy")} - {booking.EndDate.ToString("dd/MM/yyyy")}");
                        }
                    }

                    foreach (var booking in existingBookings2)
                    {
                        if ((model.StartDate >= booking.StartDate && model.StartDate < booking.EndDate) ||
                            (model.EndDate > booking.StartDate && model.EndDate <= booking.EndDate) ||
                            (model.StartDate <= booking.StartDate && model.EndDate >= booking.EndDate))
                        {
                            throw new Exception($"Yêu cầu mới không được trùng lặp với bất kỳ yêu cầu nào đã tồn tại. Yêu cầu đã tồn tại: {booking.StartDate.ToString("dd/MM/yyyy")} - {booking.EndDate.ToString("dd/MM/yyyy")}");
                        }
                    }

                    foreach (var booking in existingBookings3)
                    {
                        if ((model.StartDate >= booking.StartDate && model.StartDate < booking.EndDate) ||
                            (model.EndDate > booking.StartDate && model.EndDate <= booking.EndDate) ||
                            (model.StartDate <= booking.StartDate && model.EndDate >= booking.EndDate))
                        {
                            throw new Exception($"Yêu cầu mới không được trùng lặp với bất kỳ yêu cầu nào đã tồn tại. Yêu cầu đã tồn tại: {booking.StartDate.ToString("dd/MM/yyyy")} - {booking.EndDate.ToString("dd/MM/yyyy")}");
                        }
                    }

                    int numOfTourists = model.NumOfAdult + model.NumOfChild;

                    var rangePrice = (await _unitOfWork.TourPriceRepository.GetAsync(filter: t => t.TourId == model.TourId && (t.TotalTouristFrom <= numOfTourists && t.TotalTouristTo >= numOfTourists))).FirstOrDefault();
                    if(rangePrice == null)
                    {
                        throw new Exception("Range price doesnot exist");
                    }

                    var date = DateTime.Now;
                    var bookingRequest = new BookingTourRequest
                    {
                        CustomerId = model.CustomerId,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        RequestDate = date,
                        RequestTimeOut = date.AddDays(7),
                        Note = model.Note,
                        NumOfAdult = model.NumOfChild,
                        NumOfChild = model.NumOfChild,
                        Status = 0,
                        TourId = model.TourId,
                        TotalPrice = rangePrice.ChildPrice * model.NumOfChild + rangePrice.AdultPrice * model.NumOfChild
                    };

                    await _unitOfWork.BookingTourRequestRepository.InsertAsync(bookingRequest);
                    await _unitOfWork.SaveAsync();

                    // Lấy thông tin Tour cùng với TourGuide
                    var tours = await _unitOfWork.TourRepository.GetAllAsync(
                        t => t.TourId == model.TourId,
                        null,
                        "TourGuide"
                    );

                    var tour = tours.FirstOrDefault();
                    if (tour == null)
                    {
                        throw new Exception("Không tìm thấy thông tin Tour.");
                    }

                    // Tạo thông báo cho khách hàng
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

                    // Tạo thông báo cho Hướng dẫn viên
                    var notificationToTourGuide = new Notification
                    {
                        UserId = tour.TourGuideId,  // Sử dụng tour.TourGuideId
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
