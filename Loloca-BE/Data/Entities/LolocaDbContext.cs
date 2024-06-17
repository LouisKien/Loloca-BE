using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Loloca_BE.Data.Entities
{
    public partial class LolocaDbContext : DbContext
    {
        public LolocaDbContext()
        {
        }

        public LolocaDbContext(DbContextOptions<LolocaDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<BookingTourGuideRequest> BookingTourGuideRequests { get; set; } = null!;
        public virtual DbSet<BookingTourRequest> BookingTourRequests { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<FeedbackImage> FeedbackImages { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<PaymentRequest> PaymentRequests { get; set; } = null!;
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public virtual DbSet<Tour> Tours { get; set; } = null!;
        public virtual DbSet<TourExclude> TourExcludes { get; set; } = null!;
        public virtual DbSet<TourGuide> TourGuides { get; set; } = null!;
        public virtual DbSet<TourHighlight> TourHighlights { get; set; } = null!;
        public virtual DbSet<TourImage> TourImages { get; set; } = null!;
        public virtual DbSet<TourInclude> TourIncludes { get; set; } = null!;
        public virtual DbSet<TourItinerary> TourItineraries { get; set; } = null!;
        public virtual DbSet<TourPrice> TourPrices { get; set; } = null!;
        public virtual DbSet<TourType> TourTypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(local);Database=LolocaDb;Uid=sa;Pwd=12345;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.HashedPassword).HasMaxLength(255);

                entity.Property(e => e.ReleaseDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<BookingTourGuideRequest>(entity =>
            {
                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.RequestDate).HasColumnType("datetime");

                entity.Property(e => e.RequestTimeOut).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(13, 2)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BookingTourGuideRequests)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingTourGuideRequests_Customers");

                entity.HasOne(d => d.TourGuide)
                    .WithMany(p => p.BookingTourGuideRequests)
                    .HasForeignKey(d => d.TourGuideId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingTourGuideRequests_TourGuides");
            });

            modelBuilder.Entity<BookingTourRequest>(entity =>
            {
                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.RequestDate).HasColumnType("datetime");

                entity.Property(e => e.RequestTimeOut).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(13, 2)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BookingTourRequests)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingTourRequests_Customers");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.BookingTourRequests)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingTourRequests_Tours");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.CityBannerUploadDate).HasColumnType("datetime");

                entity.Property(e => e.CityThumbnailUploadDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.AccountId, "UQ__Customer__349DA5A7A79F15AD")
                    .IsUnique();

                entity.Property(e => e.AddressCustomer).HasMaxLength(255);

                entity.Property(e => e.AvatarPath).HasMaxLength(255);

                entity.Property(e => e.AvatarUploadTime)
                    .HasColumnType("datetime")
                    .HasColumnName("avatarUploadTime");

                entity.Property(e => e.Balance).HasColumnType("decimal(13, 2)");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.FirstName).HasMaxLength(255);

                entity.Property(e => e.LastName).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.Customer)
                    .HasForeignKey<Customer>(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customers_Accounts");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.TimeFeedback).HasColumnType("datetime");

                entity.HasOne(d => d.BookingTourGuideRequest)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.BookingTourGuideRequestId)
                    .HasConstraintName("FK_Feedbacks_BookingTourGuideRequests");

                entity.HasOne(d => d.BookingTourRequests)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.BookingTourRequestsId)
                    .HasConstraintName("FK_Feedbacks_BookingTourRequests");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedbacks_Customers");

                entity.HasOne(d => d.TourGuide)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.TourGuideId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedbacks_TourGuides");
            });

            modelBuilder.Entity<FeedbackImage>(entity =>
            {
                entity.Property(e => e.ImagePath).HasMaxLength(255);

                entity.Property(e => e.UploadDate).HasColumnType("datetime");

                entity.HasOne(d => d.Feedback)
                    .WithMany(p => p.FeedbackImages)
                    .HasForeignKey(d => d.FeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeedbackImage_Feedbacks");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.UserType).HasMaxLength(50);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.OrderCode).HasMaxLength(255);

                entity.Property(e => e.OrderPrice).HasColumnType("decimal(13, 2)");

                entity.Property(e => e.PaymentProvider).HasMaxLength(255);

                entity.Property(e => e.TransactionCode).HasMaxLength(255);

                entity.HasOne(d => d.BookingTourGuideRequest)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.BookingTourGuideRequestId)
                    .HasConstraintName("FK_Orders_BookingTourGuideRequests");

                entity.HasOne(d => d.BookingTourRequests)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.BookingTourRequestsId)
                    .HasConstraintName("FK_Orders_BookingTourRequests");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Customers");
            });

            modelBuilder.Entity<PaymentRequest>(entity =>
            {
                entity.HasKey(e => e.PaymentId)
                    .HasName("PK__PaymentR__9B556A3810D12E70");

                entity.Property(e => e.Amount).HasColumnType("decimal(13, 2)");

                entity.Property(e => e.Bank).HasMaxLength(255);

                entity.Property(e => e.BankAccount).HasMaxLength(255);

                entity.Property(e => e.RequestDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionCode).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.PaymentRequests)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountId_PaymentRequests");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.Property(e => e.DeviceName).HasMaxLength(255);

                entity.Property(e => e.ExpiredDate).HasColumnType("datetime");

                entity.Property(e => e.Token).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountId");
            });

            modelBuilder.Entity<Tour>(entity =>
            {
                entity.Property(e => e.Activity).HasMaxLength(255);

                entity.Property(e => e.Category).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Tours)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tours_Cities");

                entity.HasOne(d => d.TourGuide)
                    .WithMany(p => p.Tours)
                    .HasForeignKey(d => d.TourGuideId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tours_TourGuides");
            });

            modelBuilder.Entity<TourExclude>(entity =>
            {
                entity.HasKey(e => e.ExcludeId)
                    .HasName("PK__TourExcl__86705AE9A0ADB2ED");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.TourExcludes)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourExcludes_Tours");
            });

            modelBuilder.Entity<TourGuide>(entity =>
            {
                entity.HasIndex(e => e.AccountId, "UQ__TourGuid__349DA5A7BD0DF942")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.AvatarPath).HasMaxLength(255);

                entity.Property(e => e.AvatarUploadDate).HasColumnType("datetime");

                entity.Property(e => e.Balance).HasColumnType("decimal(13, 2)");

                entity.Property(e => e.CoverPath).HasMaxLength(255);

                entity.Property(e => e.CoverUploadDate).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.FacebookLink).HasMaxLength(255);

                entity.Property(e => e.FirstName).HasMaxLength(255);

                entity.Property(e => e.InstagramLink).HasMaxLength(255);

                entity.Property(e => e.LastName).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(255);

                entity.Property(e => e.PricePerDay).HasColumnType("decimal(13, 2)");

                entity.Property(e => e.ZaloLink).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.TourGuide)
                    .HasForeignKey<TourGuide>(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourGuides_Accounts");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.TourGuides)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourGuides_Cities");
            });

            modelBuilder.Entity<TourHighlight>(entity =>
            {
                entity.HasKey(e => e.HighlightId)
                    .HasName("PK__TourHigh__B11CEDF028AEE066");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.TourHighlights)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourHighlights_Tours");
            });

            modelBuilder.Entity<TourImage>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__TourImag__7516F70C77BE54B2");

                entity.ToTable("TourImage");

                entity.Property(e => e.UploadDate).HasColumnType("datetime");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.TourImages)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourImage_Tours");
            });

            modelBuilder.Entity<TourInclude>(entity =>
            {
                entity.HasKey(e => e.IncludeId)
                    .HasName("PK__TourIncl__519E2C2DB24EF267");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.TourIncludes)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourIncludes_Tours");
            });

            modelBuilder.Entity<TourItinerary>(entity =>
            {
                entity.HasKey(e => e.ItineraryId)
                    .HasName("PK__TourItin__361216C6B93C2D47");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.TourItineraries)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourItineraries_Tours");
            });

            modelBuilder.Entity<TourPrice>(entity =>
            {
                entity.Property(e => e.AdultPrice).HasColumnType("decimal(13, 2)");

                entity.Property(e => e.ChildPrice).HasColumnType("decimal(13, 2)");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.TourPrices)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourPrices_Tours");
            });

            modelBuilder.Entity<TourType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK__TourType__516F03B565DBB0BE");

                entity.Property(e => e.TypeDetail).HasMaxLength(255);

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.TourTypes)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourTypes_Tours");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
