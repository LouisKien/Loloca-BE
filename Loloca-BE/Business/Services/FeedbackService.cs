﻿using AutoMapper;
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

        private async Task<byte[]> GetImageFromCacheOrDriveAsync(string imagePath, string parentFolderId)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return null;
            }

            string cacheKey = $"{imagePath}";
            if (!_cache.TryGetValue(cacheKey, out byte[] imageContent))
            {
                imageContent = await _googleDriveService.GetFileContentAsync(imagePath, parentFolderId);

                if (imageContent != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                    };
                    _cache.Set(cacheKey, imageContent, cacheEntryOptions);
                }
            }

            return imageContent;
        }

        public async Task<IEnumerable<FeebackView>> GetAllFeedbacksAsync()
        {
            try
            {
                var feedbacks = await _unitOfWork.FeedbackRepository.GetAsync(includeProperties: "FeedbackImages");
                var feedbackViews = new List<FeebackView>();

                foreach (var feedback in feedbacks)
                {
                    var feedbackView = new FeebackView
                    {
                        FeedbackId = feedback.FeedbackId,
                        CustomerId = feedback.CustomerId,
                        TourGuideId = feedback.TourGuideId,
                        NumOfStars = feedback.NumOfStars,
                        Content = feedback.Content,
                        Status = feedback.Status,
                        TimeFeedback = feedback.TimeFeedback,

                    };

                    feedbackView.feedBackImgViewList = new List<FeedbackImageView>();

                    if (feedback.FeedbackImages != null)
                    {
                        foreach (var image in feedback.FeedbackImages)
                        {
                            var imagePath = await GetImageFromCacheOrDriveAsync(image.ImagePath, "1Pp_3K7a1lZZpoZ2GX9nJGtZOAzFiqHem");
                            var feedbackImageView = new FeedbackImageView
                            {
                                ImagePath = imagePath,
                                UploadDate = image.UploadDate
                            };
                            feedbackView.feedBackImgViewList.Add(feedbackImageView);
                        }
                    }

                    feedbackViews.Add(feedbackView);
                }

                return feedbackViews;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<GetFeedBackForCustomerView>> GetFeedbackByCustomerIdAsync(int customerId)
        {
            try
            {
                var feedbacks = await _unitOfWork.FeedbackRepository.GetAsync(
                    filter: f => f.CustomerId == customerId && f.Status == true, // Lọc các phản hồi theo customerId
                    includeProperties: "FeedbackImages"
                );

                if (feedbacks == null || !feedbacks.Any())
                {
                    return null; // Trả về null nếu không có phản hồi nào cho customerId đã cho
                }

                var feedbackViews = new List<GetFeedBackForCustomerView>();

                foreach (var feedback in feedbacks)
                {
                    await _unitOfWork.FeedbackRepository.LoadCollectionAsync(feedback, f => f.FeedbackImages);

                    var feedbackView = new GetFeedBackForCustomerView
                    {
                        FeedbackId = feedback.FeedbackId,
                        CustomerId = feedback.CustomerId,
                        TourGuideId = feedback.TourGuideId,
                        NumOfStars = feedback.NumOfStars,
                        Content = feedback.Content,
                        Status = feedback.Status ,
                        TimeFeedback = feedback.TimeFeedback,
                        feedBackImgViewListForCus = await GetFeedbackImageViewsAsync(feedback.FeedbackImages.ToList())
                    };

                    feedbackViews.Add(feedbackView);
                }


                return feedbackViews;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và ném lại
                throw new Exception($"Error occurred while fetching feedbacks for tour guide with ID {customerId}: {ex.Message}");
            }
        }

        public async Task<FeebackView> GetFeedbackByIdAsync(int feedbackId)
        {
            var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackId);

            if (feedback == null)
            {
                // Xử lý trường hợp không tìm thấy feedback
                return null;
            }

            // Load FeedbackImages explicitly
            await _unitOfWork.FeedbackRepository.LoadCollectionAsync(feedback, f => f.FeedbackImages);

            var feedbackView = new FeebackView
            {
                FeedbackId = feedback.FeedbackId,
                CustomerId = feedback.CustomerId,
                TourGuideId = feedback.TourGuideId,
                NumOfStars = feedback.NumOfStars,
                Content = feedback.Content,
                Status = feedback.Status,
                TimeFeedback = feedback.TimeFeedback,
            };

            feedbackView.feedBackImgViewList = new List<FeedbackImageView>();

            foreach (var image in feedback.FeedbackImages)
            {
                var imageView = new FeedbackImageView
                {
                    ImagePath = await GetImageFromCacheOrDriveAsync(image.ImagePath, "1Pp_3K7a1lZZpoZ2GX9nJGtZOAzFiqHem"),
                    UploadDate = image.UploadDate
                };

                feedbackView.feedBackImgViewList.Add(imageView);
            }

            return feedbackView;
        }
        public async Task<IEnumerable<GetFeedbackForTourGuideView>> GetFeedbackByTourGuideIdAsync(int tourGuideId)
        {
            try
            {
                var feedbacks = await _unitOfWork.FeedbackRepository.GetAsync(
                    filter: f => f.TourGuideId == tourGuideId && f.Status == true, // Lọc các phản hồi theo TourGuideId
                    includeProperties: "FeedbackImages"
                );

                if (feedbacks == null || !feedbacks.Any())
                {
                    return null; // Trả về null nếu không có phản hồi nào cho TourGuideId đã cho
                }

                var feedbackViews = new List<GetFeedbackForTourGuideView>();

                foreach (var feedback in feedbacks)
                {
                    await _unitOfWork.FeedbackRepository.LoadCollectionAsync(feedback, f => f.FeedbackImages);

                    var feedbackView = new GetFeedbackForTourGuideView
                    {
                        FeedbackId = feedback.FeedbackId,
                        CustomerId = feedback.CustomerId,
                        TourGuideId = feedback.TourGuideId,
                        NumOfStars = feedback.NumOfStars,
                        Content = feedback.Content,
                        Status = feedback.Status,
                        TimeFeedback = feedback.TimeFeedback,
                        feedBackImgViewListForTourGuide = await GetFeedbackImageViewsAsync(feedback.FeedbackImages.ToList())
                    };

                    feedbackViews.Add(feedbackView);
                }


                return feedbackViews;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và ném lại
                throw new Exception($"Error occurred while fetching feedbacks for tour guide with ID {tourGuideId}: {ex.Message}");
            }
        }

        // Phương thức để lấy danh sách FeedbackImageViews từ danh sách FeedbackImage
        private async Task<List<FeedbackImageView>> GetFeedbackImageViewsAsync(List<FeedbackImage> feedbackImages)
        {
            var imageViewList = new List<FeedbackImageView>();

            foreach (var image in feedbackImages)
            {
                var imageView = new FeedbackImageView
                {
                    ImagePath = await GetImageFromCacheOrDriveAsync(image.ImagePath, "1Pp_3K7a1lZZpoZ2GX9nJGtZOAzFiqHem"),
                    UploadDate = image.UploadDate
                };

                imageViewList.Add(imageView);
            }

            return imageViewList;
        }


        public async Task UploadFeedbackAsync(FeedbackModelView feedbackModel, List<IFormFile> images)
        {
            string parentFolderId = "1Pp_3K7a1lZZpoZ2GX9nJGtZOAzFiqHem";

            try
            {
                // Create Feedback entity
                var feedback = _mapper.Map<Feedback>(feedbackModel);
                feedback.TimeFeedback = DateTime.Now;
                feedback.Status = true;
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


        public async Task<bool> UpdateStatusAsync(int feedbackId, bool newStatus)
        {
            try
            {
                var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackId);
                if (feedback == null)
                {
                    return false; // Phản hồi không tồn tại
                }

                feedback.Status = newStatus;
                await _unitOfWork.FeedbackRepository.UpdateAsync(feedback);
                return true; // Cập nhật thành công
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và ném lại
                throw new Exception($"Error occurred while updating status for feedback with ID {feedbackId}: {ex.Message}");
            }
        }

    }
}
