﻿CREATE DATABASE LolocaDb
GO

USE LolocaDb
GO

CREATE TABLE Accounts (
    AccountId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    HashedPassword NVARCHAR(255) NOT NULL,
    Role INT NOT NULL,
    Status INT NOT NULL
);
GO

CREATE TABLE RefreshTokens (
    RefreshTokenId INT IDENTITY(1,1) PRIMARY KEY,
    AccountId INT NOT NULL,
    Token NVARCHAR(255) NOT NULL,
    DeviceName NVARCHAR(255),
    ExpiredDate DATETIME,
    Status BIT NOT NULL,
    CONSTRAINT FK_AccountId FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
);
GO

CREATE TABLE PaymentRequests (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    AccountId INT  NOT NULL,
    Amount FLOAT,
    TransactionCode NVARCHAR(255),
BankAccount NVARCHAR(255),
Bank NVARCHAR(255),
RequestDate DATETIME,
    Status INT NOT NULL,
    CONSTRAINT FK_AccountId_PaymentRequests FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
);
GO

CREATE TABLE Cities (
    CityId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Status BIT NOT NULL
);
GO

CREATE TABLE TourGuides (
    TourGuideId INT IDENTITY(1,1) PRIMARY KEY,
    AccountId INT UNIQUE NOT NULL,
    CityId INT NOT NULL,
    FirstName NVARCHAR(255),
    LastName NVARCHAR(255),
    Description NVARCHAR(MAX),
    DateOfBirth DATETIME,
    Gender INT,
    PhoneNumber NVARCHAR(255),
    Address NVARCHAR(255),
    ZaloLink NVARCHAR(255),
    FacebookLink NVARCHAR(255),
    InstagramLink NVARCHAR(255),
    PricePerDay DECIMAL(13,2),
    Status INT NOT NULL,
    AvatarPath NVARCHAR(255),
    AvatarUploadDate DATETIME,
    CoverPath NVARCHAR(255),
    CoverUploadDate DATETIME,
    Balance INT,
    CONSTRAINT FK_TourGuides_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId),
    CONSTRAINT FK_TourGuides_Cities FOREIGN KEY (CityId) REFERENCES Cities(CityId)
);
GO

CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    AccountId INT UNIQUE NOT NULL,
    FirstName NVARCHAR(255),
    Gender INT,
    LastName NVARCHAR(255),
    DateOfBirth DATETIME,
    PhoneNumber NVARCHAR(255),
    AddressCustomer NVARCHAR(255),
    AvatarPath NVARCHAR(255),
    avatarUploadTime DATETIME,
    Balance INT,
    CONSTRAINT FK_Customers_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
);
GO

CREATE TABLE Feedbacks (
    FeedbackId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    TourGuideId INT NOT NULL,
    NumOfStars INT NOT NULL,
    Content NVARCHAR(MAX),
    TimeFeedback DATETIME,
    Status BIT NOT NULL,
    CONSTRAINT FK_Feedbacks_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    CONSTRAINT FK_Feedbacks_TourGuides FOREIGN KEY (TourGuideId) REFERENCES TourGuides(TourGuideId)
);
GO

CREATE TABLE FeedbackImages (
    FeedbackImageId INT IDENTITY(1,1) PRIMARY KEY,
    FeedbackId INT NOT NULL,
    ImagePath NVARCHAR(255),
    UploadDate DATETIME,
    CONSTRAINT FK_FeedbackImage_Feedbacks FOREIGN KEY (FeedbackId) REFERENCES Feedbacks(FeedbackId)
);
GO

CREATE TABLE Tours (
    TourId INT IDENTITY(1,1) PRIMARY KEY,
    CityId INT NOT NULL,
    TourGuideId INT NOT NULL,
    Name NVARCHAR(255),
    Description NVARCHAR(MAX),
    Duration INT,
    Status INT NOT NULL,
    CONSTRAINT FK_Tours_Cities FOREIGN KEY (CityId) REFERENCES Cities(CityId),
    CONSTRAINT FK_Tours_TourGuides FOREIGN KEY (TourGuideId) REFERENCES TourGuides(TourGuideId)
);
GO

