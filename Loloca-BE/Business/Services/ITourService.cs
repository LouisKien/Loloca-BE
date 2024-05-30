﻿using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Models.TourView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services
{
    public interface ITourService
    {
        Task UploadTourImageAsync(TourModelView tourModel, List<IFormFile> images);
        Task UpdateTourAsync(int tourId, TourInfoView tourModel);

        Task UpdateTourStatusAsync(int tourId, TourStatusView tourModel);

        Task DeleteTourAsync(int tourId);
        Task<List<AllToursView>> GetRandomToursAsync(string sessionId, int page, int pageSize);
        Task<List<AllToursView>> GetRandomToursInCityAsync(string sessionId, int CityId, int page, int pageSize);
        Task<List<AllToursView>> GetRandomToursByTourGuideAsync(string sessionId, int TourGuideId, int page, int pageSize);

        Task<GetTourByIdView?> GetTourByIdAsync(int tourId);

        Task<int> GetTotalPage(int pageSize, int? cityId, string sessionId);
        Task<int> GetTotalPageTourGuide(int pageSize, int? tourId, string sessionId);
        Task<List<GetTourByStatusView>> GetToursByStatusAsync(int status);
    }
}
