using Lumen.Services.CommonServices;
using System.ComponentModel.DataAnnotations;

namespace Lumen.Models
{
    public class OrderItem : BaseEntity, IHasCreatedBy, IHasCreatedAt
    {
        public long OrderItemId { get; set; }
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        [MaxLength(200)]
        public string ProductTitleSnapshot { get; set; } // denormalize — menu prices change
        public decimal UnitPrice { get; set; }           // price at sale time
        public decimal Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal LineTotal { get; set; }           // (UnitPrice * Quantity) - Discount


        public decimal UnitCostSnapshot { get; set; }    // for profit calc
        public decimal LineCost { get; set; }

        public string? Note { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserId { get; set; }
    }
}
