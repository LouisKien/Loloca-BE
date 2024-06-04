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
        public virtual DbSet<TourGuide> TourGuides { get; set; } = null!;
        public virtual DbSet<TourImage> TourImages { get; set; } = null!;

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
                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.AccountId, "UQ__Customer__349DA5A7EF0DCA01")
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
                    .HasName("PK__PaymentR__9B556A38F66116B5");

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
                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(13, 2)");

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

            modelBuilder.Entity<TourGuide>(entity =>
            {
                entity.HasIndex(e => e.AccountId, "UQ__TourGuid__349DA5A73891DB0E")
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

            modelBuilder.Entity<TourImage>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__TourImag__7516F70CDB8AD0F1");

                entity.ToTable("TourImage");

                entity.Property(e => e.UploadDate).HasColumnType("datetime");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.TourImages)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourImage_Tours");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
