using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Drawing.Drawing2D;

namespace Loloca_BE.Data.Repositories.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private LolocaDbContext _dbContext;

        private GenericRepository<Notification> _notificationRepository;
        private GenericRepository<Account> _accountRepository;
        private GenericRepository<BookingTourGuideRequest> _bookingTourGuideRequestRepository;
        private GenericRepository<BookingTourRequest> _bookingTourRequestRepository;
        private GenericRepository<City> _cityRepository;
        private GenericRepository<Customer> _customerRepository;
        private GenericRepository<Feedback> _feedbackRepository;
        private GenericRepository<FeedbackImage> _feedbackImageRepository;
        private GenericRepository<Order> _orderRepository;
        private GenericRepository<PaymentRequest> _paymentRequestRepository;
        private GenericRepository<RefreshToken> _refreshTokenRepository;
        private GenericRepository<Tour> _tourRepository;
        private GenericRepository<TourGuide> _tourGuideRepository;
        private GenericRepository<TourImage> _tourImageRepository;
        private GenericRepository<TourExclude> _tourExcludeRepository;
        private GenericRepository<TourInclude> _tourIncludeRepository;
        private GenericRepository<TourHighlight> _tourHighlightRepository;
        private GenericRepository<TourItinerary> _tourItineraryRepository;
        private GenericRepository<TourType> _tourTypeRepository;
        private GenericRepository<TourPrice> _tourPriceRepository;

        public UnitOfWork(LolocaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _dbContext.Database.BeginTransaction();
        }
        public IGenericRepository<Notification> NotificationRepository => _notificationRepository ??= new GenericRepository<Notification>(_dbContext);

        public IGenericRepository<Account> AccountRepository => _accountRepository ??= new GenericRepository<Account>(_dbContext);

        public IGenericRepository<BookingTourGuideRequest> BookingTourGuideRepository => _bookingTourGuideRequestRepository ??= new GenericRepository<BookingTourGuideRequest>(_dbContext);

        public IGenericRepository<BookingTourRequest> BookingTourRequestRepository => _bookingTourRequestRepository ??= new GenericRepository<BookingTourRequest>(_dbContext);

        public IGenericRepository<City> CityRepository => _cityRepository ??= new GenericRepository<City>(_dbContext);

        public IGenericRepository<Customer> CustomerRepository => _customerRepository ??= new GenericRepository<Customer>(_dbContext);

        public IGenericRepository<Feedback> FeedbackRepository => _feedbackRepository ??= new GenericRepository<Feedback>(_dbContext);

        public IGenericRepository<FeedbackImage> FeedbackImageRepository => _feedbackImageRepository ??= new GenericRepository<FeedbackImage>(_dbContext);

        public IGenericRepository<Order> OrderRepository => _orderRepository ??= new GenericRepository<Order>(_dbContext);

        public IGenericRepository<PaymentRequest> PaymentRequestRepository => _paymentRequestRepository ??= new GenericRepository<PaymentRequest>(_dbContext);

        public IGenericRepository<RefreshToken> RefreshTokenRepository => _refreshTokenRepository ??= new GenericRepository<RefreshToken>(_dbContext);

        public IGenericRepository<Tour> TourRepository => _tourRepository ??= new GenericRepository<Tour>(_dbContext);

        public IGenericRepository<TourGuide> TourGuideRepository => _tourGuideRepository ??= new GenericRepository<TourGuide>(_dbContext);

        public IGenericRepository<TourImage> TourImageRepository => _tourImageRepository ??= new GenericRepository<TourImage>(_dbContext);

        public IGenericRepository<TourExclude> TourExcludeRepository => _tourExcludeRepository ??= new GenericRepository<TourExclude>(_dbContext);
        
        public IGenericRepository<TourHighlight> TourHighlightRepository => _tourHighlightRepository ??= new GenericRepository<TourHighlight>(_dbContext);

        public IGenericRepository<TourInclude> TourIncludeRepository => _tourIncludeRepository ??= new GenericRepository<TourInclude>(_dbContext);

        public IGenericRepository<TourItinerary> TourItineraryRepository => _tourItineraryRepository ??= new GenericRepository<TourItinerary>(_dbContext);

        public IGenericRepository<TourType> TourTypeRepository => _tourTypeRepository ??= new GenericRepository<TourType>(_dbContext);

        public IGenericRepository<TourPrice> TourPriceRepository => _tourPriceRepository ??= new GenericRepository<TourPrice>(_dbContext);
    }
}
