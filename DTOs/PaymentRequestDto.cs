using System.ComponentModel.DataAnnotations;

namespace MiniPaymentGateway.DTOs
{
    public class PaymentRequestDto
    {
        [Required(ErrorMessage = "Card number is required")]
        [StringLength(19, ErrorMessage = "Card number cannot exceed 19 characters")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiry month is required")]
        [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12")]
        public int ExpiryMonth { get; set; }

        [Required(ErrorMessage = "Expiry year is required")]
        [Range(2024, 2099, ErrorMessage = "Expiry year must be between 2024 and 2099")]
        public int ExpiryYear { get; set; }

        [Required(ErrorMessage = "CVV is required")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV must be between 3 and 4 characters")]
        public string Cvv { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Currency code is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be exactly 3 characters")]
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
