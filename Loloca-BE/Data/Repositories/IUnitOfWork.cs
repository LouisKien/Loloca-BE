using Loloca_BE.Data.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Loloca_BE.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IDbContextTransaction BeginTransaction();
        Task SaveAsync();

        IGenericRepository<Account> AccountRepository { get; }

        IGenericRepository<BookingTourGuideRequest> BookingTourGuideRepository { get; }

        IGenericRepository<BookingTourRequest> BookingTourRequestRepository { get; }

        IGenericRepository<City> CityRepository { get; }

        IGenericRepository<Customer> CustomerRepository { get; }

        IGenericRepository<Feedback> FeedbackRepository { get; }

        IGenericRepository<FeedbackImage> FeedbackImageRepository { get; }

        IGenericRepository<Order> OrderRepository { get; }

        IGenericRepository<RefreshToken> RefreshTokenRepository { get; }

        IGenericRepository<Tour> TourRepository { get; }

        IGenericRepository<TourGuide> TourGuideRepository { get; }

        IGenericRepository<TourImage> TourImageRepository { get; }
    }
}
