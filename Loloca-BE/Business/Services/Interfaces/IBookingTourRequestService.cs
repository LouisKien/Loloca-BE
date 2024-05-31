﻿using Loloca_BE.Business.Models.BookingTourGuideRequestModelView;
using Loloca_BE.Business.Models.BookingTourRequestModelView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface IBookingTourRequestService
    {
        Task<BookingTourRequest> AddBookingTourRequestAsync(BookingTourRequestView model);
        Task<IEnumerable<GetBookingTourRequestView>> GetAllBookingTourRequestAsync();
        Task<GetBookingTourRequestView> GetBookingTourRequestByIdAsync(int id);
    }
}
