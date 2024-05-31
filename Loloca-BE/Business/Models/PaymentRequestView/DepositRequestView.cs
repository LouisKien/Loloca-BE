namespace Loloca_BE.Business.Models.PaymentRequestView
{
    public class DepositRequestView
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionCode { get; set; }
    }
}
