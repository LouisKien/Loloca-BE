﻿namespace Loloca_BE.Business.Models.PaymentRequestView
{
    public class WithdrawalView
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string? BankAccount { get; set; }
        public string? Bank { get; set; }
    }
}
