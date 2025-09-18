using MiniPaymentGateway.DTOs;

namespace MiniPaymentGateway.Services
{
    public interface IAcquiringBankService
    {
        Task<BankResponse> ProcessPaymentAsync(PaymentRequestDto paymentRequest);
    }

    public class BankResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
