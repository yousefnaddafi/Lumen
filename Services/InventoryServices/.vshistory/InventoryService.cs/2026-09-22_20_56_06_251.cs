using Lumen.InfraStructure;
using Lumen.Models;

namespace Lumen.Services.InventoryServices
{
    public class InventoryService : IInventoryService
    {
        private readonly IProjectEFRepository<BranchInventory> _invRepo;
        private readonly IProjectEFRepository<Branch> _branchRepo;
        private readonly IProjectEFRepository<Product> _productRepo;
        private readonly IProjectEFRepository<ProductCategory> _categoryRepo;

        public InventoryService(
            IProjectEFRepository<BranchInventory> invRepo,
            IProjectEFRepository<Branch> branchRepo,
            IProjectEFRepository<Product> productRepo,
            IProjectEFRepository<ProductCategory> categoryRepo)
        {
            _invRepo = invRepo;
            _branchRepo = branchRepo;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public List<InventoryRowDto> GetInventory(long? branchId = null, string? search = null, bool onlyLow = false)
        {
            var branches = _branchRepo.GetQuery().ToDictionary(b => b.BranchId, b => b.Title);
            var products = _productRepo.GetQuery().ToDictionary(p => p.ProductId, p => p);
            var categories = _categoryRepo.GetQuery().ToDictionary(c => c.ProductCategoryId, c => c.Title);

            var query = _invRepo.GetQuery().AsQueryable();
            if (branchId.HasValue && branchId.Value > 0)
                query = query.Where(x => x.BranchId == branchId.Value);

            var list = query.ToList();

            var rows = list
                .Where(x => products.ContainsKey(x.ProductId) && branches.ContainsKey(x.BranchId))
                .Select(x =>
                {
                    var p = products[x.ProductId];
                    return new InventoryRowDto
                    {
                        BranchInventoryId = x.BranchInventoryId,
                        BranchId = x.BranchId,
                        BranchTitle = branches[x.BranchId],
                        ProductId = x.ProductId,
                        ProductTitle = p.Title,
                        CategoryName = categories.TryGetValue(p.ProductCategoryId, out var cn) ? cn : null,
                        Unit = p.Unit,
                        QuantityOnHand = x.QuantityOnHand,
                        ReorderLevel = x.ReorderLevel,
                        MaxStock = x.MaxStock,
                        CurrentUnitCost = p.CurrentUnitCost,
                        LastUpdated = x.LastUpdated
                    };
                })
                .ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                rows = rows.Where(r =>
                    r.ProductTitle.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    (r.CategoryName?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
            }

            if (onlyLow)
                rows = rows.Where(r => r.IsLow || r.IsOut).ToList();

            return rows.OrderBy(r => r.BranchTitle).ThenBy(r => r.ProductTitle).ToList();
        }

        public InventoryRowDto? GetOne(long branchId, long productId)
        {
            var inv = _invRepo.GetQuery().FirstOrDefault(x => x.BranchId == branchId && x.ProductId == productId);
            if (inv == null) return null;

            var p = _productRepo.GetQuery().FirstOrDefault(x => x.ProductId == productId);
            var b = _branchRepo.GetQuery().FirstOrDefault(x => x.BranchId == branchId);
            if (p == null || b == null) return null;

            var cat = _categoryRepo.GetQuery().FirstOrDefault(c => c.ProductCategoryId == p.ProductCategoryId);

            return new InventoryRowDto
            {
                BranchInventoryId = inv.BranchInventoryId,
                BranchId = inv.BranchId,
                BranchTitle = b.Title,
                ProductId = inv.ProductId,
                ProductTitle = p.Title,
                CategoryName = cat?.Title,
                Unit = p.Unit,
                QuantityOnHand = inv.QuantityOnHand,
                ReorderLevel = inv.ReorderLevel,
                MaxStock = inv.MaxStock,
                CurrentUnitCost = p.CurrentUnitCost,
                LastUpdated = inv.LastUpdated
            };
        }

        public void SetLevels(SetInventoryLevelDto dto)
        {
            var inv = _invRepo.GetQuery()
                .FirstOrDefault(x => x.BranchId == dto.BranchId && x.ProductId == dto.ProductId);
            if (inv == null) return;

            inv.ReorderLevel = dto.ReorderLevel;
            inv.MaxStock = dto.MaxStock;
            _invRepo.VoidUpdate(inv);
        }

        public void EnsureInventoryRowsForBranch(long branchId)
        {
            var existingProductIds = _invRepo.GetQuery()
                .Where(x => x.BranchId == branchId)
                .Select(x => x.ProductId)
                .ToHashSet();

            var allProductIds = _productRepo.GetQuery().Select(p => p.ProductId).ToList();

            foreach (var pid in allProductIds)
            {
                if (existingProductIds.Contains(pid)) continue;

                _invRepo.Insert(new BranchInventory
                {
                    BranchId = branchId,
                    ProductId = pid,
                    QuantityOnHand = 0,
                    ReorderLevel = 0,
                    LastUpdated = DateTime.Now,
                    IsDeleted = false
                });
            }
        }
    }
}
