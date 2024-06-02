using Hangfire;
using Loloca_BE.Business.BackgroundServices.Interfaces;
using Loloca_BE.Business.Models.TourView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Loloca_BE.Business.BackgroundServices.Implements
{
    public class TourBackgroundService : ITourBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMemoryCache _cache;
        private static readonly Random _random = new Random();

        public TourBackgroundService(IUnitOfWork unitOfWork, IGoogleDriveService googleDriveService, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _googleDriveService = googleDriveService;
            _cache = cache;
        }

        public async Task RefreshTourCache()
        {
            try
            {
                var latestTours = await _unitOfWork.TourRepository.GetAllAsync(filter: t => t.Status == 1, includeProperties: "TourGuide,City");
                var items = new List<AllToursView>();
                foreach (var tour in latestTours)
                {
                    var tourImage = (await _unitOfWork.TourImageRepository.FindAsync(t => t.TourId == tour.TourId)).FirstOrDefault();
                    var item = new AllToursView
                    {
                        CityName = tour.City.Name,
                        Description = tour.Description,
                        Duration = tour.Duration,
                        Name = tour.Name,
                        ThumbnailTourImage = tourImage == null ? null : await _googleDriveService.GetImageFromCacheOrDriveAsync(tourImage.ImagePath, "1j6R0VaaZXFbruE553kdGyUrboAxfVw3o"),
                        CityId = tour.CityId,
                        TourGuideId = tour.TourGuideId,
                        TourId = tour.TourId,
                        TourGuideName = $"{tour.TourGuide.LastName} {tour.TourGuide.FirstName}"
                    };
                    items.Add(item);
                }

                var activeSessionIds = _cache.Get<List<string>>("ActiveSessions") ?? new List<string>();

                foreach (var sessionId in activeSessionIds)
                {
                    var cacheKey = $"Tour_{sessionId}";
                    var shuffledItems = items.OrderBy(x => _random.Next()).ToList();
                    _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(5));
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task RefreshTourInCityCache()
        {
            var latestTours = await _unitOfWork.TourRepository.GetAllAsync(filter: t => t.Status == 1, includeProperties: "TourGuide,City");
            var toursByCity = latestTours.GroupBy(t => t.CityId).ToDictionary(g => g.Key, g => g.ToList());

            var activeSessionIds = _cache.Get<List<string>>("ActiveSessions") ?? new List<string>();
            foreach (var sessionId in activeSessionIds)
            {
                foreach (var cityGroup in toursByCity)
                {
                    var cityId = cityGroup.Key;
                    var toursInCity = cityGroup.Value;

                    var cacheKey = $"Tour_CityId:{cityId}_{sessionId}";
                    var shuffledItems = await GenerateShuffledTourList(toursInCity);
                    _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(5));
                }
            }
        }

        private async Task<List<AllToursView>> GenerateShuffledTourList(List<Tour> tours)
        {
            var items = new List<AllToursView>();

            foreach (var tour in tours)
            {
                var tourImage = (await _unitOfWork.TourImageRepository.FindAsync(t => t.TourId == tour.TourId)).FirstOrDefault();

                var item = new AllToursView
                {
                    CityName = tour.City.Name,
                    Description = tour.Description,
                    Duration = tour.Duration,
                    Name = tour.Name,
                    ThumbnailTourImage = tourImage == null ? null : await _googleDriveService.GetImageFromCacheOrDriveAsync(tourImage.ImagePath, "1j6R0VaaZXFbruE553kdGyUrboAxfVw3o"),
                    CityId = tour.CityId,
                    TourGuideId = tour.TourGuideId,
                    TourId = tour.TourId,
                    TourGuideName = $"{tour.TourGuide.LastName} {tour.TourGuide.FirstName}"
                };

                items.Add(item);
            }

            return items.OrderBy(x => _random.Next()).ToList();
        }
    }
}
