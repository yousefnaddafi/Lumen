using Lumen.Models;

namespace Lumen.Services.BranchAvailabilityServices
{
    public interface IBranchAvailabilityService
    {
        // Returns the effective menu for a branch (menu items only, respecting per-branch visibility)
        List<Product> GetMenuForBranch(long branchId);

        // All (branch, product) rows for admin UI
        List<BranchProductAvailability> GetByBranch(long branchId);

        // Toggle availability
        void SetAvailability(long branchId, long productId, bool isAvailable);

        // Batch: hide/show many
        void SetAvailabilityBatch(long branchId, IEnumerable<long> productIds, bool isAvailable);

        // Ensure a row exists (for existing products added later)
        void EnsureRow(long branchId, long productId);

        // Seed all missing rows for a branch
        void EnsureAllRowsForBranch(long branchId);
    }
}
