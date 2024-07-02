using AutoMapper;
using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Loloca_BE.Business.Services.Implements
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

        public async Task<IEnumerable<FeebackView>> GetAllFeedbacksAsync()
        {
            try
            {
                var feedbacks = await _unitOfWork.FeedbackRepository.GetAsync(includeProperties: "Customer,FeedbackImages");
                var feedbackViews = new List<FeebackView>();

                foreach (var feedback in feedbacks)
                {
                    var customerName = feedback.Customer != null ? $"{feedback.Customer.FirstName} {feedback.Customer.LastName}" : "Unknown Customer";

                    var feedbackView = new FeebackView
                    {
                        FeedbackId = feedback.FeedbackId,
                        CustomerId = feedback.CustomerId,
                        CustomerName = customerName,
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
                            var imagePath = await _googleDriveService.GetImageFromCacheOrDriveAsync(image.ImagePath, "1Pp_3K7a1lZZpoZ2GX9nJGtZOAzFiqHem");
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
                    var customerName = feedback.Customer != null ? $"{feedback.Customer.FirstName} {feedback.Customer.LastName}" : "Unknown Customer";

                    var feedbackView = new GetFeedBackForCustomerView
                    {
                        FeedbackId = feedback.FeedbackId,
                        CustomerId = feedback.CustomerId,
                        CustomerName = customerName,
                        TourGuideId = feedback.TourGuideId,
                        NumOfStars = feedback.NumOfStars,
                        Content = feedback.Content,
                        Status = feedback.Status,
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
            try
            {
                // Fetch feedback including Customer and FeedbackImages
                var feedback = await _unitOfWork.FeedbackRepository.GetAsync(
                    filter: f => f.FeedbackId == feedbackId,
                    includeProperties: "Customer,FeedbackImages"
                );

                var feedbackEntity = feedback.FirstOrDefault();

                if (feedbackEntity == null)
                {
                    // Handle case where feedback is not found
                    return null;
                }

                // Load FeedbackImages explicitly if not already included
                await _unitOfWork.FeedbackRepository.LoadCollectionAsync(feedbackEntity, f => f.FeedbackImages);

                var customerName = feedbackEntity.Customer != null ? $"{feedbackEntity.Customer.FirstName} {feedbackEntity.Customer.LastName}" : "Unknown Customer";

                var feedbackView = new FeebackView
                {
                    FeedbackId = feedbackEntity.FeedbackId,
                    CustomerId = feedbackEntity.CustomerId,
                    CustomerName = customerName,
                    TourGuideId = feedbackEntity.TourGuideId,
                    NumOfStars = feedbackEntity.NumOfStars,
                    Content = feedbackEntity.Content,
                    Status = feedbackEntity.Status,
                    TimeFeedback = feedbackEntity.TimeFeedback,
                    feedBackImgViewList = new List<FeedbackImageView>()
                };

                foreach (var image in feedbackEntity.FeedbackImages)
                {
                    var imageView = new FeedbackImageView
                    {
                        ImagePath = await _googleDriveService.GetImageFromCacheOrDriveAsync(image.ImagePath, "1Pp_3K7a1lZZpoZ2GX9nJGtZOAzFiqHem"),
                        UploadDate = image.UploadDate
                    };

                    feedbackView.feedBackImgViewList.Add(imageView);
                }

                return feedbackView;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while fetching feedback with ID {feedbackId}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<GetFeedbackForTourGuideView>> GetFeedbackByTourGuideIdAsync(int tourGuideId)
        {
            try
            {
                var feedbacks = await _unitOfWork.FeedbackRepository.GetAsync(
                    filter: f => f.TourGuideId == tourGuideId && f.Status == true, // Lọc các phản hồi theo TourGuideId
                    includeProperties: "Customer,FeedbackImages"
                );

                if (feedbacks == null || !feedbacks.Any())
                {
                    return null; // Trả về null nếu không có phản hồi nào cho TourGuideId đã cho
                }

                var feedbackViews = new List<GetFeedbackForTourGuideView>();

                foreach (var feedback in feedbacks)
                {
                    await _unitOfWork.FeedbackRepository.LoadCollectionAsync(feedback, f => f.FeedbackImages);

                    var customerName = feedback.Customer != null ? $"{feedback.Customer.FirstName} {feedback.Customer.LastName}" : "Unknown Customer";

                    var feedbackView = new GetFeedbackForTourGuideView
                    {
                        FeedbackId = feedback.FeedbackId,
                        CustomerId = feedback.CustomerId,
                        CustomerName = customerName,
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
                    ImagePath = await _googleDriveService.GetImageFromCacheOrDriveAsync(image.ImagePath, "1Pp_3K7a1lZZpoZ2GX9nJGtZOAzFiqHem"),
                    UploadDate = image.UploadDate
                };

                imageViewList.Add(imageView);
            }

            return imageViewList;
        }


        //public async Task UploadFeedbackAsync(FeedbackModelView feedbackModel, List<IFormFile> images)
        //{
        //    string parentFolderId = "1Pp_3K7a1lZZpoZ2GX9nJGtZOAzFiqHem";

        //    using (var transaction = _unitOfWork.BeginTransaction())
        //    {
        //        try
        //        {
        //            // Kiểm tra tất cả các file trước khi bắt đầu upload
        //            foreach (var image in images)
        //            {
        //                if (!image.ContentType.StartsWith("image/"))
        //                {
        //                    throw new InvalidDataException("Only image files are allowed.");
        //                }
        //            }

        //            // Create Feedback entity
        //            var feedback = _mapper.Map<Feedback>(feedbackModel);
        //            feedback.TimeFeedback = DateTime.Now;
        //            feedback.Status = true;

        //            // Save Feedback to database
        //            await _unitOfWork.FeedbackRepository.InsertAsync(feedback);
        //            await _unitOfWork.SaveAsync();

        //            var feedbackId = feedback.FeedbackId;
        //            var feedbackImages = new List<FeedbackImage>();

        //            // Upload each image to Google Drive
        //            foreach (var image in images)
        //            {
        //                string fileName = $"Feedback_{feedbackId}_{Guid.NewGuid()}";

        //                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        //                {
        //                    Name = fileName,
        //                    Parents = new List<string>() { parentFolderId },
        //                    MimeType = image.ContentType
        //                };

        //                using (var stream = image.OpenReadStream())
        //                {
        //                    await _googleDriveService.UploadFileAsync(stream, fileMetadata);
        //                }

        //                // Create FeedbackImage entity
        //                var feedbackImage = new FeedbackImage
        //                {
        //                    FeedbackId = feedbackId,
        //                    ImagePath = fileName,
        //                    UploadDate = DateTime.Now
        //                };

        //                feedbackImages.Add(feedbackImage);
        //            }

        //            // Save FeedbackImages to database
        //            if (feedbackImages.Count > 0)
        //            {
        //                await _unitOfWork.FeedbackImageRepository.AddRangeAsync(feedbackImages);
        //                await _unitOfWork.SaveAsync();
        //            }

        //            // Commit the transaction
        //            await transaction.CommitAsync();
        //        }
        //        catch (InvalidDataException)
        //        {
        //            // Nếu có InvalidDataException, rollback transaction và không upload bất kỳ file nào
        //            await transaction.RollbackAsync();
        //            throw;
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback the transaction in case of error
        //            await transaction.RollbackAsync();
        //            throw new Exception("Cannot upload feedback", ex);
        //        }
        //    }
        //}

        public async Task UploadFeedbackAsync(FeedbackModelView feedbackModel, List<IFormFile> images)
        {
            string parentFolderId = "1Pp_3K7a1lZZpoZ2GX9nJGtZOAzFiqHem";

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    // Kiểm tra tất cả các file trước khi bắt đầu upload
                    foreach (var image in images)
                    {
                        if (!image.ContentType.StartsWith("image/"))
                        {
                            throw new InvalidDataException("Only image files are allowed.");
                        }
                    }

                    // Kiểm tra rằng chỉ một trong hai ID (BookingTourRequestsId hoặc BookingTourGuideRequestId) được cung cấp
                    if ((feedbackModel.BookingTourRequestsId.HasValue && feedbackModel.BookingTourGuideRequestId.HasValue) ||
                        (!feedbackModel.BookingTourRequestsId.HasValue && !feedbackModel.BookingTourGuideRequestId.HasValue))
                    {
                        throw new Exception("You must provide exactly one Booking ID.");
                    }

                    // Kiểm tra trạng thái và quyền sở hữu của BookingTourRequests hoặc BookingTourGuideRequest
                    if (feedbackModel.BookingTourRequestsId.HasValue)
                    {
                        var bookingTourRequest = await _unitOfWork.BookingTourRequestRepository.GetByIDAsync(feedbackModel.BookingTourRequestsId.Value);
                        if (bookingTourRequest == null || bookingTourRequest.Status != 3)
                        {
                            throw new Exception("Invalid or incomplete BookingTourRequest.");
                        }

                        // Kiểm tra quyền sở hữu
                        if (bookingTourRequest.CustomerId != feedbackModel.CustomerId)
                        {
                            throw new Exception("BookingTourRequest does not belong to the customer.");
                        }
                    }
                    else if (feedbackModel.BookingTourGuideRequestId.HasValue)
                    {
                        var bookingTourGuideRequest = await _unitOfWork.BookingTourGuideRepository.GetByIDAsync(feedbackModel.BookingTourGuideRequestId.Value);
                        if (bookingTourGuideRequest == null || bookingTourGuideRequest.Status != 3)
                        {
                            throw new Exception("Invalid or incomplete BookingTourGuideRequest.");
                        }

                        // Kiểm tra quyền sở hữu
                        if (bookingTourGuideRequest.CustomerId != feedbackModel.CustomerId || bookingTourGuideRequest.TourGuideId != feedbackModel.TourGuideId)
                        {
                            throw new Exception("BookingTourGuideRequest does not belong to the customer or tour guide.");
                        }
                    }

                    // Tạo Feedback entity
                    var feedback = _mapper.Map<Feedback>(feedbackModel);
                    feedback.TimeFeedback = DateTime.Now;
                    feedback.Status = true;

                    // Lưu Feedback vào cơ sở dữ liệu
                    await _unitOfWork.FeedbackRepository.InsertAsync(feedback);
                    await _unitOfWork.SaveAsync();

                    var feedbackId = feedback.FeedbackId;
                    var feedbackImages = new List<FeedbackImage>();

                    // Upload từng ảnh lên Google Drive
                    foreach (var image in images)
                    {
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

                        // Tạo FeedbackImage entity
                        var feedbackImage = new FeedbackImage
                        {
                            FeedbackId = feedbackId,
                            ImagePath = fileName,
                            UploadDate = DateTime.Now
                        };

                        feedbackImages.Add(feedbackImage);
                    }

                    // Lưu FeedbackImages vào cơ sở dữ liệu
                    if (feedbackImages.Count > 0)
                    {
                        await _unitOfWork.FeedbackImageRepository.AddRangeAsync(feedbackImages);
                        await _unitOfWork.SaveAsync();
                    }

                    // Lấy thông tin TourGuide và Customer
                    var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(feedbackModel.TourGuideId);
                    var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(feedbackModel.CustomerId);

                    // Tạo thông báo cho khách hàng
                    var notificationToCustomer = new Notification
                    {
                        UserId = customer.CustomerId,
                        UserType = "Customer",
                        Title = "Phản hồi mới",
                        Message = "Phản hồi của bạn đã được gửi thành công.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    // Tạo thông báo cho hướng dẫn viên
                    var notificationToTourGuide = new Notification
                    {
                        UserId = tourGuide.TourGuideId,
                        UserType = "TourGuide",
                        Title = "Phản hồi mới",
                        Message = "Bạn có một phản hồi mới từ khách hàng.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    await _unitOfWork.NotificationRepository.InsertAsync(notificationToCustomer);
                    await _unitOfWork.NotificationRepository.InsertAsync(notificationToTourGuide);
                    await _unitOfWork.SaveAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();
                }
                catch (InvalidDataException)
                {
                    // Nếu có InvalidDataException, rollback transaction và không upload bất kỳ file nào
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of error
                    await transaction.RollbackAsync();
                    throw new Exception("Cannot upload feedback", ex);
                }
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

        public async Task<(int count, float average)> GetFeedbackStatsAsync(int id, bool isTour)
        {
            var feedbacks = isTour
                ? await _unitOfWork.FeedbackRepository.GetAsync(f => f.BookingTourRequestsId == id)
                : await _unitOfWork.FeedbackRepository.GetAsync(f => f.BookingTourGuideRequestId == id);

            var count = feedbacks.Count();
            var average = count > 0 ? feedbacks.Average(f => f.NumOfStars) : 0;

            return (count, (float)Math.Round(average, 2));
        }




        public async Task<IEnumerable<FeebackView>> GetFeedbacksByTourIdAsync(int tourId)
        {
            try
            {
                // Fetch all BookingTourRequests for the given TourId
                var bookingTourRequests = await _unitOfWork.BookingTourRequestRepository.GetAsync(b => b.TourId == tourId);
                var bookingTourRequestIds = bookingTourRequests.Select(b => b.BookingTourRequestId).ToList();

                if (bookingTourRequestIds.Count == 0)
                {
                    return null; // No bookings found for the given TourId
                }

                // Fetch all feedbacks for the given BookingTourRequestIds
                var feedbacks = await _unitOfWork.FeedbackRepository.GetAsync(
                    f => bookingTourRequestIds.Contains(f.BookingTourRequestsId.Value),
                    includeProperties: "Customer,FeedbackImages"
                );

                if (feedbacks == null || !feedbacks.Any())
                {
                    return null; // No feedbacks found for the given TourId
                }

                var feedbackViews = new List<FeebackView>();

                foreach (var feedback in feedbacks)
                {
                    await _unitOfWork.FeedbackRepository.LoadCollectionAsync(feedback, f => f.FeedbackImages);
                    var customerName = feedback.Customer != null ? $"{feedback.Customer.FirstName} {feedback.Customer.LastName}" : "Unknown Customer";

                    var feedbackView = new FeebackView
                    {
                        FeedbackId = feedback.FeedbackId,
                        CustomerId = feedback.CustomerId,
                        CustomerName = customerName,
                        TourGuideId = feedback.TourGuideId,
                        NumOfStars = feedback.NumOfStars,
                        Content = feedback.Content,
                        Status = feedback.Status,
                        TimeFeedback = feedback.TimeFeedback,
                        feedBackImgViewList = await GetFeedbackImageViewsAsync(feedback.FeedbackImages.ToList())
                    };

                    feedbackViews.Add(feedbackView);
                }

                return feedbackViews;
            }
            catch (Exception ex)
            {
                // Handle exception and rethrow
                throw new Exception($"Error occurred while fetching feedbacks for tour with ID {tourId}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<FeebackView>> GetFeedbacksByCityIdAsync(int cityId)
        {
            try
            {
                // Fetch all TourGuides for the given CityId
                var tourGuides = await _unitOfWork.TourGuideRepository.GetAsync(tg => tg.CityId == cityId);
                var tourGuideIds = tourGuides.Select(tg => tg.TourGuideId).ToList();

                if (tourGuideIds.Count == 0)
                {
                    return null; // No tour guides found for the given CityId
                }

                // Fetch all BookingTourGuideRequests for the given TourGuideIds
                var bookingTourGuideRequests = await _unitOfWork.BookingTourGuideRepository.GetAsync(
                    btg => tourGuideIds.Contains(btg.TourGuideId));
                var bookingTourGuideRequestIds = bookingTourGuideRequests.Select(btg => btg.BookingTourGuideRequestId).ToList();

                if (bookingTourGuideRequestIds.Count == 0)
                {
                    return null; // No booking tour guide requests found for the given TourGuideIds
                }

                // Fetch all feedbacks for the given BookingTourGuideRequestIds
                var feedbacks = await _unitOfWork.FeedbackRepository.GetAsync(
                    f => bookingTourGuideRequestIds.Contains(f.BookingTourGuideRequestId.Value),
                    includeProperties: "Customer,FeedbackImages"
                );

                if (feedbacks == null || !feedbacks.Any())
                {
                    return null; // No feedbacks found for the given CityId
                }

                var feedbackViews = new List<FeebackView>();

                foreach (var feedback in feedbacks)
                {
                    await _unitOfWork.FeedbackRepository.LoadCollectionAsync(feedback, f => f.FeedbackImages);
                    var customerName = feedback.Customer != null ? $"{feedback.Customer.FirstName} {feedback.Customer.LastName}" : "Unknown Customer";

                    var feedbackView = new FeebackView
                    {
                        FeedbackId = feedback.FeedbackId,
                        CustomerId = feedback.CustomerId,
                        CustomerName = customerName,
                        TourGuideId = feedback.TourGuideId,
                        NumOfStars = feedback.NumOfStars,
                        Content = feedback.Content,
                        Status = feedback.Status,
                        TimeFeedback = feedback.TimeFeedback,
                        feedBackImgViewList = await GetFeedbackImageViewsAsync(feedback.FeedbackImages.ToList())
                    };

                    feedbackViews.Add(feedbackView);
                }

                return feedbackViews;
            }
            catch (Exception ex)
            {
                // Handle exception and rethrow
                throw new Exception($"Error occurred while fetching feedbacks for city with ID {cityId}: {ex.Message}");
            }
        }


    }
}
