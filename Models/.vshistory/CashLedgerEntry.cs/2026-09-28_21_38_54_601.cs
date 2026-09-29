using Lumen.Models.Enums;
using Lumen.Services.CommonServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumen.Models
{
    public class CashLedgerEntry : BaseEntity, IHasCreatedBy, IHasCreatedAt
    {
        public long CashLedgerEntryId { get; set; }
        public long BranchId { get; set; }

        public long LedgerCategoryId { get; set; }    // FK instead of enum
        public LedgerEntryType Type { get; set; }     // Income / Expense

        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public long? OrderId { get; set; }
        public long? StockTransactionId { get; set; }

        public DateTime OccurredAt { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get ; set; }

        public Branch Branch { get; set; }
        public LedgerCategory Category { get; set; }

        [NotMapped]
        public string? Image { get; set; }
    }

    
}
