namespace Loloca_BE.Business.Models.PaymentRequestView
{
    public class WithdrawalView
    {
        public int PaymentId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public int Type { get; set; } = 2;
        public string? BankAccount { get; set; }
        public string? Bank { get; set; }
        public DateTime RequestDate { get; set; }
        public int Status { get; set; }
    }
}
