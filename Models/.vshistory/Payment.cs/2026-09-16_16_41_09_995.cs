using Lumen.Models.Enums;

namespace Lumen.Models
{
    public class Payment : BaseEntity
    {
        public long PaymentId { get; set; }
        public long OrderId { get; set; }
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public string? Reference { get; set; }      // trace no, POS ref
        public DateTime PaidAt { get; set; }

        public Order Order { get; set; }
    }
}
