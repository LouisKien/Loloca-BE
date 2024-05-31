namespace Loloca_BE.Business.Models.PaymentRequestView
{
    public class GetWithdrawalView
    {
        public int PaymentId { get; set; }
        public int AccountId { get; set; }
        public string? Email { get; set; }
        public decimal Amount { get; set; }
        public string? BankAccount { get; set; }
        public string? Bank { get; set; }
        public DateTime RequestDate { get; set; }
        public int Status { get; set; }
    }
}
