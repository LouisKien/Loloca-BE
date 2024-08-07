﻿using Loloca_BE.Business.Models.BookingTourGuideRequestModelView;
using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface IBookingTourGuideRequestService
    {
        Task<BookingTourGuideRequest> AddBookingTourGuideRequestAsync(BookingTourGuideRequestView model);
        Task<IEnumerable<GetBookingTourGuideRequestView>> GetAllBookingTourGuideRequestAsync();
        Task<GetBookingTourGuideRequestView> GetBookingTourGuideRequestByIdAsync(int id);

        Task<IEnumerable<GetBookingTourGuideRequestView>> GetBookingTourGuideRequestByCustomerId(int customerId);
        Task<IEnumerable<GetBookingTourGuideRequestView>> GetBookingTourGuideRequestByTourGuideId(int tourGuideId);

        Task<Customer> GetCustomerByIdAsync(int customerId);
    }
}
