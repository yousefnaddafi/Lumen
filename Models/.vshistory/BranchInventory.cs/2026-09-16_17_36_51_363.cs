using Lumen.Services.CommonServices;

namespace Lumen.Models
{
    public class BranchInventory : BaseEntity, IHasCreatedBy, IHasCreatedAt
    {
        public long BranchInventoryId { get; set; }
        public long BranchId { get; set; }
        public long ProductId { get; set; }

        public decimal QuantityOnHand { get; set; }      // current stock
        public decimal ReorderLevel { get; set; }        // warning threshold
        public decimal? MaxStock { get; set; }           // optional

        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserId { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public Branch Branch { get; set; }
        public Product Product { get; set; }
    }
}
