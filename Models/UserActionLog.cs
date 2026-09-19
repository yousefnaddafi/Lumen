namespace Lumen.Models
{
    public class UserActionLog : BaseEntity
    {
        public long UserActionLogId { get; set; }
        public string UserId { get; set; }
        public long? BranchId { get; set; }
        public string Action { get; set; }       // "OrderSubmitted", "StockAdjusted", ...
        public string? EntityName { get; set; }  // "Order"
        public string? EntityId { get; set; }
        public string? Details { get; set; }     // JSON snapshot
        public DateTime OccurredAt { get; set; }
    }
}
