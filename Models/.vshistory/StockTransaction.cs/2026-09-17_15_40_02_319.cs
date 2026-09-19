using Lumen.Models.Enums;
using Lumen.Services.CommonServices;

namespace Lumen.Models
{
    public class StockTransaction : BaseEntity, IHasCreatedAt, IHasCreatedBy
    {
        public long StockTransactionId { get; set; }
        public long BranchId { get; set; }
        public long ProductId { get; set; }

        public StockTransactionType Type { get; set; }

        // signed: positive = in, negative = out
        public decimal Quantity { get; set; }

        // running balance after this transaction (denormalized for fast reads)
        public decimal BalanceAfter { get; set; }

        public decimal? UnitCost { get; set; }        // for purchase / valuation
        public string? Reference { get; set; }        // order id, invoice no, etc.

        public long? OrderId { get; set; }            // if this came from a sale
        public long? RelatedTransactionId { get; set; } // pairs transfer in/out

        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserId { get; set; }

        public Branch Branch { get; set; }
        public Product Product { get; set; }
        public Order? Order { get; set; }
    }
}
