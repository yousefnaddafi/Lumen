using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Models.Enums;
using Lumen.Services.MediaServices;
using Lumen.Services.UserServices;
using System.Linq;

namespace Lumen.Services.LedgerServices
{
    public class LedgerService : ILedgerService
    {
        private readonly IProjectEFRepository<CashLedgerEntry> _entryRepo;
        private readonly IProjectEFRepository<LedgerCategory> _catRepo;
        private readonly IProjectEFRepository<Branch> _branchRepo;
        private readonly ProjectDBContext _db;
        private readonly ICurrentUserService _currentUser;
        private readonly IProjectEFRepository<Media> _mediaRepo;
        private readonly IMediaService _mediaService;
        public LedgerService(
            IProjectEFRepository<CashLedgerEntry> entryRepo,
            IProjectEFRepository<LedgerCategory> catRepo,
            IProjectEFRepository<Branch> branchRepo,
            ProjectDBContext db,
            ICurrentUserService currentUser,
            IProjectEFRepository<Media> mediaRepo,
            IMediaService mediaService)
        {
            _entryRepo = entryRepo;
            _catRepo = catRepo;
            _branchRepo = branchRepo;
            _db = db;
            _currentUser = currentUser;
            _mediaRepo = mediaRepo;
            _mediaService = mediaService;
        }

        // ============================== ENTRIES ==============================
        public List<LedgerEntryDto> GetEntries(LedgerFilterDto filter, int take = 500)
        {
            var branches = _branchRepo.GetQuery().ToDictionary(b => b.BranchId, b => b.Title);
            var cats = _catRepo.GetQuery().ToDictionary(c => c.LedgerCategoryId, c => c);

            var q = _entryRepo.GetQuery();

            if (filter.BranchId.HasValue && filter.BranchId.Value > 0)
                q = q.Where(x => x.BranchId == filter.BranchId.Value);

            if (filter.CategoryId.HasValue && filter.CategoryId.Value > 0)
                q = q.Where(x => x.LedgerCategoryId == filter.CategoryId.Value);

            if (filter.Type.HasValue)
                q = q.Where(x => x.Type == filter.Type.Value);

            if (filter.PaymentMethod.HasValue)
                q = q.Where(x => x.PaymentMethod == filter.PaymentMethod.Value);

            if (filter.From.HasValue)
                q = q.Where(x => x.OccurredAt >= filter.From.Value.Date);

            if (filter.To.HasValue)
                q = q.Where(x => x.OccurredAt < filter.To.Value.Date.AddDays(1));

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.Trim();
                q = q.Where(x => x.Description != null && x.Description.Contains(s));
            }

            var rows = q.OrderByDescending(x => x.OccurredAt)
                        .Take(take)
                        .ToList();

            // Attachments count
            var entryIds = rows.Select(r => r.CashLedgerEntryId).ToList();
            var attachCounts = _mediaRepo.GetQuery()
    .Where(m => m.Type == MediaTypes.LedgerAttachment && entryIds.Contains(m.ObjectId))
    .GroupBy(m => m.ObjectId)
    .Select(g => new { Id = g.Key, Count = g.Count() })
    .ToDictionary(x => x.Id, x => x.Count);

            // User names
            var userIds = rows.Where(r => !string.IsNullOrEmpty(r.CreatedByUserId))
                              .Select(r => r.CreatedByUserId!)
                              .Distinct()
                              .ToList();
            var userNames = _db.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u.FullName ?? u.UserName ?? "");

