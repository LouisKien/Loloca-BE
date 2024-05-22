
using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Loloca_BE.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Loloca_BE.Business.Services
{
    public class TourGuideService : ITourGuideService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IGoogleDriveService _googleDriveService;

        public TourGuideService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper, IGoogleDriveService googleDriveService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _googleDriveService = googleDriveService;
        }

        public async Task UploadAvatarAsync(IFormFile file, int TourGuideId)
        {
            string parentFolderId = "1Jej2xcGybrPJDV4f6CiEkgaQN2fN8Nvn";

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new InvalidDataException("Only image files are allowed.");
            }

            var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(TourGuideId);
            if (tourGuide == null)
            {
                throw new Exception($"TourGuide with TourGuideId {TourGuideId} doesn't exist");
            }

            try
            {
                // If an avatar exists, delete the old one
                if (!tourGuide.AvatarPath.IsNullOrEmpty() && tourGuide.AvatarUploadDate.HasValue)
                {
                    await _googleDriveService.DeleteFileAsync(tourGuide.AvatarPath, parentFolderId);
                }

                string fileName = $"Avatar_TourGuide_{TourGuideId}";

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    Parents = new List<string>() { parentFolderId },
                    MimeType = file.ContentType
                };

                // Upload Directly to Google Drive
                using (var stream = file.OpenReadStream())
                {
                    await _googleDriveService.UploadFileAsync(stream, fileMetadata);
                }

                tourGuide.AvatarPath = fileName;
                tourGuide.AvatarUploadDate = DateTime.Now;

                await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                await _unitOfWork.SaveAsync();
            } catch (Exception ex)
            {
                throw new Exception("Cannot update avatar");
            }
        }

        public async Task UploadCoverAsync(IFormFile file, int TourGuideId)
        {
            string parentFolderId = "1s642kdPTeuccQ0bcXXPXkEdAVCWDItmH";

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new InvalidDataException("Only image files are allowed.");
            }

            var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(TourGuideId);
            if (tourGuide == null)
            {
                throw new Exception($"TourGuide with TourGuideId {TourGuideId} doesn't exist");
            }

            try
            {
                // If an avatar exists, delete the old one
                if (!tourGuide.CoverPath.IsNullOrEmpty() && tourGuide.CoverUploadDate.HasValue)
                {
                    await _googleDriveService.DeleteFileAsync(tourGuide.CoverPath, parentFolderId);
                }

                string fileName = $"Cover_TourGuide_{TourGuideId}";

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    Parents = new List<string>() { parentFolderId },
                    MimeType = file.ContentType
                };

                // Upload Directly to Google Drive
                using (var stream = file.OpenReadStream())
                {
                    await _googleDriveService.UploadFileAsync(stream, fileMetadata);
                }

                tourGuide.CoverPath = fileName;
                tourGuide.CoverUploadDate = DateTime.Now;

                await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                await _unitOfWork.SaveAsync();
            } catch (Exception ex)
            {
                throw new Exception("Cannot update cover");
            }
        }
    }
}
