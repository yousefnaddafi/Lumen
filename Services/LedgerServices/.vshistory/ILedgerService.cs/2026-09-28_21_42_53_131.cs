using Lumen.Models;

namespace Lumen.Services.LedgerServices
{
    public interface ILedgerService
    {
        // Entries
        List<LedgerEntryDto> GetEntries(LedgerFilterDto filter, int take = 500);
        LedgerEntryDto? GetEntry(long id);
        long SaveEntry(LedgerEntryInputDto dto);
        void DeleteEntry(long id);

        // Summary
        LedgerSummaryDto GetSummary(LedgerFilterDto filter);
        List<CategoryBreakdownDto> GetCategoryBreakdown(LedgerFilterDto filter);

        // Reports
        List<PeriodReportDto> GetDailyReport(LedgerFilterDto filter);
        List<PeriodReportDto> GetMonthlyReport(LedgerFilterDto filter);
        List<PeriodReportDto> GetYearlyReport(LedgerFilterDto filter);

        // Categories
        List<LedgerCategory> GetCategories(bool onlyActive = false);
        LedgerCategory SaveCategory(LedgerCategory category);
        void DeleteCategory(long id);
        bool CanDeleteCategory(long id, out string? reason);

        // Attachments
        List<Media> GetAttachments(long entryId);
        long AddAttachment(long entryId, string base64OrPath, string? title);
        void DeleteAttachment(long mediaId);
    }
}
