using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class PaymentRequest
    {
        public int PaymentId { get; set; }
        public int AccountId { get; set; }
        public double? Amount { get; set; }
        public string? TransactionCode { get; set; }
        public string? BankAccount { get; set; }
        public string? Bank { get; set; }
        public DateTime? RequestDate { get; set; }
        public int Status { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