            return rows.Select(r => new LedgerEntryDto
            {
                CashLedgerEntryId = r.CashLedgerEntryId,
                BranchId = r.BranchId,
                BranchTitle = branches.TryGetValue(r.BranchId, out var bn) ? bn : "",
                LedgerCategoryId = r.LedgerCategoryId,
                CategoryTitle = cats.TryGetValue(r.LedgerCategoryId, out var c) ? c.Title : "",
                CategoryCode = cats.TryGetValue(r.LedgerCategoryId, out var c2) ? c2.Code : null,
                Type = r.Type,
                PaymentMethod = r.PaymentMethod,
                Amount = r.Amount,
                Description = r.Description,
                OccurredAt = r.OccurredAt,
                CreatedAt = r.CreatedAt,
                CreatedByName = !string.IsNullOrEmpty(r.CreatedByUserId) &&
                                userNames.TryGetValue(r.CreatedByUserId, out var un) ? un : null,
                OrderId = r.OrderId,
                StockTransactionId = r.StockTransactionId,
                AttachmentCount = attachCounts.TryGetValue(r.CashLedgerEntryId, out var ac) ? ac : 0
            }).ToList();
        }

        public LedgerEntryDto? GetEntry(long id)
        {
            var r = _entryRepo.GetQuery().FirstOrDefault(x => x.CashLedgerEntryId == id);
            if (r == null) return null;

            var branch = _branchRepo.GetQuery().FirstOrDefault(b => b.BranchId == r.BranchId);
            var cat = _catRepo.GetQuery().FirstOrDefault(c => c.LedgerCategoryId == r.LedgerCategoryId);

            return new LedgerEntryDto
            {
                CashLedgerEntryId = r.CashLedgerEntryId,
                BranchId = r.BranchId,
                BranchTitle = branch?.Title ?? "",
                LedgerCategoryId = r.LedgerCategoryId,
                CategoryTitle = cat?.Title ?? "",
                CategoryCode = cat?.Code,
                Type = r.Type,
                PaymentMethod = r.PaymentMethod,
                Amount = r.Amount,
                Description = r.Description,
                OccurredAt = r.OccurredAt,
                CreatedAt = r.CreatedAt,
                OrderId = r.OrderId,
                StockTransactionId = r.StockTransactionId,
                AttachmentCount = _mediaRepo.GetQuery()
    .Count(m => m.ObjectId == r.CashLedgerEntryId
             && m.Type == MediaTypes.LedgerAttachment)
            };
        }

        public long SaveEntry(LedgerEntryInputDto dto)
        {
            if (dto.Amount <= 0)
                throw new InvalidOperationException("مبلغ باید بزرگ‌تر از صفر باشد");
            if (dto.BranchId == 0)
                throw new InvalidOperationException("شعبه را انتخاب کنید");
            if (dto.LedgerCategoryId == 0)
                throw new InvalidOperationException("دسته را انتخاب کنید");

            // Validate category direction against entry type
            var cat = _catRepo.GetQuery().FirstOrDefault(c => c.LedgerCategoryId == dto.LedgerCategoryId);
            if (cat == null)
                throw new InvalidOperationException("دسته یافت نشد");

            if (cat.Direction == LedgerDirection.Income && dto.Type != LedgerEntryType.Income)
                throw new InvalidOperationException($"دسته «{cat.Title}» فقط برای درآمد است");
            if (cat.Direction == LedgerDirection.Expense && dto.Type != LedgerEntryType.Expense)
                throw new InvalidOperationException($"دسته «{cat.Title}» فقط برای هزینه است");

            if (dto.CashLedgerEntryId == 0)
            {
                var entry = new CashLedgerEntry
                {
                    BranchId = dto.BranchId,
                    LedgerCategoryId = dto.LedgerCategoryId,
                    Type = dto.Type,
                    PaymentMethod = dto.PaymentMethod,
                    Amount = dto.Amount,
                    Description = dto.Description,
                    OccurredAt = dto.OccurredAt,
                    IsDeleted = false
                };
                _entryRepo.Insert(entry);
                return entry.CashLedgerEntryId;
            }
            else
            {
                var entry = _entryRepo.GetQuery().FirstOrDefault(x => x.CashLedgerEntryId == dto.CashLedgerEntryId);
                if (entry == null)
                    throw new InvalidOperationException("سند یافت نشد");

                entry.BranchId = dto.BranchId;
                entry.LedgerCategoryId = dto.LedgerCategoryId;
                entry.Type = dto.Type;
                entry.PaymentMethod = dto.PaymentMethod;
                entry.Amount = dto.Amount;
                entry.Description = dto.Description;
                entry.OccurredAt = dto.OccurredAt;
                _entryRepo.VoidUpdate(entry);
                return entry.CashLedgerEntryId;
            }
        }

        public void DeleteEntry(long id)
        {
            var entry = _entryRepo.GetQuery().FirstOrDefault(x => x.CashLedgerEntryId == id);
            if (entry == null) return;

            // Detach from order/stock so we don't lose the history link
            entry.OrderId = null;
            entry.StockTransactionId = null;
            entry.IsDeleted = true;
            _entryRepo.VoidUpdate(entry);
        }

        // ============================== SUMMARY ==============================
        public LedgerSummaryDto GetSummary(LedgerFilterDto filter)
        {
            var rows = GetEntries(filter, int.MaxValue);
            return new LedgerSummaryDto
            {
                TotalIncome = rows.Where(r => r.Type == LedgerEntryType.Income).Sum(r => r.Amount),
                TotalExpense = rows.Where(r => r.Type == LedgerEntryType.Expense).Sum(r => r.Amount),
                EntryCount = rows.Count
            };
        }
        public Dictionary<long, int> GetCategoryUsageCounts()
        {
            return _entryRepo.GetQuery()
                .GroupBy(e => e.LedgerCategoryId)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Id, x => x.Count);
        }
        public List<CategoryBreakdownDto> GetCategoryBreakdown(LedgerFilterDto filter)
        {
            var rows = GetEntries(filter, int.MaxValue);
            var incomeTotal = rows.Where(r => r.Type == LedgerEntryType.Income).Sum(r => r.Amount);
            var expenseTotal = rows.Where(r => r.Type == LedgerEntryType.Expense).Sum(r => r.Amount);

            var grouped = rows
                .GroupBy(r => new { r.LedgerCategoryId, r.CategoryTitle, r.Type })
                .Select(g => new CategoryBreakdownDto
                {
                    CategoryId = g.Key.LedgerCategoryId,
                    CategoryTitle = g.Key.CategoryTitle,
                    Type = g.Key.Type,
                    Total = g.Sum(x => x.Amount),
                    Count = g.Count(),
                    PercentOfType = g.Key.Type == LedgerEntryType.Income
                        ? (incomeTotal > 0 ? g.Sum(x => x.Amount) / incomeTotal * 100 : 0)
                        : (expenseTotal > 0 ? g.Sum(x => x.Amount) / expenseTotal * 100 : 0)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            return grouped;
        }

        // ============================== REPORTS ==============================
        public List<PeriodReportDto> GetDailyReport(LedgerFilterDto filter)
        {
            var rows = GetEntries(filter, int.MaxValue);
            return rows
                .GroupBy(r => r.OccurredAt.Date)
                .Select(g => new PeriodReportDto
                {
                    PeriodStart = g.Key,
                    PeriodEnd = g.Key.AddDays(1).AddTicks(-1),
                    Label = g.Key.ToString("yyyy/MM/dd"),
                    TotalIncome = g.Where(x => x.Type == LedgerEntryType.Income).Sum(x => x.Amount),
                    TotalExpense = g.Where(x => x.Type == LedgerEntryType.Expense).Sum(x => x.Amount)
                })
                .OrderBy(x => x.PeriodStart)
                .ToList();
        }

        public List<PeriodReportDto> GetMonthlyReport(LedgerFilterDto filter)
        {
            var rows = GetEntries(filter, int.MaxValue);
            return rows
                .GroupBy(r => new { r.OccurredAt.Year, r.OccurredAt.Month })
                .Select(g => new PeriodReportDto
                {
                    PeriodStart = new DateTime(g.Key.Year, g.Key.Month, 1),
                    PeriodEnd = new DateTime(g.Key.Year, g.Key.Month, 1).AddMonths(1).AddTicks(-1),
                    Label = $"{g.Key.Year:D4}/{g.Key.Month:D2}",
                    TotalIncome = g.Where(x => x.Type == LedgerEntryType.Income).Sum(x => x.Amount),
                    TotalExpense = g.Where(x => x.Type == LedgerEntryType.Expense).Sum(x => x.Amount)
                })
                .OrderBy(x => x.PeriodStart)
                .ToList();
        }

        public List<PeriodReportDto> GetYearlyReport(LedgerFilterDto filter)
        {
            var rows = GetEntries(filter, int.MaxValue);
            return rows
                .GroupBy(r => r.OccurredAt.Year)
                .Select(g => new PeriodReportDto
                {
                    PeriodStart = new DateTime(g.Key, 1, 1),
                    PeriodEnd = new DateTime(g.Key, 12, 31, 23, 59, 59),
                    Label = g.Key.ToString(),
                    TotalIncome = g.Where(x => x.Type == LedgerEntryType.Income).Sum(x => x.Amount),
                    TotalExpense = g.Where(x => x.Type == LedgerEntryType.Expense).Sum(x => x.Amount)
                })
                .OrderBy(x => x.PeriodStart)
                .ToList();
        }

        // ============================== CATEGORIES ==============================
        public List<LedgerCategory> GetCategories(bool onlyActive = false)
        {
            var q = _catRepo.GetQuery();
            if (onlyActive) q = q.Where(c => c.IsActive);
            return q.OrderBy(c => c.SortOrder).ThenBy(c => c.Title).ToList();
        }

        public LedgerCategory SaveCategory(LedgerCategory category)
        {
            if (string.IsNullOrWhiteSpace(category.Title))
                throw new InvalidOperationException("عنوان دسته الزامی است");

            // Duplicate title check
            var dup = _catRepo.GetQuery()
                .FirstOrDefault(c => c.Title == category.Title.Trim() &&
                                     c.LedgerCategoryId != category.LedgerCategoryId);
            if (dup != null)
                throw new InvalidOperationException("دسته‌ای با این عنوان قبلاً وجود دارد");

            // Code uniqueness if provided
            if (!string.IsNullOrWhiteSpace(category.Code))
            {
                var dupCode = _catRepo.GetQuery()
                    .FirstOrDefault(c => c.Code == category.Code.Trim() &&
                                         c.LedgerCategoryId != category.LedgerCategoryId);
                if (dupCode != null)
                    throw new InvalidOperationException("کد دسته تکراری است");
            }

            if (category.LedgerCategoryId == 0)
            {
                category.IsDeleted = false;
                return _catRepo.Insert(category);
            }
            else
            {
                var existing = _catRepo.GetQuery().FirstOrDefault(c => c.LedgerCategoryId == category.LedgerCategoryId);
                if (existing == null) throw new InvalidOperationException("دسته یافت نشد");
                if (existing.IsSystem && existing.Title != category.Title)
                    throw new InvalidOperationException("عنوان دسته‌های سیستمی قابل تغییر نیست");

                existing.Title = category.Title;
                existing.Code = category.Code;
                existing.Description = category.Description;
                existing.Direction = category.Direction;
                existing.SortOrder = category.SortOrder;
                existing.IsActive = category.IsActive;
                existing.ParentId = category.ParentId;
                _catRepo.VoidUpdate(existing);
                return existing;
            }
        }

        public void DeleteCategory(long id)
        {
            var cat = _catRepo.GetQuery().FirstOrDefault(c => c.LedgerCategoryId == id);
            if (cat == null) return;
            if (cat.IsSystem)
                throw new InvalidOperationException("دسته سیستمی قابل حذف نیست");

            var inUse = _entryRepo.GetQuery().Any(e => e.LedgerCategoryId == id);
            if (inUse)
                throw new InvalidOperationException("این دسته در اسناد استفاده شده و قابل حذف نیست. می‌توانید غیرفعالش کنید.");

            cat.IsDeleted = true;
            _catRepo.VoidUpdate(cat);
        }

        public bool CanDeleteCategory(long id, out string? reason)
        {
            var cat = _catRepo.GetQuery().FirstOrDefault(c => c.LedgerCategoryId == id);
            if (cat == null) { reason = "دسته یافت نشد"; return false; }
            if (cat.IsSystem) { reason = "دسته سیستمی قابل حذف نیست"; return false; }

            var inUse = _entryRepo.GetQuery().Any(e => e.LedgerCategoryId == id);
            if (inUse) { reason = "این دسته در اسناد استفاده شده است"; return false; }

            reason = null;
            return true;
        }

        // ============================== ATTACHMENTS ==============================
        public List<Media> GetAttachments(long entryId) =>
    _mediaRepo.GetQuery()
        .Where(m => m.ObjectId == entryId && m.Type == MediaTypes.LedgerAttachment)
        .OrderByDescending(m => m.CreateDate)
        .ToList();

        public long AddAttachment(long entryId, string base64OrPath, string? title)
        {
            // If it's already a saved path (no "data:" prefix), use it directly.
            // Otherwise, save via the media service.
            string fileName;

            if (base64OrPath.StartsWith("data:", StringComparison.OrdinalIgnoreCase) ||
                base64OrPath.Length > 500)
            {
                // Looks like a base64 blob — hand off to the media service
                var output = _mediaService.SaveImage(base64OrPath, EntityUrls.Ledger);
                if (!output.IsSuccess)
                    throw new InvalidOperationException("ذخیره فایل ناموفق بود");
                fileName = output.ImageName;
            }
            else
            {
                fileName = base64OrPath;
            }

            var media = new Media
            {
                ObjectId = entryId,
                PictureUrl = fileName,
                Type = MediaTypes.LedgerAttachment,
                Title = title,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                IsDeleted = false
            };
            _mediaRepo.Insert(media);
            return media.MediaId;
        }

        public void DeleteAttachment(long mediaId)
        {
            var m = _mediaRepo.GetQuery().FirstOrDefault(x => x.MediaId == mediaId);
            if (m == null) return;
            m.IsDeleted = true;
            m.UpdateDate = DateTime.Now;
            _mediaRepo.VoidUpdate(m);
        }
    }
}
