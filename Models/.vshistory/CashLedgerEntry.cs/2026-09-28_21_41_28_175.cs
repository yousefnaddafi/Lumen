using Lumen.Models.Enums;
using Lumen.Services.CommonServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumen.Models
{
    public class CashLedgerEntry : BaseEntity, IHasCreatedBy, IHasCreatedAt
    {
        public long CashLedgerEntryId { get; set; }
        public long BranchId { get; set; }

        public long LedgerCategoryId { get; set; }    // FK instead of enum
        public LedgerEntryType Type { get; set; }     // Income / Expense

        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public long? OrderId { get; set; }
        public long? StockTransactionId { get; set; }

        public DateTime OccurredAt { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get ; set; }

        public Branch Branch { get; set; }
        public LedgerCategory Category { get; set; }

        [NotMapped]
        public string? Image { get; set; }
    }
    public class LedgerEntryDto
    {
        public long CashLedgerEntryId { get; set; }
        public long BranchId { get; set; }
        public string BranchTitle { get; set; } = "";

        public long LedgerCategoryId { get; set; }
        public string CategoryTitle { get; set; } = "";
        public string? CategoryCode { get; set; }

        public LedgerEntryType Type { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public decimal Amount { get; set; }
        public string? Description { get; set; }

        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByName { get; set; }

        public long? OrderId { get; set; }
        public long? StockTransactionId { get; set; }
        public int AttachmentCount { get; set; }
    }

    public class LedgerEntryInputDto
    {
        public long CashLedgerEntryId { get; set; }   // 0 = new
        public long BranchId { get; set; }
        public long LedgerCategoryId { get; set; }
        public LedgerEntryType Type { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.Now;
    }

    public class LedgerFilterDto
    {
        public long? BranchId { get; set; }
        public long? CategoryId { get; set; }
        public LedgerEntryType? Type { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Search { get; set; }
    }

    public class LedgerSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetProfit => TotalIncome - TotalExpense;
        public int EntryCount { get; set; }
    }

    public class CategoryBreakdownDto
    {
        public long CategoryId { get; set; }
        public string CategoryTitle { get; set; } = "";
        public LedgerEntryType Type { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }
        public decimal PercentOfType { get; set; }
    }

    public class PeriodReportDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string Label { get; set; } = "";   // "1403/07", "شنبه 1403/07/05", "سال 1403"
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetProfit => TotalIncome - TotalExpense;
    }

}
