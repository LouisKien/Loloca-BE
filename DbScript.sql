CREATE DATABASE LolocaDb
GO

USE LolocaDb

CREATE TABLE Accounts (
    AccountId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    HashedPassword NVARCHAR(255) NOT NULL,
    Role INT NOT NULL,
    Status INT NOT NULL
);

CREATE TABLE RefreshTokens (
    RefreshTokenId INT IDENTITY(1,1) PRIMARY KEY,
    AccountId INT  NOT NULL,
    Token NVARCHAR(255) NOT NULL,
    DeviceName NVARCHAR(255),
	ExpiredDate DATETIME,
    Status BIT NOT NULL,
    CONSTRAINT FK_AccountId FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
);

CREATE TABLE Cities (
    CityId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Status BIT NOT NULL
);

CREATE TABLE TourGuides (
    TourGuideId INT IDENTITY(1,1) PRIMARY KEY,
    AccountId INT UNIQUE  NOT NULL,
    CityId INT UNIQUE  NOT NULL,
    FirstName NVARCHAR(255),
    LastName NVARCHAR(255),
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
    CONSTRAINT FK_TourGuides_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId),
	CONSTRAINT FK_TourGuides_Cities FOREIGN KEY (CityId) REFERENCES Cities(CityId)
);

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
	avatarUploadTime DATETIME ,
    CONSTRAINT FK_Customers_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
);

CREATE TABLE Feedbacks (
    FeedbackId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT  NOT NULL,
    TourGuideId INT  NOT NULL,
    NumOfStars INT  NOT NULL,
    Content NVARCHAR(MAX),
	TimeFeedback DATETIME,
    Status BIT  NOT NULL,
    CONSTRAINT FK_Feedbacks_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    CONSTRAINT FK_Feedbacks_TourGuides FOREIGN KEY (TourGuideId) REFERENCES TourGuides(TourGuideId)
);

CREATE TABLE FeedbackImages (
    FeedbackImageId INT IDENTITY(1,1) PRIMARY KEY,
    FeedbackId INT NOT NULL,
    ImagePath NVARCHAR(255),
    UploadDate DATETIME,
    CONSTRAINT FK_FeedbackImage_Feedbacks FOREIGN KEY (FeedbackId) REFERENCES Feedbacks(FeedbackId)
);

CREATE TABLE Tours (
    TourId INT IDENTITY(1,1) PRIMARY KEY,
    CityId INT  NOT NULL,
    TourGuideId INT  NOT NULL,
    Name NVARCHAR(255),
    Description NVARCHAR(MAX),
    Duration INT,
    Status INT  NOT NULL,
    CONSTRAINT FK_Tours_Cities FOREIGN KEY (CityId) REFERENCES Cities(CityId),
    CONSTRAINT FK_Tours_TourGuides FOREIGN KEY (TourGuideId) REFERENCES TourGuides(TourGuideId)
);

CREATE TABLE TourImage (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,
    TourId INT  NOT NULL,
    ImagePath NVARCHAR(MAX),
    Caption NVARCHAR(MAX),
    UploadDate DATETIME,
    CONSTRAINT FK_TourImage_Tours FOREIGN KEY (TourId) REFERENCES Tours(TourId)
);

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

CREATE TABLE BookingTourRequests (
    BookingTourRequestId INT IDENTITY(1,1) PRIMARY KEY,
    TourId INT  NOT NULL,
    CustomerId INT  NOT NULL,
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

CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    BookingTourRequestsId INT ,
    BookingTourGuideRequestId INT ,
    OrderCode NVARCHAR(255) NOT NULL,
    OrderPrice FLOAT  NOT NULL,
	PaymentProvider NVARCHAR(255),
	TransactionCode NVARCHAR(255),
    Status INT NOT NULL,
    CreateAt DATETIME NOT NULL,
    
    CONSTRAINT FK_Orders_Customers
        FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
        
    CONSTRAINT FK_Orders_BookingTourGuideRequests
        FOREIGN KEY (BookingTourGuideRequestId) REFERENCES BookingTourGuideRequests(BookingTourGuideRequestId),
        
    CONSTRAINT FK_Orders_BookingTourRequests
        FOREIGN KEY (BookingTourRequestsId) REFERENCES BookingTourRequests(BookingTourRequestId),

    CHECK (
        (BookingTourGuideRequestId IS NOT NULL AND BookingTourRequestsId IS NULL)
        OR (BookingTourGuideRequestId IS NULL AND BookingTourRequestsId IS NOT NULL)
    )
);
