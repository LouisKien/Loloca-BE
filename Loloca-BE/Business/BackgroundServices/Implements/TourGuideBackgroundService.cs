using Hangfire;
using Loloca_BE.Business.BackgroundServices.Interfaces;
using Loloca_BE.Business.Models.TourGuideView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Loloca_BE.Business.BackgroundServices.Implements
{
    public class TourGuideBackgroundService : ITourGuideBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMemoryCache _cache;
        private static readonly Random _random = new Random();

        public TourGuideBackgroundService(IUnitOfWork unitOfWork, IGoogleDriveService googleDriveService, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _googleDriveService = googleDriveService;
        }

        public async Task RefreshTourGuideCache()
        {
            try
            {
                var latestTourGuides = await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1);
                var items = new List<GetTourGuide>();
                foreach (var tourGuide in latestTourGuides)
                {
                    var item = new GetTourGuide
                    {
                        Avatar = string.IsNullOrEmpty(tourGuide.AvatarPath) ? null : await _googleDriveService.GetImageFromCacheOrDriveAsync(tourGuide.AvatarPath, "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn"),
                        AvatarUploadedTime = string.IsNullOrEmpty(tourGuide.AvatarPath) ? null : tourGuide.AvatarUploadDate,
                        DateOfBirth = tourGuide.DateOfBirth,
                        Description = tourGuide.Description,
                        FirstName = tourGuide.FirstName,
                        Gender = tourGuide.Gender,
                        Id = tourGuide.TourGuideId,
                        LastName = tourGuide.LastName,
                        PricePerDay = tourGuide.PricePerDay
                    };
                    items.Add(item);
                }

                var activeSessionIds = _cache.Get<List<string>>("ActiveSessions") ?? new List<string>();

                foreach (var sessionId in activeSessionIds)
                {
                    var cacheKey = $"TourGuide_{sessionId}";
                    var shuffledItems = items.OrderBy(x => _random.Next()).ToList();
                    _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(5));
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task RefreshTourGuideInCityCache()
        {
            try
            {
                var latestTourGuides = await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.Status == 1);
                var tourGuidesByCity = latestTourGuides.GroupBy(t => t.CityId).ToDictionary(g => g.Key, g => g.ToList());

                var activeSessionIds = _cache.Get<List<string>>("ActiveSessions") ?? new List<string>();
                foreach (var sessionId in activeSessionIds)
                {
                    foreach (var cityGroup in tourGuidesByCity)
                    {
                        var cityId = cityGroup.Key;
                        var tourGuidesInCity = cityGroup.Value;

                        var cacheKey = $"TourGuide_CityId:{cityId}_{sessionId}";
                        var shuffledItems = await GenerateShuffledTourList(tourGuidesInCity);
                        _cache.Set(cacheKey, shuffledItems, TimeSpan.FromMinutes(5));
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task<List<GetTourGuide>> GenerateShuffledTourList(List<TourGuide> tourGuides)
        {
            var items = new List<GetTourGuide>();

            foreach (var tourGuide in tourGuides)
            {

                var item = new GetTourGuide
                {
                    Avatar = string.IsNullOrEmpty(tourGuide.AvatarPath) ? null : await _googleDriveService.GetImageFromCacheOrDriveAsync(tourGuide.AvatarPath, "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn"),
                    AvatarUploadedTime = string.IsNullOrEmpty(tourGuide.AvatarPath) ? null : tourGuide.AvatarUploadDate,
                    DateOfBirth = tourGuide.DateOfBirth,
                    Description = tourGuide.Description,
                    FirstName = tourGuide.FirstName,
                    Gender = tourGuide.Gender,
                    Id = tourGuide.TourGuideId,
                    LastName = tourGuide.LastName,
                    PricePerDay = tourGuide.PricePerDay
                };
                items.Add(item);
            }

            return items.OrderBy(x => _random.Next()).ToList();
        }
    }
}
