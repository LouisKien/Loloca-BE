    using AutoMapper;
using Loloca_BE.Business.Models.CustomerView;
using Loloca_BE.Business.Models.TourGuideView;
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
        }
    }
    }
