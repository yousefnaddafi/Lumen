using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Models.Enums;
using Lumen.Services.BranchServices;
using Lumen.Services.CustomerServices;
using Lumen.Services.RecipeServices;

namespace Lumen.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IProjectEFRepository<Order> _orderRepo;
        private readonly IProjectEFRepository<OrderItem> _itemRepo;
        private readonly IProjectEFRepository<Payment> _paymentRepo;
        private readonly IProjectEFRepository<Product> _productRepo;
        private readonly IProjectEFRepository<BranchInventory> _invRepo;
        private readonly IProjectEFRepository<StockTransaction> _stockRepo;
        private readonly IProjectEFRepository<LedgerCategory> _ledgerCatRepo;
        private readonly IProjectEFRepository<CashLedgerEntry> _ledgerRepo;
        private readonly ICustomerService _customerService;
        private readonly IRecipeService _recipeService;
        private readonly IBranchService _branchService;

        private readonly ProjectDBContext _db;

        public OrderService(
            IProjectEFRepository<Order> orderRepo,
            IProjectEFRepository<OrderItem> itemRepo,
            IProjectEFRepository<Payment> paymentRepo,
            IProjectEFRepository<Product> productRepo,
            IProjectEFRepository<BranchInventory> invRepo,
            IProjectEFRepository<StockTransaction> stockRepo,
            IProjectEFRepository<LedgerCategory> ledgerCatRepo,
            IProjectEFRepository<CashLedgerEntry> ledgerRepo,
            ICustomerService customerService,
            IRecipeService recipeService,
            IBranchService branchService,
            ProjectDBContext db)
        {
            _orderRepo = orderRepo;
            _itemRepo = itemRepo;
            _paymentRepo = paymentRepo;
            _productRepo = productRepo;
            _invRepo = invRepo;
            _stockRepo = stockRepo;
            _ledgerCatRepo = ledgerCatRepo;
            _ledgerRepo = ledgerRepo;
            _customerService = customerService;
            _recipeService = recipeService;
            _branchService = branchService;
            _db = db;
        }

        public (Order order, Customer? customer, bool customerCreated) SubmitOrder(SubmitOrderDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("سبد سفارش خالی است");

            using var tx = _db.Database.BeginTransaction();
            var allowedBranches = _branchService.GetMyBranches();
            if (!allowedBranches.Any(b => b.BranchId == dto.BranchId))
                throw new InvalidOperationException("شما به این شعبه دسترسی ندارید");

            using var tx = _db.Database.BeginTransaction();
            try
            {
                // 1) Customer
                Customer? customer = null;
                bool created = false;
                if (!string.IsNullOrWhiteSpace(dto.CustomerPhone))
                {
                    var existing = _customerService.GetByPhone(dto.CustomerPhone.Trim());
                    customer = existing ?? _customerService.CreateOrGetByPhone(
                        dto.CustomerPhone.Trim(),
                        dto.CustomerName ?? "",
                        dto.BranchId,
                        dto.CustomerIsBusiness);
                    created = existing == null;
                }

                // 2) Order header
                var order = _orderRepo.Insert(new Order
                {
                    BranchId = dto.BranchId,
                    CustomerId = customer?.CustomerId,
                    CustomerName = customer?.FullName ?? dto.CustomerName,
                    CustomerPhone = customer?.PhoneNumber ?? dto.CustomerPhone,
                    Status = OrderStatus.Submitted,
                    Channel = dto.Channel,               // 👈 from DTO now
                    OrderNumber = GenerateOrderNumber(dto.BranchId),
                    IsDeleted = false
                });

                // 3) Preload products, recipes and inventories in bulk
                var orderProductIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();

                var products = _productRepo.GetQuery()
                    .Where(p => orderProductIds.Contains(p.ProductId))
                    .ToList()
                    .ToDictionary(p => p.ProductId);

                // Recipes for all parent products in this order
                var recipes = _recipeService.GetForProducts(orderProductIds);

                // Union of order product ids + recipe component ids
                var componentIds = recipes.Values
                    .SelectMany(list => list.Select(r => r.ComponentProductId))
                    .Distinct()
                    .ToList();

                var idsToLoad = orderProductIds.Union(componentIds).Distinct().ToList();

                // Add any component products not already loaded
                var missingProductIds = idsToLoad.Except(products.Keys).ToList();
                if (missingProductIds.Count > 0)
                {
                    var extra = _productRepo.GetQuery()
                        .Where(p => missingProductIds.Contains(p.ProductId))
                        .ToList();
                    foreach (var p in extra)
                        products[p.ProductId] = p;
                }

                // All inventories we might touch, one query
                var inventories = _invRepo.GetQuery()
                    .Where(bi => bi.BranchId == dto.BranchId && idsToLoad.Contains(bi.ProductId))
                    .ToList()
                    .ToDictionary(bi => bi.ProductId);

                // 4) Lines
                decimal subTotal = 0, costTotal = 0;
                foreach (var line in dto.Items)
                {
                    if (!products.TryGetValue(line.ProductId, out var product))
                        throw new InvalidOperationException($"محصول {line.ProductId} یافت نشد");

                    // 👇 The whole stock movement is now handled here
                    ApplyStockDecrement(
                        order: order,
                        branchId: dto.BranchId,
                        line: line,
                        parentProduct: product,
                        channel: dto.Channel,
                        recipes: recipes,
                        products: products,
                        inventories: inventories);

                    var unitPrice = line.UnitPrice > 0
                        ? line.UnitPrice
                        : (dto.CustomerIsBusiness && product.BusinessPrice.HasValue
                            ? product.BusinessPrice.Value
                            : product.EcoPrice);

                    var lineTotal = unitPrice * line.Quantity - line.Discount;
                    var lineCost = product.CurrentUnitCost * line.Quantity;

                    _itemRepo.Insert(new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = product.ProductId,
                        ProductTitleSnapshot = product.Title,
                        UnitPrice = unitPrice,
                        Quantity = line.Quantity,
                        Discount = line.Discount,
                        LineTotal = lineTotal,
                        UnitCostSnapshot = product.CurrentUnitCost,
                        LineCost = lineCost,
                        Note = line.Note,
                        IsDeleted = false
                    });

                    subTotal += lineTotal;
                    costTotal += lineCost;
                }

                order.SubTotal = subTotal;
                //order.Discount = dto.OrderDiscount;
                order.Tax = dto.Tax;
                //order.Total = subTotal - dto.OrderDiscount + dto.Tax;
                var safeDiscount = Math.Min(dto.OrderDiscount, subTotal);
                order.Discount = safeDiscount;
                order.Total = subTotal - safeDiscount + dto.Tax;
                order.CostTotal = costTotal;
                _orderRepo.VoidUpdate(order);

                // 5) Payments + ledger
                var saleCat = _ledgerCatRepo.GetQuery().FirstOrDefault(c => c.Code == "SALE");
                foreach (var p in dto.Payments)
                {
                    _paymentRepo.Insert(new Payment
                    {
                        OrderId = order.OrderId,
                        Method = p.Method,
                        Amount = p.Amount,
                        Reference = p.Reference,
                        PaidAt = DateTime.Now,
                        IsDeleted = false
                    });

                    if (saleCat != null)
                    {
                        _ledgerRepo.Insert(new CashLedgerEntry
                        {
                            BranchId = dto.BranchId,
                            LedgerCategoryId = saleCat.LedgerCategoryId,
                            Type = LedgerEntryType.Income,
                            Amount = p.Amount,
                            PaymentMethod = p.Method,          // 👈 ADD THIS
                            OrderId = order.OrderId,
                            OccurredAt = DateTime.Now,
                            Description = $"سفارش {order.OrderNumber}",
                            IsDeleted = false
                        });
                    }
                }

                tx.Commit();
                return (order, customer, created);
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        /// <summary>
        /// For a given order line, either deplete the recipe components
        /// (if a recipe exists for the parent product), or deplete the
        /// parent product itself (fallback), or do nothing (if untracked).
        /// </summary>
        private void ApplyStockDecrement(
            Order order,
            long branchId,
            OrderLineDto line,
            Product parentProduct,
            OrderChannel channel,
            Dictionary<long, List<ProductRecipe>> recipes,
            Dictionary<long, Product> products,
            Dictionary<long, BranchInventory> inventories)
        {
            // 1) Does this product have a recipe?
            recipes.TryGetValue(parentProduct.ProductId, out var recipeList);

            var applicable = recipeList?
                .Where(r => AppliesFor(r.Condition, channel))
                .ToList();

            // 2) Recipe-driven depletion
            if (applicable != null && applicable.Count > 0)
            {
                foreach (var r in applicable)
                {
                    var consumed = r.Quantity * line.Quantity;

                    if (!products.TryGetValue(r.ComponentProductId, out var component))
                        throw new InvalidOperationException(
                            $"جزء رسپی (شناسه {r.ComponentProductId}) یافت نشد");

                    if (!inventories.TryGetValue(r.ComponentProductId, out var inv))
                        throw new InvalidOperationException(
                            $"موجودی «{component.Title}» برای این شعبه ثبت نشده است");

                    if (inv.QuantityOnHand < consumed)
                        throw new InvalidOperationException(
                            $"موجودی «{component.Title}» کافی نیست (موجود: {inv.QuantityOnHand.ToString("N2")}, نیاز: {consumed.ToString("N2")})");

                    inv.QuantityOnHand -= consumed;
                    inv.LastUpdated = DateTime.Now;
                    _invRepo.VoidUpdate(inv);

                    _stockRepo.Insert(new StockTransaction
                    {
                        BranchId = branchId,
                        ProductId = r.ComponentProductId,
                        Type = StockTransactionType.Sale,
                        Quantity = -consumed,
                        BalanceAfter = inv.QuantityOnHand,
                        UnitCost = component.CurrentUnitCost,
                        OrderId = order.OrderId,
                        Note = $"برای {parentProduct.Title}",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }

                return;   // recipe takes precedence
            }

            // 3) Fallback: deplete the product itself
            if (!parentProduct.TracksInventory)
                return;

            if (!inventories.TryGetValue(parentProduct.ProductId, out var selfInv))
                throw new InvalidOperationException(
                    $"موجودی برای «{parentProduct.Title}» در این شعبه ثبت نشده");

            if (selfInv.QuantityOnHand < line.Quantity)
                throw new InvalidOperationException(
                    $"موجودی «{parentProduct.Title}» کافی نیست (موجود: {selfInv.QuantityOnHand}, نیاز: {line.Quantity})");

            selfInv.QuantityOnHand -= line.Quantity;
            selfInv.LastUpdated = DateTime.Now;
            _invRepo.VoidUpdate(selfInv);

            _stockRepo.Insert(new StockTransaction
            {
                BranchId = branchId,
                ProductId = parentProduct.ProductId,
                Type = StockTransactionType.Sale,
                Quantity = -line.Quantity,
                BalanceAfter = selfInv.QuantityOnHand,
                UnitCost = parentProduct.CurrentUnitCost,
                OrderId = order.OrderId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            });
        }

        private static bool AppliesFor(RecipeCondition condition, OrderChannel channel) => condition switch
        {
            RecipeCondition.Always => true,
            RecipeCondition.TakeawayOnly => channel == OrderChannel.Takeaway
                                         || channel == OrderChannel.Delivery
                                         || channel == OrderChannel.Online,
            RecipeCondition.DineInOnly => channel == OrderChannel.InStore,
            _ => true
        };

        public List<Order> GetByBranch(long branchId, DateTime? from = null, DateTime? to = null)
        {
            var q = _orderRepo.GetQuery().Where(o => o.BranchId == branchId);
            if (from.HasValue) q = q.Where(o => o.CreatedAt >= from);
            if (to.HasValue) q = q.Where(o => o.CreatedAt <= to);
            return q.OrderByDescending(o => o.CreatedAt).ToList();
        }

        public Order? Get(long id) =>
            _orderRepo.GetQuery().FirstOrDefault(o => o.OrderId == id);

        private string GenerateOrderNumber(long branchId)
        {
            var count = _orderRepo.GetQuery().Count(o => o.BranchId == branchId) + 1;
            return $"BR{branchId}-{count:D5}";
        }
    }
}