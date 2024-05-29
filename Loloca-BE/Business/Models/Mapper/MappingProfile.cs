    using AutoMapper;
using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Business.Models.CustomerView;
using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Models.OrderView;
using Loloca_BE.Business.Models.TourGuideView;
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

            CreateMap<OrderModelView, Order>().ReverseMap();
            CreateMap<OrderForBookingTourGuideView, Order>().ReverseMap();
            CreateMap<OrderForBookingTourView, Order>().ReverseMap();

            CreateMap<FeedbackModelView, Feedback>().ReverseMap();

            CreateMap<TourModelView, Tour>().ReverseMap();
            CreateMap<TourInfoView, Tour>().ReverseMap();
            CreateMap<TourImageView, TourImage>().ReverseMap();

            //CreateMap<FeedbackImageView, Feedback>().ReverseMap();
            //CreateMap<FeebackView, Feedback>().ReverseMap();
            //CreateMap<GetFeedBackForCustomerView, Feedback>().ReverseMap();
            //CreateMap<GetFeedbackForTourGuideView, Feedback>().ReverseMap();



        }
    }
    }
