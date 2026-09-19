namespace Lumen.Models
{
    public class Order : BaseEntity
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

        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CreatedByUserId { get; set; }

        public Branch Branch { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
