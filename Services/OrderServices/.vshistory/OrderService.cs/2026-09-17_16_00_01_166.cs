using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Models.Enums;
using Lumen.Services.CustomerServices;

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
            _db = db;
        }

        public (Order order, Customer? customer, bool customerCreated) SubmitOrder(SubmitOrderDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("سبد سفارش خالی است");

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
                    Channel = OrderChannel.InStore,
                    OrderNumber = GenerateOrderNumber(dto.BranchId),
                    IsDeleted = false
                });

                // 3) Lines
                decimal subTotal = 0, costTotal = 0;
                foreach (var line in dto.Items)
                {
                    var product = _productRepo.GetQuery()
                        .FirstOrDefault(p => p.ProductId == line.ProductId);
                    if (product == null)
                        throw new InvalidOperationException($"محصول {line.ProductId} یافت نشد");

                    if (product.TracksInventory)
                    {
                        var inv = _invRepo.GetQuery().FirstOrDefault(bi =>
                            bi.BranchId == dto.BranchId && bi.ProductId == product.ProductId);
                        if (inv == null)
                            throw new InvalidOperationException($"موجودی برای «{product.Title}» ثبت نشده");
                        if (inv.QuantityOnHand < line.Quantity)
                            throw new InvalidOperationException(
                                $"موجودی «{product.Title}» کافی نیست (موجود: {inv.QuantityOnHand})");

                        inv.QuantityOnHand -= line.Quantity;
                        inv.LastUpdated = DateTime.Now;
                        _invRepo.VoidUpdate(inv);

                        _stockRepo.Insert(new StockTransaction
                        {
                            BranchId = dto.BranchId,
                            ProductId = product.ProductId,
                            Type = StockTransactionType.Sale,
                            Quantity = -line.Quantity,
                            BalanceAfter = inv.QuantityOnHand,
                            UnitCost = product.CurrentUnitCost,
                            OrderId = order.OrderId,
                            IsDeleted = false
                        });
                    }

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
                order.Discount = dto.OrderDiscount;
                order.Tax = dto.Tax;
                order.Total = subTotal - dto.OrderDiscount + dto.Tax;
                order.CostTotal = costTotal;
                _orderRepo.VoidUpdate(order);

                // 4) Payments + ledger
                var saleCat = _ledgerCatRepo.GetQuery().FirstOrDefault(c => c.Code == "SALE");
                foreach (var p in dto.Payments)
                {
                    _paymentRepo.Insert(new Payment
                    {
                        OrderId = order.OrderId,
                        Method = p.Method,
                        Amount = p.Amount,
                        Reference = p.Reference,
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
