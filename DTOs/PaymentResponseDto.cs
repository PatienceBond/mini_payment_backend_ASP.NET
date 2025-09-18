namespace MiniPaymentGateway.DTOs
{
    public class PaymentResponseDto
    {
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
