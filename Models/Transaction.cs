using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniPaymentGateway.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PaymentRequestId { get; set; }

        [Required]
        [StringLength(20)]
        public string MaskedCardNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [StringLength(500)]
        public string BankMessage { get; set; } = string.Empty;

        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("PaymentRequestId")]
        public virtual PaymentRequest PaymentRequest { get; set; } = null!;
    }
}
