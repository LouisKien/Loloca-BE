﻿using AutoMapper;
using Loloca_BE.Business.Models.CustomerView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Implements;
using Loloca_BE.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Drawing.Printing;
using System.Threading.Tasks;

namespace Loloca_BE.Business.Services.Implements
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IAuthenticateService _authService;
        private readonly IGoogleDriveService _googleDriveService;

        public CustomerService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IAuthenticateService authService, IGoogleDriveService googleDriveService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _authService = authService;
            _googleDriveService = googleDriveService;
        }

        public async Task UpdateCustomerInfo(int customerId, UpdateProfile model)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(customerId);
                if (customer == null)
                {
                    throw new Exception("Customer not found");
                }

                _mapper.Map(model, customer);

                await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                // Add logging or other error handling as needed
                throw new Exception("Error updating customer info", ex);
            }
        }

        public async Task<bool> ChangeCustomerPassword(int customerId, ChangePassword model)
        {
            using (var Transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(customerId);
                    if (account == null)
                    {
                        throw new Exception("Không tìm thấy khách hàng");
                    }

                    // Kiểm tra vai trò của tài khoản
                    if (account.Role != 3)
                    {
                        throw new Exception("Không được phép thay đổi mật khẩu");
                    }

                    if (!await _authService.VerifyPassword(model.OldPassword, account.HashedPassword))
                    {
                        throw new Exception("Mật khẩu hiện tại không đúng");
                    }

                    account.HashedPassword = await _authService.HashPassword(model.NewPassword);

                    await _unitOfWork.AccountRepository.UpdateAsync(account);
                    
                    var refeshTokens = await _unitOfWork.RefreshTokenRepository.GetAsync(r => r.AccountId == account.AccountId);
                    if (refeshTokens.Any())
                    {
                        foreach(var refeshToken in refeshTokens)
                        {
                            await _unitOfWork.RefreshTokenRepository.DeleteAsync(refeshToken);
                        }
                    }
                    await _unitOfWork.SaveAsync();
                    await Transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    // Thêm logging hoặc xử lý lỗi khác nếu cần
                    await Transaction.RollbackAsync();
                    throw new Exception("Lỗi khi thay đổi mật khẩu khách hàng", ex);
                }
            }
        }

        public async Task UploadAvatarAsync(IFormFile file, int CustomerId)
        {
            string parentFolderId = "1w1JtGcVwuhWnYdpGsXrTKr_dcSnGTG4Z";

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new InvalidDataException("Only image files are allowed.");
            }

            var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(CustomerId);
            if (customer == null)
            {
                throw new Exception($"TourGuide with TourGuideId {CustomerId} doesn't exist");
            }

            try
            {
                // If an avatar exists, delete the old one
                if (!customer.AvatarPath.IsNullOrEmpty() && customer.AvatarUploadTime.HasValue)
                {
                    await _googleDriveService.DeleteFileAsync(customer.AvatarPath, parentFolderId);
                }

                string guid = Guid.NewGuid().ToString();
                string fileName = $"Avatar_Customer_{CustomerId}_{guid}";

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

                customer.AvatarPath = fileName;
                customer.AvatarUploadTime = DateTime.Now;

                await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot update avatar");
            }
        }

        public async Task<List<GetCustomersView>> GetCustomers(int page, int pageSize)
        {
            try
            {
                List<GetCustomersView> customersViews = new List<GetCustomersView>();
                var customers = await _unitOfWork.CustomerRepository.GetAsync(includeProperties: "Account", pageIndex: page, pageSize: pageSize);
                foreach (var customer in customers)
                {
                    var customerView = new GetCustomersView
                    {
                        AccountStatus = customer.Account.Status,
                        AddressCustomer = customer.AddressCustomer,
                        Avatar = await _googleDriveService.GetImageFromCacheOrDriveAsync(customer.AvatarPath, "1w1JtGcVwuhWnYdpGsXrTKr_dcSnGTG4Z"),
                        AvatarUploadTime = customer.AvatarUploadTime,
                        CustomerId = customer.CustomerId,
                        DateOfBirth = customer.DateOfBirth,
                        Email = customer.Account.Email,
                        FirstName = customer.FirstName,
                        Gender = customer.Gender,
                        LastName = customer.LastName,
                        PhoneNumber = customer.PhoneNumber
                    };
                    customersViews.Add(customerView);
                }
                return customersViews;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalPage(int pageSize)
        {
            try
            {
                int total;
                total = await _unitOfWork.CustomerRepository.CountAsync();
                return (int)Math.Ceiling(total / (double)pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetCustomersView> GetCustomerById(int customerId)
        {
            try
            {
                var customer = (await _unitOfWork.CustomerRepository.GetAsync(filter: c => c.CustomerId == customerId, includeProperties: "Account")).FirstOrDefault();
                if (customer != null)
                {
                    var customerView = new GetCustomersView
                    {
                        AccountStatus = customer.Account.Status,
                        AddressCustomer = customer.AddressCustomer,
                        Avatar = await _googleDriveService.GetImageFromCacheOrDriveAsync(customer.AvatarPath, "1w1JtGcVwuhWnYdpGsXrTKr_dcSnGTG4Z"),
                        AvatarUploadTime = customer.AvatarUploadTime,
                        CustomerId = customer.CustomerId,
                        DateOfBirth = customer.DateOfBirth,
                        FirstName = customer.FirstName,
                        Gender = customer.Gender,
                        LastName = customer.LastName,
                    };
                    return customerView;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetCustomersView> GetCustomerByIdPrivate(int customerId)
        {
            try
            {
                var customer = (await _unitOfWork.CustomerRepository.GetAsync(filter: c => c.CustomerId == customerId, includeProperties: "Account")).FirstOrDefault();
                if (customer != null)
                {
                    var customerView = new GetCustomersView
                    {
                        AccountStatus = customer.Account.Status,
                        AddressCustomer = customer.AddressCustomer,
                        Avatar = await _googleDriveService.GetImageFromCacheOrDriveAsync(customer.AvatarPath, "1w1JtGcVwuhWnYdpGsXrTKr_dcSnGTG4Z"),
                        AvatarUploadTime = customer.AvatarUploadTime,
                        CustomerId = customer.CustomerId,
                        DateOfBirth = customer.DateOfBirth,
                        Email = customer.Account.Email,
                        FirstName = customer.FirstName,
                        Gender = customer.Gender,
                        LastName = customer.LastName,
                        PhoneNumber = customer.PhoneNumber,
                        Balance = customer.Balance
                    };
                    return customerView;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ChangeStatusBookingTourGuideAsync(int bookingTourGuideRequestId)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var booking = await _unitOfWork.BookingTourGuideRepository.GetByIDAsync(bookingTourGuideRequestId);

                    if (booking == null)
                    {
                        throw new Exception("Booking tour request not found.");
                    }

                    if (booking.Status == 1)
                    {
                        booking.Status = 3;
                        await _unitOfWork.BookingTourGuideRepository.UpdateAsync(booking);

                        var order = (await _unitOfWork.OrderRepository.GetAsync(o => o.BookingTourGuideRequestId == bookingTourGuideRequestId)).FirstOrDefault();
                        if (order == null)
                        {
                            throw new Exception("Order not found.");
                        }

                        var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(booking.TourGuideId);
                        if (tourGuide == null)
                        {
                            throw new Exception("Tour guide not found.");
                        }

                        // Cộng tiền vào tài khoản của hướng dẫn viên
                        tourGuide.Balance += (order.OrderPrice * (decimal)0.7);
                        await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);

                        await _unitOfWork.SaveAsync();

                        // Tạo thông báo cho khách hàng
                        var customerNotification = new Notification
                        {
                            UserId = booking.CustomerId,
                            UserType = "Customer",
                            Title = "Trạng thái đặt hướng dẫn viên đã thay đổi",
                            Message = "Chuyến đi đã đánh dấu hoàn thành.",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        };

                        // Tạo thông báo cho hướng dẫn viên
                        var tourGuideNotification = new Notification
                        {
                            UserId = booking.TourGuideId,
                            UserType = "TourGuide",
                            Title = "Trạng thái đặt hướng dẫn viên đã thay đổi",
                            Message = $"Chuyến đi đã được đánh dấu hoàn thành. Bạn đã nhận được {order.OrderPrice * (decimal)0.7} vào số dư của mình.",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        };

                        await _unitOfWork.NotificationRepository.InsertAsync(customerNotification);
                        await _unitOfWork.NotificationRepository.InsertAsync(tourGuideNotification);
                        await _unitOfWork.SaveAsync();

                        await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        throw new Exception("Cannot change status because the current status is not 1.");
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> ChangeStatusBookingTourAsync(int bookingTourRequestId)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var booking = await _unitOfWork.BookingTourRequestRepository.GetByIDAsync(bookingTourRequestId);

                    if (booking == null)
                    {
                        throw new Exception("Booking tour request not found.");
                    }

                    if (booking.Status == 1)
                    {
                        booking.Status = 3;
                        await _unitOfWork.BookingTourRequestRepository.UpdateAsync(booking);

                        var order = (await _unitOfWork.OrderRepository.GetAsync(o => o.BookingTourRequestsId == bookingTourRequestId)).FirstOrDefault();
                        if (order == null)
                        {
                            throw new Exception("Order not found.");
                        }
                        var tour = await _unitOfWork.TourRepository.GetByIDAsync(booking.TourId);
                        if (tour == null)
                        {
                            throw new Exception("Tour not found.");
                        }

                        var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(booking.Tour.TourGuideId);
                        if (tourGuide == null)
                        {
                            throw new Exception("Tour guide not found.");
                        }

                        // Cộng tiền vào tài khoản của hướng dẫn viên
                        tourGuide.Balance += (order.OrderPrice * (decimal)0.7);
                        await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);

                        await _unitOfWork.SaveAsync();

                        // Tạo thông báo cho khách hàng
                        var customerNotification = new Notification
                        {
                            UserId = booking.CustomerId,
                            UserType = "Customer",
                            Title = "Trạng thái đặt tour đã thay đổi",
                            Message = "Trạng thái đặt tour của bạn đã được thay đổi thành hoàn thành.",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        };

                        // Tạo thông báo cho hướng dẫn viên
                        var tourGuideNotification = new Notification
                        {
                            UserId = booking.Tour.TourGuideId,
                            UserType = "TourGuide",
                            Title = "Trạng thái đặt tour đã thay đổi",
                            Message = $"Chuyến đi đã được đánh dấu hoàn thành. Bạn đã nhận được {order.OrderPrice * (decimal)0.7} vào số dư của mình.",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        };

                        await _unitOfWork.NotificationRepository.InsertAsync(customerNotification);
                        await _unitOfWork.NotificationRepository.InsertAsync(tourGuideNotification);
                        await _unitOfWork.SaveAsync();

                        await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        throw new Exception("Cannot change status because the current status is not 1.");
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
