namespace MiniPaymentGateway.DTOs
{
    public class TransactionDetailsDto
    {
        public string TransactionId { get; set; } = string.Empty;
        public string MaskedCardNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string BankMessage { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
    }
}
