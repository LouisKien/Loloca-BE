﻿USE LolocaDb;

-- Insert into Accounts
INSERT INTO Accounts (Email, HashedPassword, Role, Status) VALUES
('louisnamu02@gmail.com', '2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', 1, 1),
('kiendtse161968@fpt.edu.vn', '2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', 2, 1),
('louisnamu190398@gmail.com', '2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', 3, 1),
('user4@example.com', '2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', 2, 0),
('user5@example.com', '2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', 3, 1);

-- Insert into Cities
INSERT INTO cities (name, status) VALUES
    ('An Giang', 1),
    ('Bà Rịa - Vũng Tàu', 1),
    ('Bắc Giang', 1),
    ('Bắc Kạn', 1),
    ('Bắc Ninh', 1),
    ('Bến Tre', 1),
    ('Bình Dương', 1),
    ('Bình Định', 1),
    ('Bình Phước', 1),
    ('Bình Thuận', 1),
    ('Cà Mau', 1),
    ('Cao Bằng', 1),
    ('Cần Thơ', 1),
    ('Đà Nẵng', 1),
    ('Điện Biên', 1),
    ('Đắk Lắk', 1),
    ('Đắk Nông', 1),
    ('Đồng Nai', 1),
    ('Đồng Tháp', 1),
    ('Gia Lai', 1),
    ('Hà Giang', 1),
    ('Hà Nam', 1),
    ('Hà Nội', 1),
    ('Hà Tĩnh', 1),
    ('Hải Dương', 1),
    ('Hải Phòng', 1),
    ('Hòa Bình', 1),
    ('Hậu Giang', 1),
    ('Hưng Yên', 1),
    ('Khánh Hòa', 1),
    ('Kiên Giang', 1),
    ('Kon Tum', 1),
    ('Lai Châu', 1),
    ('Lạng Sơn', 1),
    ('Lâm Đồng', 1),
    ('Long An', 1),
    ('Nam Định', 1),
    ('Nghệ An', 1),
    ('Ninh Bình', 1),
    ('Ninh Thuận', 1),
    ('Phú Thọ', 1),
    ('Quảng Bình', 1),
    ('Quảng Nam', 1),
    ('Quảng Ngãi', 1),
    ('Quảng Ninh', 1),
    ('Quảng Trị', 1),
    ('Sóc Trăng', 1),
    ('Sơn La', 1),
    ('Tây Ninh', 1),
    ('Thái Bình', 1),
    ('Thái Nguyên', 1),
    ('Thanh Hóa', 1),
    ('Thừa Thiên Huế', 1),
    ('Tiền Giang', 1),
    ('Trà Vinh', 1),
    ('Tuyên Quang', 1),
    ('Vĩnh Long', 1),
    ('Vĩnh Phúc', 1),
    ('Yên Bái', 1),
    ('Phú Yên', 1);

-- Insert into TourGuides
INSERT INTO TourGuides (AccountId, CityId, FirstName, LastName, Description, DateOfBirth, Gender, PhoneNumber, Address, ZaloLink, FacebookLink, InstagramLink, PricePerDay, Status, AvatarUploadDate, CoverUploadDate, Balance, RejectedBookingCount) VALUES
(2, 1, 'John', 'Doe','Im so handsome', '1985-05-15', 1, '123456789', '123 Street', 'zalo1', 'fb1', 'ig1', 100.00, 1,  NULL, NULL, 0, 0),
(4, 2, 'Jane', 'Smith','My name is Dinh Trung Cho', '1990-07-20', 2, '987654321', '456 Avenue', 'zalo2', 'fb2', 'ig2', 120.00, 1,  NULL, NULL, 0, 0)

-- Insert into Customers
INSERT INTO Customers (AccountId, FirstName, Gender, LastName, DateOfBirth, PhoneNumber, AddressCustomer, avatarUploadTime, Balance, CanceledBookingCount) VALUES
(3, 'Emily', 2, 'Clark', '1993-04-22', '111222333', '321 Street', NULL, 0, 0),
(5, 'Michael', 1, 'Johnson', '1987-11-13', '444555666', '654 Avenue', NULL, 0 , 0);

