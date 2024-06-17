using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Implements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Loloca_BE.Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDbContextTransaction BeginTransaction();
        Task SaveAsync();
        IGenericRepository<Notification> NotificationRepository { get; }
        IGenericRepository<Account> AccountRepository { get; }

        IGenericRepository<BookingTourGuideRequest> BookingTourGuideRepository { get; }

        IGenericRepository<BookingTourRequest> BookingTourRequestRepository { get; }

        IGenericRepository<City> CityRepository { get; }

        IGenericRepository<Customer> CustomerRepository { get; }

        IGenericRepository<Feedback> FeedbackRepository { get; }

        IGenericRepository<FeedbackImage> FeedbackImageRepository { get; }

        IGenericRepository<Order> OrderRepository { get; }

        IGenericRepository<PaymentRequest> PaymentRequestRepository { get; }

        IGenericRepository<RefreshToken> RefreshTokenRepository { get; }

        IGenericRepository<Tour> TourRepository { get; }

        IGenericRepository<TourGuide> TourGuideRepository { get; }
        IGenericRepository<TourImage> TourImageRepository { get; }

        IGenericRepository<TourExclude> TourExcludeRepository { get; }

        IGenericRepository<TourHighlight> TourHighlightRepository { get; }

        IGenericRepository<TourInclude> TourIncludeRepository { get; }

        IGenericRepository<TourItinerary> TourItineraryRepository { get; }

        IGenericRepository<TourType> TourTypeRepository { get; }

        IGenericRepository<TourPrice> TourPriceRepository { get; }
    }
}
