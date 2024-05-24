USE LolocaDb;

-- Insert into Accounts
INSERT INTO Accounts (Email, HashedPassword, Role, Status) VALUES
('louisnamu02@gmail.com', '2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', 1, 1),
('kiendtse161968@fpt.edu.vn', '2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', 2, 1),
('louisnamu190398@gmail.com', '2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', 3, 1),
('user4@example.com', '2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', 2, 0),
('user5@example.com', '2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', 3, 1);

-- Insert into Cities
INSERT INTO Cities (Name, Status) VALUES
('Hanoi', 1),
('Ho Chi Minh City', 1),
('Da Nang', 1),
('Nha Trang', 1),
('Hue', 1);

-- Insert into TourGuides
INSERT INTO TourGuides (AccountId, CityId, FirstName, LastName, Description, DateOfBirth, Gender, PhoneNumber, Address, ZaloLink, FacebookLink, InstagramLink, PricePerDay, Status, AvatarUploadDate, CoverUploadDate) VALUES
(2, 1, 'John', 'Doe','Im so handsome', '1985-05-15', 1, '123456789', '123 Street', 'zalo1', 'fb1', 'ig1', 100.00, 1,  '2024-01-01', '2024-01-01'),
(4, 2, 'Jane', 'Smith','My name is Dinh Trung Cho', '1990-07-20', 2, '987654321', '456 Avenue', 'zalo2', 'fb2', 'ig2', 120.00, 1,  '2024-01-01', '2024-01-01')

-- Insert into Customers
INSERT INTO Customers (AccountId, FirstName, Gender, LastName, DateOfBirth, PhoneNumber, AddressCustomer, avatarUploadTime) VALUES
(3, 'Emily', 2, 'Clark', '1993-04-22', '111222333', '321 Street', '2024-05-23'),
(5, 'Michael', 1, 'Johnson', '1987-11-13', '444555666', '654 Avenue', '2024-05-23');

-- Insert into Feedbacks
INSERT INTO Feedbacks (CustomerId, TourGuideId, NumOfStars, Content, TimeFeedback, Status) VALUES
(1, 2, 5, 'Great guide!', '2024-05-20', 1),
(2, 1, 4, 'Very good experience', '2024-05-21', 1),
(1, 2, 3, 'It was okay', '2024-05-22', 1),
(2, 1, 5, 'Highly recommend!', '2024-05-23', 1),
(1, 2, 4, 'Enjoyed the tour', '2024-05-24', 1);

-- Insert into FeedbackImages
INSERT INTO FeedbackImages (FeedbackId, UploadDate) VALUES
(1, '2024-05-20'),
(2, '2024-05-21'),
(3, '2024-05-22'),
(4, '2024-05-23'),
(5, '2024-05-24');

-- Insert into Tours
INSERT INTO Tours (CityId, TourGuideId, Name, Description, Duration, Status) VALUES
(1, 1, 'Hanoi City Tour', 'Explore the main attractions of Hanoi', 4, 1),
(2, 2, 'Ho Chi Minh Adventure', 'A thrilling journey through Ho Chi Minh City', 6, 1),
(3, 1, 'Da Nang Highlights', 'Discover the beauty of Da Nang', 5, 1),
(4, 2, 'Nha Trang Beach Day', 'Enjoy a relaxing day at the beach', 8, 1),
(5, 1, 'Hue Cultural Trip', 'Dive into the culture and history of Hue', 3, 1);

-- Insert into TourImage
INSERT INTO TourImage (TourId, Caption, UploadDate) VALUES
(1, 'Beautiful Hanoi', '2024-05-01'),
(2, 'Exciting Ho Chi Minh', '2024-05-02'),
(3, 'Stunning Da Nang', '2024-05-03'),
(4, 'Relaxing Nha Trang', '2024-05-04'),
(5, 'Historic Hue', '2024-05-05');

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
