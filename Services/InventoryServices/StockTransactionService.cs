using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Models.Enums;
using Lumen.Services.UserServices;

namespace Lumen.Services.InventoryServices
{
    public class StockTransactionService : IStockTransactionService
    {
        private readonly IProjectEFRepository<StockTransaction> _txRepo;
        private readonly IProjectEFRepository<BranchInventory> _invRepo;
        private readonly IProjectEFRepository<Product> _productRepo;
        private readonly IProjectEFRepository<Branch> _branchRepo;
        private readonly ProjectDBContext _db;
        private readonly ICurrentUserService _currentUser;

        public StockTransactionService(
            IProjectEFRepository<StockTransaction> txRepo,
            IProjectEFRepository<BranchInventory> invRepo,
            IProjectEFRepository<Product> productRepo,
            IProjectEFRepository<Branch> branchRepo,
            ProjectDBContext db,
            ICurrentUserService currentUser)
        {
            _txRepo = txRepo;
            _invRepo = invRepo;
            _productRepo = productRepo;
            _branchRepo = branchRepo;
            _db = db;
            _currentUser = currentUser;
        }

        public void Adjust(AdjustStockDto dto)
        {
            if (dto.Quantity == 0)
                throw new InvalidOperationException("مقدار نمی‌تواند صفر باشد");

            using var tx = _db.Database.BeginTransaction();
            try
            {
                // Normalize sign: for non-Adjustment types, always use positive input and let the type determine direction
                decimal signedQty = dto.Type switch
                {
                    StockTransactionType.Purchase => Math.Abs(dto.Quantity),
                    StockTransactionType.Return => Math.Abs(dto.Quantity),
                    StockTransactionType.TransferIn => Math.Abs(dto.Quantity),
                    StockTransactionType.InitialStock => Math.Abs(dto.Quantity),
                    StockTransactionType.Sale => -Math.Abs(dto.Quantity),
                    StockTransactionType.Waste => -Math.Abs(dto.Quantity),
                    StockTransactionType.TransferOut => -Math.Abs(dto.Quantity),
                    StockTransactionType.Adjustment => dto.Quantity,  // allow negative
                    _ => dto.Quantity
                };

                var inv = _invRepo.GetQuery()
                    .FirstOrDefault(x => x.BranchId == dto.BranchId && x.ProductId == dto.ProductId);
                if (inv == null)
                    throw new InvalidOperationException("سطر موجودی برای این محصول در این شعبه یافت نشد");

                var newBalance = inv.QuantityOnHand + signedQty;
                if (newBalance < 0)
                    throw new InvalidOperationException(
                        $"موجودی کافی نیست. موجود فعلی: {inv.QuantityOnHand}, مقدار درخواستی: {Math.Abs(signedQty)}");

                inv.QuantityOnHand = newBalance;
                inv.LastUpdated = DateTime.Now;
                _invRepo.VoidUpdate(inv);

                // If Purchase with unit cost, update product's cost (weighted average or last-price)
                if (dto.Type == StockTransactionType.Purchase && dto.UnitCost.HasValue && dto.UnitCost.Value > 0)
                {
                    var product = _productRepo.GetQuery().FirstOrDefault(p => p.ProductId == dto.ProductId);
                    if (product != null)
                    {
                        // simple weighted average
                        decimal oldValue = product.CurrentUnitCost * (newBalance - signedQty);
                        decimal newValue = dto.UnitCost.Value * signedQty;
                        var totalQty = newBalance;
                        product.CurrentUnitCost = totalQty > 0 ? (oldValue + newValue) / totalQty : dto.UnitCost.Value;
                        _productRepo.VoidUpdate(product);
                    }
                }

                _txRepo.Insert(new StockTransaction
                {
                    BranchId = dto.BranchId,
                    ProductId = dto.ProductId,
                    Type = dto.Type,
                    Quantity = signedQty,
                    BalanceAfter = newBalance,
                    UnitCost = dto.UnitCost,
                    Reference = dto.Reference,
                    Note = dto.Note,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = _currentUser.UserId,
                    IsDeleted = false
                });

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public List<StockTransactionRowDto> GetHistory(long? branchId = null, long? productId = null,
                                                        DateTime? from = null, DateTime? to = null,
                                                        StockTransactionType? type = null)
        {
            var branchTitles = _branchRepo.GetQuery().ToDictionary(b => b.BranchId, b => b.Title);
            var productTitles = _productRepo.GetQuery().ToDictionary(p => p.ProductId, p => p.Title);

            var q = _txRepo.GetQuery();
            if (branchId.HasValue && branchId.Value > 0) q = q.Where(x => x.BranchId == branchId.Value);
            if (productId.HasValue && productId.Value > 0) q = q.Where(x => x.ProductId == productId.Value);
            if (from.HasValue) q = q.Where(x => x.CreatedAt >= from.Value.Date);
            if (to.HasValue) q = q.Where(x => x.CreatedAt < to.Value.Date.AddDays(1));
            if (type.HasValue) q = q.Where(x => x.Type == type.Value);

            var rows = q.OrderByDescending(x => x.CreatedAt)
                        .Take(500)
                        .ToList()
                        .Where(x => branchTitles.ContainsKey(x.BranchId) && productTitles.ContainsKey(x.ProductId))
                        .Select(x => new StockTransactionRowDto
                        {
                            StockTransactionId = x.StockTransactionId,
                            CreatedAt = x.CreatedAt,
                            BranchTitle = branchTitles[x.BranchId],
                            ProductTitle = productTitles[x.ProductId],
                            Type = x.Type,
                            Quantity = x.Quantity,
                            BalanceAfter = x.BalanceAfter,
                            UnitCost = x.UnitCost,
                            Reference = x.Reference,
                            Note = x.Note,
                            CreatedByUserId = x.CreatedByUserId
                        })
                        .ToList();

            // Resolve user names
            if (rows.Any(r => !string.IsNullOrEmpty(r.CreatedByUserId)))
            {
                var ids = rows.Where(r => !string.IsNullOrEmpty(r.CreatedByUserId))
                              .Select(r => r.CreatedByUserId!).Distinct().ToList();
                var users = _db.Users.Where(u => ids.Contains(u.Id))
                                     .ToDictionary(u => u.Id, u => u.FullName ?? u.UserName ?? "");
                foreach (var r in rows)
                {
                    if (!string.IsNullOrEmpty(r.CreatedByUserId) &&
                        users.TryGetValue(r.CreatedByUserId, out var name))
                        r.CreatedByName = name;
                }
            }

            return rows;
        }
    }
}
