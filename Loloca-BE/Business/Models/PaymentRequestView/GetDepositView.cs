namespace Loloca_BE.Business.Models.PaymentRequestView
{
    public class GetDepositView
    {
        public int PaymentId { get; set; }
        public int AccountId { get; set; }
        public string? Email { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionCode { get; set; }
        public DateTime RequestDate { get; set; }
        public int Status { get; set; }
    }
}
