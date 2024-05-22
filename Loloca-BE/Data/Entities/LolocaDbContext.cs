﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Loloca_BE.Data.Entities
{
    public partial class LolocaDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public LolocaDbContext()
        {
        }

        public LolocaDbContext(DbContextOptions<LolocaDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<BookingTourGuideRequest> BookingTourGuideRequests { get; set; } = null!;
        public virtual DbSet<BookingTourRequest> BookingTourRequests { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<FeedbackImage> FeedbackImages { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public virtual DbSet<Tour> Tours { get; set; } = null!;
        public virtual DbSet<TourGuide> TourGuides { get; set; } = null!;
        public virtual DbSet<TourImage> TourImages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.HashedPassword).HasMaxLength(255);
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
                entity.HasIndex(e => e.AccountId, "UQ__Customer__349DA5A7C779FE70")
                    .IsUnique();

                entity.Property(e => e.AvatarPath).HasMaxLength(255);

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

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.BookingTourRequestsId, "UQ__Orders__7E676DF6708DA8AB")
                    .IsUnique();

                entity.HasIndex(e => e.BookingTourGuideRequestId, "UQ__Orders__DE77BC49479A7658")
                    .IsUnique();

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.OrderCode).HasMaxLength(255);

                entity.Property(e => e.PaymentProvider).HasMaxLength(255);

                entity.Property(e => e.TransactionCode).HasMaxLength(255);

                entity.HasOne(d => d.BookingTourGuideRequest)
                    .WithOne(p => p.Order)
                    .HasForeignKey<Order>(d => d.BookingTourGuideRequestId)
                    .HasConstraintName("FK_Orders_BookingTourGuideRequests");

                entity.HasOne(d => d.BookingTourRequests)
                    .WithOne(p => p.Order)
                    .HasForeignKey<Order>(d => d.BookingTourRequestsId)
                    .HasConstraintName("FK_Orders_BookingTourRequests");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Customers");
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
                entity.HasIndex(e => e.AccountId, "UQ__TourGuid__349DA5A70C7235D7")
                    .IsUnique();

                entity.HasIndex(e => e.CityId, "UQ__TourGuid__F2D21B777CDF5B1D")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.AvatarPath).HasMaxLength(255);

                entity.Property(e => e.AvatarUploadDate).HasColumnType("datetime");

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
                    .WithOne(p => p.TourGuide)
                    .HasForeignKey<TourGuide>(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourGuides_Cities");
            });

            modelBuilder.Entity<TourImage>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__TourImag__7516F70C2800D1F1");

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