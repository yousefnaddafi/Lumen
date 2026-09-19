using Lumen.Services.CommonServices;

namespace Lumen.Models
{
    public class InventoryTransfer : BaseEntity, IHasCreatedAt, IHasCreatedBy
    {
        public long InventoryTransferId { get; set; }
        public long FromBranchId { get; set; }
        public long ToBranchId { get; set; }
        public long ProductId { get; set; }

        public decimal Quantity { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime? ReceivedAt { get; set; }

        public string? Note { get; set; }

        // Pair of stock transactions written when the transfer is created / received
        public long? OutTransactionId { get; set; }
        public long? InTransactionId { get; set; }

        // Navigation
        public Branch FromBranch { get; set; } = null!;
        public Branch ToBranch { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
