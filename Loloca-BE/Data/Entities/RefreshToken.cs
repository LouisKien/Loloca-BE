using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class RefreshToken
    {
        public int RefreshTokenId { get; set; }
        public int AccountId { get; set; }
        public string Token { get; set; } = null!;
        public string? DeviceName { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public bool Status { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
