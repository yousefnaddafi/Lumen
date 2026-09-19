namespace Lumen.Models
{
    public class LedgerCategory : BaseEntity
    {
        public long LedgerCategoryId { get; set; }
        public string Title { get; set; }          // "خرید مواد اولیه"
        public string? Code { get; set; }          // optional short code like "PUR-ING"
        public string? Description { get; set; }

        // Which side of the ledger this category belongs to.
        // Income / Expense / Both
        public LedgerDirection Direction { get; set; } = LedgerDirection.Both;

        // System categories can't be renamed or deleted by admins
        // (e.g. "Sale" created by the order flow). User-created ones are editable.
        public bool IsSystem { get; set; }

        // For nicer UI grouping: parent / child
        public long? ParentId { get; set; }
        public LedgerCategory? Parent { get; set; }
        public ICollection<LedgerCategory> Children { get; set; } = new List<LedgerCategory>();

        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<CashLedgerEntry> Entries { get; set; } = new List<CashLedgerEntry>();
    }
}
