using Microsoft.AspNetCore.Mvc;
using MiniPaymentGateway.DTOs;
using MiniPaymentGateway.Services;

namespace MiniPaymentGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new payment request
        /// </summary>
        /// <param name="paymentRequest">Payment request details</param>
        /// <returns>Transaction ID and status</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment([FromBody] PaymentRequestDto paymentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _paymentService.CreatePaymentAsync(paymentRequest);
                return CreatedAtAction(nameof(GetPayment), new { transactionId = result.TransactionId }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Payment validation failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Payment failed due to business rule: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                return StatusCode(500, new { error = "Internal server error occurred while processing payment" });
            }
        }

        /// <summary>
        /// Get a specific transaction by ID
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Transaction details</returns>
        [HttpGet("{transactionId}")]
        [ProducesResponseType(typeof(TransactionDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionDetailsDto>> GetPayment(string transactionId)
        {
            try
            {
                if (!Guid.TryParse(transactionId, out var transactionGuid))
                {
                    return BadRequest(new { error = "Invalid transaction ID format" });
                }

                var transaction = await _paymentService.GetTransactionAsync(transactionGuid);
                
                if (transaction == null)
                {
                    return NotFound(new { error = "Transaction not found" });
                }

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction {TransactionId}", transactionId);
                return StatusCode(500, new { error = "Internal server error occurred while retrieving transaction" });
            }
        }

        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <returns>List of all transactions</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionDetailsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransactionDetailsDto>>> GetAllPayments()
        {
            try
            {
                var transactions = await _paymentService.GetAllTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all transactions");
                return StatusCode(500, new { error = "Internal server error occurred while retrieving transactions" });
            }
        }
    }
}
