using Lumen.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Lumen.InfraStructure
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        private readonly IHttpContextAccessor _http;
        private readonly ICurrentUserService _currentUser;

        public AppDbContext(DbContextOptions<AppDbContext> options,
                            IHttpContextAccessor http,
                            ICurrentUserService currentUser) : base(options)
        {
            _http = http;
            _currentUser = currentUser;
        }

        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var userId = _currentUser.UserId; // from claims
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is IHasCreatedBy cb && string.IsNullOrEmpty(cb.CreatedByUserId))
                        cb.CreatedByUserId = userId;

                    if (entry.Entity is IHasCreatedAt ca && ca.CreatedAt == default)
                        ca.CreatedAt = now;
                }
            }
            return base.SaveChangesAsync(ct);
        }
    }
}
