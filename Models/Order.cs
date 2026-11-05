using Lumen.Models.Enums;
using Lumen.Services.CommonServices;

namespace Lumen.Models
{
    public class Order : BaseEntity, IHasCreatedBy, IHasCreatedAt
    {
        public long OrderId { get; set; }
        public long BranchId { get; set; }

        public string OrderNumber { get; set; }          // human-readable, per-branch sequence
        public OrderStatus Status { get; set; }
        public OrderChannel Channel { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public long? CustomerId { get; set; }            // if you later add Customers table

        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public decimal CostTotal { get; set; }           // COGS snapshot at sale time
        public decimal Profit => Total - CostTotal;      // not mapped

        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserId { get; set; }

        public Branch Branch { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public Customer? Customer { get; set; }
    }

    public class OrderLineDto
    {
        public long ProductId { get; set; }
        public string ProductTitle { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; } = 1;
        public decimal Discount { get; set; }
        public string? Note { get; set; }
        public decimal LineTotal => UnitPrice * Quantity - Discount;
    }

    public class PaymentDto
    {
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
    }

    public class SubmitOrderDto
    {
        public long BranchId { get; set; }
        public OrderChannel Channel { get; set; } = OrderChannel.InStore;   // 👈 ADD THIS LINE
        public string? CustomerPhone { get; set; }
        public string? CustomerName { get; set; }
        public bool CustomerIsBusiness { get; set; }
        public List<OrderLineDto> Items { get; set; } = new();
        public decimal OrderDiscount { get; set; }
        public decimal Tax { get; set; }
        public List<PaymentDto> Payments { get; set; } = new();
    }
    public class OrderDetailDto
    {
        public Order Order { get; set; } = null!;
        public string BranchTitle { get; set; } = "";
        public List<OrderItem> Items { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
        public string? CreatedByName { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
    }
}
