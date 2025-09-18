using Microsoft.EntityFrameworkCore;
using MiniPaymentGateway.Models;

namespace MiniPaymentGateway.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure PaymentRequest entity
            modelBuilder.Entity<PaymentRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CardNumber).IsRequired().HasMaxLength(19);
                entity.Property(e => e.ExpiryMonth).IsRequired();
                entity.Property(e => e.ExpiryYear).IsRequired();
                entity.Property(e => e.Cvv).IsRequired().HasMaxLength(4);
                entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

                // One-to-one relationship with Transaction
                entity.HasOne(e => e.Transaction)
                      .WithOne(e => e.PaymentRequest)
                      .HasForeignKey<Transaction>(e => e.PaymentRequestId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Transaction entity
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PaymentRequestId).IsRequired();
                entity.Property(e => e.MaskedCardNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.BankMessage).HasMaxLength(500);
                entity.Property(e => e.ProcessedAt).HasDefaultValueSql("NOW()");

                // Index for faster lookups
                entity.HasIndex(e => e.PaymentRequestId).IsUnique();
            });
        }
    }
}
