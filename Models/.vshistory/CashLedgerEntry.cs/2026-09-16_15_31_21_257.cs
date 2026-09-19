namespace Lumen.Models
{
    public class CashLedgerEntry : BaseEntity
    {
        public long CashLedgerEntryId { get; set; }
        public long BranchId { get; set; }

        public LedgerEntryType Type { get; set; }
        public LedgerCategory Category { get; set; }

        public decimal Amount { get; set; }          // always positive, direction via Type
        public string? Description { get; set; }

        public long? OrderId { get; set; }           // links sale income back to order
        public long? StockTransactionId { get; set; }// links purchase expense to stock in

        public DateTime OccurredAt { get; set; }
        public string? CreatedByUserId { get; set; }

        public Branch Branch { get; set; }
    }
}
