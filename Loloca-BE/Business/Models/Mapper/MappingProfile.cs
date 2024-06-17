    using AutoMapper;
using Loloca_BE.Business.Models.BookingTourGuideRequestModelView;
using Loloca_BE.Business.Models.BookingTourRequestModelView;
using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Business.Models.CustomerView;
using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Models.OrderView;
using Loloca_BE.Business.Models.TourExcludeView;
using Loloca_BE.Business.Models.TourGuideView;
using Loloca_BE.Business.Models.TourHighlightView;
using Loloca_BE.Business.Models.TourIncludeView;
using Loloca_BE.Business.Models.TourItineraryView;
using Loloca_BE.Business.Models.TourTypeView;
using Loloca_BE.Business.Models.TourView;
using Loloca_BE.Data.Entities;
using System.Diagnostics.Contracts;
namespace Loloca_BE.Business.Models.Mapper
{
    public class MappingProfile : Profile
        {
            public MappingProfile()
            {
            CreateMap<UpdateProfile, Customer>();
            CreateMap<ChangePassword, Customer>();

            CreateMap<UpdateProfileTourGuide, TourGuide>();
            CreateMap<ChangePasswordTourGuide, TourGuide>();

            CreateMap<CityView, City>().ReverseMap();
            CreateMap<UpdateCityView, City>().ReverseMap();
            CreateMap<CreateCity, City>().ReverseMap();

            CreateMap<OrderModelView, Order>().ReverseMap();
            CreateMap<OrderForBookingTourGuideView, Order>().ReverseMap();
            CreateMap<OrderForBookingTourView, Order>().ReverseMap();

            CreateMap<FeedbackModelView, Feedback>().ReverseMap();

            CreateMap<TourModelView, Tour>().ReverseMap();
            CreateMap<TourInfoView, Tour>().ReverseMap();
            CreateMap<TourImageView, TourImage>().ReverseMap();
            CreateMap<TourStatusView, Tour>().ReverseMap();
            CreateMap<BookingTourGuideRequestView, BookingTourGuideRequest>().ReverseMap();
            CreateMap<GetBookingTourGuideRequestView, BookingTourGuideRequest>().ReverseMap();
            CreateMap<BookingTourRequestView, BookingTourRequest>().ReverseMap();
            CreateMap<GetBookingTourRequestView, BookingTourRequest>().ReverseMap();


            //CreateMap<FeedbackImageView, Feedback>().ReverseMap();
            //CreateMap<FeebackView, Feedback>().ReverseMap();
            //CreateMap<GetFeedBackForCustomerView, Feedback>().ReverseMap();
            //CreateMap<GetFeedbackForTourGuideView, Feedback>().ReverseMap();



        }
    }
    }