-- Insert into Tours
INSERT INTO Tours (CityId, TourGuideId, Name, Description, Duration, Price, Status) VALUES
(1, 1, 'Hanoi City Tour', 'Explore the main attractions of Hanoi', 4,200, 1),
(2, 2, 'Ho Chi Minh Adventure', 'A thrilling journey through Ho Chi Minh City', 6,300, 1),
(3, 1, 'Da Nang Highlights', 'Discover the beauty of Da Nang', 5,100, 1),
(4, 2, 'Nha Trang Beach Day', 'Enjoy a relaxing day at the beach', 8,500, 1),
(5, 1, 'Hue Cultural Trip', 'Dive into the culture and history of Hue', 3,560, 1);

-- Insert into BookingTourGuideRequests
INSERT INTO BookingTourGuideRequests (TourGuideId, CustomerId, RequestDate, RequestTimeOut, StartDate, EndDate, TotalPrice, Note, Status) VALUES
(1, 2, '2024-05-10', '2024-05-15', '2024-05-20', '2024-05-21', 200.00, 'Looking forward to it', 1),
(2, 1, '2024-05-11', '2024-05-16', '2024-05-22', '2024-05-23', 240.00, 'Excited for the tour', 1),
(1, 2, '2024-05-12', '2024-05-17', '2024-05-24', '2024-05-25', 220.00, 'Cant wait!', 1),
(2, 1, '2024-05-13', '2024-05-18', '2024-05-26', '2024-05-27', 260.00, 'Looking forward to it', 1),
(1, 2, '2024-05-14', '2024-05-19', '2024-05-28', '2024-05-29', 230.00, 'Excited for the tour', 1);

-- Insert into BookingTourRequests (s?a l?i cú pháp)
INSERT INTO BookingTourRequests (TourId, CustomerId, RequestDate, RequestTimeOut, StartDate, EndDate, TotalPrice, Note, Status) VALUES
(1, 1, '2024-05-05', '2024-05-10', '2024-05-15', '2024-05-16', 150.00, 'Excited for this tour', 1),
(2, 2, '2024-05-06', '2024-05-11', '2024-05-17', '2024-05-18', 180.00, 'Looking forward to it', 1),
(3, 1, '2024-05-07', '2024-05-12', '2024-05-19', '2024-05-20', 170.00, 'Can''t wait to explore!', 1),
(4, 2, '2024-05-08', '2024-05-13', '2024-05-21', '2024-05-22', 160.00, 'Very excited!', 1),
(5, 1, '2024-05-09', '2024-05-14', '2024-05-23', '2024-05-24', 190.00, 'Really looking forward to this!', 1);

-- Insert into Orders
INSERT INTO Orders(CustomerId, BookingTourRequestsId, BookingTourGuideRequestId, OrderCode, OrderPrice, PaymentProvider, TransactionCode, Status, CreateAt) VALUES
(1, 1, NULL, 'ORDER001', 150.00, 'PayPal', 'TXN001', 1, '2024-05-01'),
(2, NULL, 1, 'ORDER002', 200.00, 'Stripe', 'TXN002', 1, '2024-05-02'),
(1, 2, NULL, 'ORDER003', 180.00, 'PayPal', 'TXN003', 1, '2024-05-03'),
(2, NULL, 2, 'ORDER004', 240.00, 'Stripe', 'TXN004', 1, '2024-05-04'),
(1, 3, NULL, 'ORDER005', 170.00, 'PayPal', 'TXN005', 1, '2024-05-05');

-- Insert into Feedbacks
INSERT INTO Feedbacks (CustomerId, TourGuideId, NumOfStars, Content, TimeFeedback, Status, BookingTourRequestsId, BookingTourGuideRequestId) VALUES
(1, 2, 5, 'Great guide!', '2024-05-20', 1, 1, NULL),
(2, 1, 4, 'Very good experience', '2024-05-21', 1, NULL, 1),
(1, 2, 3, 'It was okay', '2024-05-22', 1, 2, NULL),
(2, 1, 5, 'Highly recommend!', '2024-05-23', 1, NULL, 2),
(1, 2, 4, 'Enjoyed the tour', '2024-05-24', 1, 3, NULL);

