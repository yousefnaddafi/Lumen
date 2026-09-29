using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Models.Enums;

namespace Lumen.Services.BranchAvailabilityServices
{
    public class BranchAvailabilityService : IBranchAvailabilityService
    {
        private readonly IProjectEFRepository<BranchProductAvailability> _availRepo;
        private readonly IProjectEFRepository<Product> _productRepo;
        private readonly IProjectEFRepository<Branch> _branchRepo;

        public BranchAvailabilityService(
            IProjectEFRepository<BranchProductAvailability> availRepo,
            IProjectEFRepository<Product> productRepo,
            IProjectEFRepository<Branch> branchRepo)
        {
            _availRepo = availRepo;
            _productRepo = productRepo;
            _branchRepo = branchRepo;
        }

        public List<Product> GetMenuForBranch(long branchId)
        {
            // Menu items only, globally available,
            // AND branch availability row is either present with IsAvailable=true,
            // OR absent (falls back to global availability).
            var menuItems = _productRepo.GetQuery()
                .Where(p => p.Kind == ProductKind.MenuItem && p.IsAvailable)
                .ToList();

            var overrides = _availRepo.GetQuery()
                .Where(a => a.BranchId == branchId)
                .ToList()
                .ToDictionary(a => a.ProductId, a => a.IsAvailable);

            return menuItems
                .Where(p => !overrides.TryGetValue(p.ProductId, out var avail) || avail)
                .OrderBy(p => p.Title)
                .ToList();
        }

        public List<BranchProductAvailability> GetByBranch(long branchId) =>
            _availRepo.GetQuery()
                .Where(a => a.BranchId == branchId)
                .OrderBy(a => a.ProductId)
                .ToList();

        public void SetAvailability(long branchId, long productId, bool isAvailable)
        {
            var row = _availRepo.GetQuery()
                .FirstOrDefault(a => a.BranchId == branchId && a.ProductId == productId);
            if (row == null)
            {
                _availRepo.Insert(new BranchProductAvailability
                {
                    BranchId = branchId,
                    ProductId = productId,
                    IsAvailable = isAvailable,
                    SortOrder = 0,
                    IsDeleted = false
                });
                return;
            }
            row.IsAvailable = isAvailable;
            _availRepo.VoidUpdate(row);
        }

        public void SetAvailabilityBatch(long branchId, IEnumerable<long> productIds, bool isAvailable)
        {
            foreach (var pid in productIds.Distinct())
                SetAvailability(branchId, pid, isAvailable);
        }

        public void EnsureRow(long branchId, long productId)
        {
            var exists = _availRepo.GetQuery()
                .Any(a => a.BranchId == branchId && a.ProductId == productId);
            if (exists) return;

            _availRepo.Insert(new BranchProductAvailability
            {
                BranchId = branchId,
                ProductId = productId,
                IsAvailable = true,
                SortOrder = 0,
                IsDeleted = false
            });
        }

        public void EnsureAllRowsForBranch(long branchId)
        {
            var productIds = _productRepo.GetQuery().Select(p => p.ProductId).ToList();
            var existing = _availRepo.GetQuery()
                .Where(a => a.BranchId == branchId)
                .Select(a => a.ProductId)
                .ToHashSet();

            foreach (var pid in productIds.Where(id => !existing.Contains(id)))
            {
                _availRepo.Insert(new BranchProductAvailability
                {
                    BranchId = branchId,
                    ProductId = pid,
                    IsAvailable = true,
                    SortOrder = 0,
                    IsDeleted = false
                });
            }
        }
    }
}
