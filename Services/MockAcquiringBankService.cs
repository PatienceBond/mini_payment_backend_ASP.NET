using MiniPaymentGateway.DTOs;

namespace MiniPaymentGateway.Services
{
    public class MockAcquiringBankService : IAcquiringBankService
    {
        public async Task<BankResponse> ProcessPaymentAsync(PaymentRequestDto paymentRequest)
        {
            // Simulate bank processing delay
            await Task.Delay(100);

            // Mock logic: if last digit of card number is even → Success, else → Failed
            var lastDigit = paymentRequest.CardNumber.LastOrDefault();
            var isEven = char.IsDigit(lastDigit) && (lastDigit - '0') % 2 == 0;

            if (isEven)
            {
                return new BankResponse
                {
                    Status = "Success",
                    Message = "Payment approved"
                };
            }
            else
            {
                return new BankResponse
                {
                    Status = "Failed",
                    Message = "Payment declined"
                };
            }
        }
    }
}
