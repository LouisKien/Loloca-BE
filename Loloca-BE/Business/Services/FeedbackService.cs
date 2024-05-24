using AutoMapper;
using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Loloca_BE.Business.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMemoryCache _cache;

        public FeedbackService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper, IGoogleDriveService googleDriveService, IMemoryCache cache)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _googleDriveService = googleDriveService;
            _cache = cache;
        }


        public Task<IEnumerable<FeedbackModelView>> GetAllFeedbacksAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FeedbackModelView?> GetFeedbackByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UploadFeedbackAsync(FeedbackModelView feedbackModel, List<IFormFile> images)
        {
            string parentFolderId = "1Pp_3K7a1lZZpoZ2GX9nJGtZOAzFiqHem";

            try
            {
                // Create Feedback entity
                var feedback = _mapper.Map<Feedback>(feedbackModel);
                feedback.TimeFeedback = DateTime.Now;

                // Save Feedback to database
                await _unitOfWork.FeedbackRepository.InsertAsync(feedback);
                await _unitOfWork.SaveAsync();

                var feedbackId = feedback.FeedbackId;
                var feedbackImages = new List<FeedbackImage>();

                // Upload each image to Google Drive
                foreach (var image in images)
                {
                    if (!image.ContentType.StartsWith("image/"))
                    {
                        throw new InvalidDataException("Only image files are allowed.");
                    }

                    string fileName = $"Feedback_{feedbackId}_{Guid.NewGuid()}";

                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = fileName,
                        Parents = new List<string>() { parentFolderId },
                        MimeType = image.ContentType
                    };

                    using (var stream = image.OpenReadStream())
                    {
                        await _googleDriveService.UploadFileAsync(stream, fileMetadata);
                    }

                    // Create FeedbackImage entity
                    var feedbackImage = new FeedbackImage
                    {
                        FeedbackId = feedbackId,
                        ImagePath = fileName,
                        UploadDate = DateTime.Now
                    };

                    feedbackImages.Add(feedbackImage);
                }

                // Save FeedbackImages to database
                if (feedbackImages.Count > 0)
                {
                    await _unitOfWork.FeedbackImageRepository.AddRangeAsync(feedbackImages);
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot upload feedback", ex);
            }
        }
    }
}
