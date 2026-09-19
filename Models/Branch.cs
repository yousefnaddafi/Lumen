using Lumen.Services.CommonServices;

namespace Lumen.Models
{
    public class Branch : BaseEntity
    {
        public long BranchId { get; set; }
        public string Title { get; set; }          // "فردوس شرق"
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreateDate { get; set; }

        public ICollection<BranchInventory> Inventories { get; set; } = new List<BranchInventory>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
        public ICollection<CashLedgerEntry> CashLedger { get; set; } = new List<CashLedgerEntry>();
    }
}
