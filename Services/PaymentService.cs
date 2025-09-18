using Microsoft.EntityFrameworkCore;
using MiniPaymentGateway.Data;
using MiniPaymentGateway.DTOs;
using MiniPaymentGateway.Models;

namespace MiniPaymentGateway.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto paymentRequest);
        Task<TransactionDetailsDto?> GetTransactionAsync(Guid transactionId);
        Task<IEnumerable<TransactionDetailsDto>> GetAllTransactionsAsync();
    }

    public class PaymentService : IPaymentService
    {
        private readonly PaymentDbContext _context;
        private readonly IAcquiringBankService _bankService;

        public PaymentService(PaymentDbContext context, IAcquiringBankService bankService)
        {
            _context = context;
            _bankService = bankService;
        }

        public async Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto paymentRequest)
        {
            // Validate payment request
            ValidatePaymentRequest(paymentRequest);

            // Check if amount ends in odd number
            var amountString = paymentRequest.Amount.ToString("F2");
            var lastDigit = amountString.Replace(".", "").LastOrDefault();
            var isOddAmount = char.IsDigit(lastDigit) && (lastDigit - '0') % 2 == 1;

            // Create PaymentRequest entity
            var paymentRequestEntity = new PaymentRequest
            {
                CardNumber = paymentRequest.CardNumber,
                ExpiryMonth = paymentRequest.ExpiryMonth,
                ExpiryYear = paymentRequest.ExpiryYear,
                Cvv = paymentRequest.Cvv,
                Amount = paymentRequest.Amount,
                CurrencyCode = paymentRequest.CurrencyCode.ToUpper()
            };

            _context.PaymentRequests.Add(paymentRequestEntity);
            await _context.SaveChangesAsync();

            // Process payment with bank (only if amount doesn't end in odd number)
            BankResponse bankResponse;
            if (isOddAmount)
            {
                bankResponse = new BankResponse
                {
                    Status = "Failed",
                    Message = "Payment declined - amount ending in odd number not allowed"
                };
            }
            else
            {
                bankResponse = await _bankService.ProcessPaymentAsync(paymentRequest);
            }

            // Create Transaction entity (always store transaction regardless of odd amount validation)
            var transaction = new Transaction
            {
                PaymentRequestId = paymentRequestEntity.Id,
                MaskedCardNumber = MaskCardNumber(paymentRequest.CardNumber),
                Status = bankResponse.Status,
                BankMessage = bankResponse.Message
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Return error response for odd amounts but still provide transaction ID
            if (isOddAmount)
            {
                throw new InvalidOperationException($"Payment failed: {bankResponse.Message}. Transaction ID: {transaction.Id}");
            }

            return new PaymentResponseDto
            {
                TransactionId = transaction.Id.ToString(),
                Status = transaction.Status
            };
        }

        public async Task<TransactionDetailsDto?> GetTransactionAsync(Guid transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.PaymentRequest)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                return null;

            return new TransactionDetailsDto
            {
                TransactionId = transaction.Id.ToString(),
                MaskedCardNumber = transaction.MaskedCardNumber,
                Status = transaction.Status,
                Amount = transaction.PaymentRequest.Amount,
                CurrencyCode = transaction.PaymentRequest.CurrencyCode,
                BankMessage = transaction.BankMessage,
                ProcessedAt = transaction.ProcessedAt
            };
        }

        public async Task<IEnumerable<TransactionDetailsDto>> GetAllTransactionsAsync()
        {
            var transactions = await _context.Transactions
                .Include(t => t.PaymentRequest)
                .OrderByDescending(t => t.ProcessedAt)
                .ToListAsync();

            return transactions.Select(t => new TransactionDetailsDto
            {
                TransactionId = t.Id.ToString(),
                MaskedCardNumber = t.MaskedCardNumber,
                Status = t.Status,
                Amount = t.PaymentRequest.Amount,
                CurrencyCode = t.PaymentRequest.CurrencyCode,
                BankMessage = t.BankMessage,
                ProcessedAt = t.ProcessedAt
            });
        }

        private static void ValidatePaymentRequest(PaymentRequestDto paymentRequest)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(paymentRequest.CardNumber))
                errors.Add("Card number is required");

            if (string.IsNullOrWhiteSpace(paymentRequest.Cvv))
                errors.Add("CVV is required");

            if (string.IsNullOrWhiteSpace(paymentRequest.CurrencyCode))
                errors.Add("Currency code is required");

            if (paymentRequest.Amount <= 0)
                errors.Add("Amount must be greater than 0");

            // Validate expiry date
            var currentDate = DateTime.UtcNow;
            var expiryDate = new DateTime(paymentRequest.ExpiryYear, paymentRequest.ExpiryMonth, 1);
            var lastDayOfMonth = expiryDate.AddMonths(1).AddDays(-1);

            if (lastDayOfMonth < currentDate)
                errors.Add("Card has expired");

            if (errors.Any())
                throw new ArgumentException(string.Join("; ", errors));
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return "************";

            var lastFour = cardNumber.Substring(cardNumber.Length - 4);
            return new string('*', cardNumber.Length - 4) + lastFour;
        }
    }
}
