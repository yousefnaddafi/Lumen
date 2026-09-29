namespace Lumen.Models
{
    public class BranchProductAvailability : BaseEntity
    {
        public long BranchProductAvailabilityId { get; set; }

        public long BranchId { get; set; }
        public long ProductId { get; set; }

        // The branch-specific menu visibility.
        // If true → shows on the branch's customer menu.
        // If false → hidden for that branch, but still sellable globally.
        public bool IsAvailable { get; set; } = true;

        // Optional extras for later:
        // If you want branch-specific price override, uncomment:
        // public long? OverrideEcoPrice { get; set; }
        // public long? OverrideBusinessPrice { get; set; }

        // Sort order for the branch's menu (lower = earlier).
        public int SortOrder { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public Branch Branch { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
