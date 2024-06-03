using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class Account
    {
        public Account()
        {
            PaymentRequests = new HashSet<PaymentRequest>();
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public int AccountId { get; set; }
        public string Email { get; set; } = null!;
        public string HashedPassword { get; set; } = null!;
        public int Role { get; set; }
        public int Status { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual TourGuide? TourGuide { get; set; }
        public virtual ICollection<PaymentRequest> PaymentRequests { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