CREATE TABLE TourImage (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,
    TourId INT NOT NULL,
    ImagePath NVARCHAR(MAX),
    Caption NVARCHAR(MAX),
    UploadDate DATETIME,
    CONSTRAINT FK_TourImage_Tours FOREIGN KEY (TourId) REFERENCES Tours(TourId)
);
GO

CREATE TABLE BookingTourGuideRequests (
    BookingTourGuideRequestId INT IDENTITY(1,1) PRIMARY KEY,
    TourGuideId INT NOT NULL,
    CustomerId INT NOT NULL,
    RequestDate DATETIME NOT NULL,
    RequestTimeOut DATETIME NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    TotalPrice DECIMAL(13,2) NOT NULL,
    Note NVARCHAR(MAX),
    Status INT NOT NULL,
    CONSTRAINT FK_BookingTourGuideRequests_TourGuides FOREIGN KEY (TourGuideId) REFERENCES TourGuides(TourGuideId),
    CONSTRAINT FK_BookingTourGuideRequests_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);
GO

CREATE TABLE BookingTourRequests (
    BookingTourRequestId INT IDENTITY(1,1) PRIMARY KEY,
    TourId INT NOT NULL,
    CustomerId INT NOT NULL,
    RequestDate DATETIME NOT NULL,
    RequestTimeOut DATETIME NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    TotalPrice DECIMAL(13,2) NOT NULL,
    Note NVARCHAR(MAX),
    Status INT NOT NULL,
    CONSTRAINT FK_BookingTourRequests_Tours FOREIGN KEY (TourId) REFERENCES Tours(TourId),
    CONSTRAINT FK_BookingTourRequests_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);
GO

CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    BookingTourRequestsId INT,
    BookingTourGuideRequestId INT,
    OrderCode NVARCHAR(255) NOT NULL,
    OrderPrice FLOAT NOT NULL,
    PaymentProvider NVARCHAR(255),
    TransactionCode NVARCHAR(255),
    Status INT NOT NULL,
    CreateAt DATETIME NOT NULL,
    
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    CONSTRAINT FK_Orders_BookingTourGuideRequests FOREIGN KEY (BookingTourGuideRequestId) REFERENCES BookingTourGuideRequests(BookingTourGuideRequestId),
    CONSTRAINT FK_Orders_BookingTourRequests FOREIGN KEY (BookingTourRequestsId) REFERENCES BookingTourRequests(BookingTourRequestId),

    CHECK (
        (BookingTourGuideRequestId IS NOT NULL AND BookingTourRequestsId IS NULL)
        OR (BookingTourGuideRequestId IS NULL AND BookingTourRequestsId IS NOT NULL)
    )
);
GO

-- Tạo bảng Notifications
CREATE TABLE Notifications (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    UserType NVARCHAR(50) NOT NULL, -- 'Customer', 'TourGuide', 'Admin'
    Title NVARCHAR(255) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_UserType CHECK (UserType IN ('Customer', 'TourGuide', 'Admin'))
);
GO

-- Trigger để kiểm tra và bảo đảm UserId tồn tại trong bảng tương ứng
CREATE TRIGGER trg_Notifications_Insert
ON Notifications
FOR INSERT
AS
BEGIN
    DECLARE @UserId INT, @UserType NVARCHAR(50)
    SELECT @UserId = UserId, @UserType = UserType FROM inserted
    
    IF @UserType = 'Customer' AND NOT EXISTS (SELECT 1 FROM Customers WHERE CustomerId = @UserId)
    BEGIN
        RAISERROR ('Invalid CustomerId', 16, 1)
        ROLLBACK TRANSACTION
    END

    IF @UserType = 'TourGuide' AND NOT EXISTS (SELECT 1 FROM TourGuides WHERE TourGuideId = @UserId)
    BEGIN
        RAISERROR ('Invalid TourGuideId', 16, 1)
        ROLLBACK TRANSACTION
    END

    IF @UserType = 'Admin' AND NOT EXISTS (SELECT 1 FROM Accounts WHERE AccountId = @UserId)
    BEGIN
        RAISERROR ('Invalid Admin AccountId', 16, 1)
        ROLLBACK TRANSACTION
    END
END
GO
