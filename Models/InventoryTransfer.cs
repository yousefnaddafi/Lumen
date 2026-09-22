using Lumen.Models.Enums;
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

    public class InventoryRowDto
    {
        public long BranchInventoryId { get; set; }
        public long BranchId { get; set; }
        public string BranchTitle { get; set; } = "";
        public long ProductId { get; set; }
        public string ProductTitle { get; set; } = "";
        public string? CategoryName { get; set; }
        public string? Unit { get; set; }
        public decimal QuantityOnHand { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal? MaxStock { get; set; }
        public decimal CurrentUnitCost { get; set; }
        public DateTime LastUpdated { get; set; }

        public bool IsLow => QuantityOnHand <= ReorderLevel && ReorderLevel > 0;
        public bool IsOut => QuantityOnHand <= 0;
    }

    public class AdjustStockDto
    {
        public long BranchId { get; set; }
        public long ProductId { get; set; }
        public StockTransactionType Type { get; set; } = StockTransactionType.Purchase;
        public decimal Quantity { get; set; }         // positive = in, negative = out (only used for Adjustment)
        public decimal? UnitCost { get; set; }        // for Purchase
        public string? Reference { get; set; }
        public string? Note { get; set; }
    }

    public class StockTransactionRowDto
    {
        public long StockTransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BranchTitle { get; set; } = "";
        public string ProductTitle { get; set; } = "";
        public StockTransactionType Type { get; set; }
        public decimal Quantity { get; set; }
        public decimal BalanceAfter { get; set; }
        public decimal? UnitCost { get; set; }
        public string? Reference { get; set; }
        public string? Note { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByName { get; set; }
    }

    public class SetInventoryLevelDto
    {
        public long BranchId { get; set; }
        public long ProductId { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal? MaxStock { get; set; }
    }
}
