﻿using AutoMapper;
using Loloca_BE.Business.Models.CustomerView;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Loloca_BE.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IGoogleDriveService _googleDriveService;

        public CustomerService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IAuthService authService, IGoogleDriveService googleDriveService)
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
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Thêm logging hoặc xử lý lỗi khác nếu cần
                throw new Exception("Lỗi khi thay đổi mật khẩu khách hàng", ex);
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

                string fileName = $"Avatar_Customer_{CustomerId}";

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
    }
}