using Lumen.Models;
using Lumen.Models.Enums;
using Lumen.Services.CommonServices;
using Lumen.Services.UserServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lumen.InfraStructure
{
    
        public class ProjectDBContext : IdentityDbContext<AppUser, AppRole, string>
        {
            private readonly ICurrentUserService? _currentUser;

            public ProjectDBContext() { }

            public ProjectDBContext(
                DbContextOptions<ProjectDBContext> options,
                ICurrentUserService currentUser) : base(options)
            {
                _currentUser = currentUser;
            }

            // ----- Menu / catalog -----
            public DbSet<Media> Medias { get; set; } = null!;
            public DbSet<Product> Products { get; set; } = null!;
            public DbSet<ProductCategory> ProductCategories { get; set; } = null!;

            // ----- Branches -----
            public DbSet<Branch> Branches { get; set; } = null!;
            public DbSet<BranchInventory> BranchInventories { get; set; } = null!;
            public DbSet<StockTransaction> StockTransactions { get; set; } = null!;
            public DbSet<InventoryTransfer> InventoryTransfers { get; set; } = null!;

            // ----- Orders -----
            public DbSet<Order> Orders { get; set; } = null!;
            public DbSet<OrderItem> OrderItems { get; set; } = null!;
            public DbSet<Payment> Payments { get; set; } = null!;

            // ----- Accounting -----
            public DbSet<LedgerCategory> LedgerCategories { get; set; } = null!;
            public DbSet<CashLedgerEntry> CashLedgerEntries { get; set; } = null!;

            // ----- Users -----
            public DbSet<AppUser> AppUsers { get; set; } = null!;
            public DbSet<AppRole> AppRoles { get; set; } = null!;
            public DbSet<UserBranch> UserBranches { get; set; } = null!;
            public DbSet<UserActionLog> UserActionLogs { get; set; } = null!;

            protected override void OnModelCreating(ModelBuilder b)
            {
                // Always call base first — IdentityDbContext configures the AspNet* tables.
                base.OnModelCreating(b);

                // ================================================================
                // Global convention: soft-delete query filter on BaseEntity
                // ================================================================
                foreach (var entityType in b.Model.GetEntityTypes())
                {
                    if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                    {
                        var param = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                        var prop = System.Linq.Expressions.Expression.Property(param, nameof(BaseEntity.IsDeleted));
                        var filter = System.Linq.Expressions.Expression.Lambda(
                            System.Linq.Expressions.Expression.Equal(prop,
                                System.Linq.Expressions.Expression.Constant(false)),
                            param);
                        b.Entity(entityType.ClrType).HasQueryFilter(filter);
                    }
                }

                // ================================================================
                // Menu
                // ================================================================
                b.Entity<Media>(e =>
                {
                    e.ToTable("Medias", "dbo");
                    e.HasKey(x => x.MediaId);
                    e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
                });

            b.Entity<AppRole>().HasData(
new AppRole { Id = "role-owner", Name = "Owner", NormalizedName = "OWNER" },
new AppRole { Id = "role-manager", Name = "BranchManager", NormalizedName = "BRANCHMANAGER" },
new AppRole { Id = "role-cashier", Name = "Cashier", NormalizedName = "CASHIER" },
new AppRole { Id = "role-barista", Name = "Barista", NormalizedName = "BARISTA" },
new AppRole { Id = "role-account", Name = "Accountant", NormalizedName = "ACCOUNTANT" },
new AppRole { Id = "role-menu", Name = "MenuAdmin", NormalizedName = "MENUADMIN" }
);
            b.Entity<Product>(e =>
                {
                    e.ToTable("Products", "dbo");
                    e.HasKey(x => x.ProductId);
                    e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");

                    e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                    e.Property(x => x.Description).HasMaxLength(2000);
                    e.Property(x => x.Unit).HasMaxLength(20);
                    e.Property(x => x.CurrentUnitCost).HasColumnType("decimal(18,4)");

                    e.HasOne<ProductCategory>()
                     .WithMany()
                     .HasForeignKey(x => x.ProductCategoryId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasIndex(x => x.ProductCategoryId);
                    e.HasIndex(x => x.Title);
                });

                b.Entity<ProductCategory>(e =>
                {
                    e.ToTable("ProductCategories", "dbo");
                    e.HasKey(x => x.ProductCategoryId);
                    e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                });

                // ================================================================
                // Branch
                // ================================================================
                b.Entity<Branch>(e =>
                {
                    e.ToTable("Branches", "dbo");
                    e.HasKey(x => x.BranchId);
                    e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                    e.Property(x => x.Address).HasMaxLength(500);
                    e.Property(x => x.Phone).HasMaxLength(50);
                    e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
                    e.HasIndex(x => x.Title);
                });

                // ================================================================
                // BranchInventory
                // ================================================================
                b.Entity<BranchInventory>(e =>
                {
                    e.ToTable("BranchInventories", "dbo");
                    e.HasKey(x => x.BranchInventoryId);
                    e.Property(x => x.QuantityOnHand).HasColumnType("decimal(18,4)");
                    e.Property(x => x.ReorderLevel).HasColumnType("decimal(18,4)");
                    e.Property(x => x.MaxStock).HasColumnType("decimal(18,4)");
                    e.Property(x => x.RowVersion).IsRowVersion();

                    e.HasOne(x => x.Branch)
                     .WithMany(x => x.Inventories)
                     .HasForeignKey(x => x.BranchId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasOne(x => x.Product)
                     .WithMany()
                     .HasForeignKey(x => x.ProductId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasIndex(x => new { x.BranchId, x.ProductId }).IsUnique();
                    e.HasIndex(x => new { x.BranchId, x.ProductId, x.LastUpdated });
                });

                // ================================================================
                // StockTransaction (append-only ledger)
                // ================================================================
                b.Entity<StockTransaction>(e =>
                {
                    e.ToTable("StockTransactions", "dbo");
                    e.HasKey(x => x.StockTransactionId);
                    e.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
                    e.Property(x => x.BalanceAfter).HasColumnType("decimal(18,4)");
                    e.Property(x => x.UnitCost).HasColumnType("decimal(18,4)");
                    e.Property(x => x.Reference).HasMaxLength(100);
                    e.Property(x => x.Note).HasMaxLength(1000);

                    e.HasOne(x => x.Branch)
                     .WithMany(x => x.StockTransactions)
                     .HasForeignKey(x => x.BranchId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasOne(x => x.Product)
                     .WithMany()
                     .HasForeignKey(x => x.ProductId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasOne(x => x.Order)
                     .WithMany()
                     .HasForeignKey(x => x.OrderId)
                     .OnDelete(DeleteBehavior.SetNull);

                    e.HasIndex(x => new { x.BranchId, x.ProductId, x.CreatedAt });
                    e.HasIndex(x => new { x.BranchId, x.CreatedAt });
                    e.HasIndex(x => x.OrderId);
                });

                // ================================================================
                // InventoryTransfer
                // ================================================================
                b.Entity<InventoryTransfer>(e =>
                {
                    e.ToTable("InventoryTransfers", "dbo");
                    e.HasKey(x => x.InventoryTransferId);
                    e.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
                    e.Property(x => x.Note).HasMaxLength(1000);

                    e.HasOne<Branch>()
                     .WithMany()
                     .HasForeignKey(x => x.FromBranchId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasOne<Branch>()
                     .WithMany()
                     .HasForeignKey(x => x.ToBranchId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasOne<Product>()
                     .WithMany()
                     .HasForeignKey(x => x.ProductId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasIndex(x => new { x.FromBranchId, x.CreatedAt });
                    e.HasIndex(x => new { x.ToBranchId, x.CreatedAt });
                });

                // ================================================================
                // Order
                // ================================================================
                b.Entity<Order>(e =>
                {
                    e.ToTable("Orders", "dbo");
                    e.HasKey(x => x.OrderId);

                    e.Property(x => x.OrderNumber).HasMaxLength(50).IsRequired();
                    e.Property(x => x.CustomerName).HasMaxLength(200);
                    e.Property(x => x.CustomerPhone).HasMaxLength(50);
                    e.Property(x => x.SubTotal).HasColumnType("decimal(18,4)");
                    e.Property(x => x.Discount).HasColumnType("decimal(18,4)");
                    e.Property(x => x.Tax).HasColumnType("decimal(18,4)");
                    e.Property(x => x.Total).HasColumnType("decimal(18,4)");
                    e.Property(x => x.CostTotal).HasColumnType("decimal(18,4)");
                    e.Ignore(x => x.Profit);

                    e.HasOne(x => x.Branch)
                     .WithMany(x => x.Orders)
                     .HasForeignKey(x => x.BranchId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasIndex(x => new { x.BranchId, x.CreatedAt });
                    e.HasIndex(x => new { x.BranchId, x.Status });
                    e.HasIndex(x => x.OrderNumber).IsUnique();
                });

                // ================================================================
                // OrderItem
                // ================================================================
                b.Entity<OrderItem>(e =>
                {
                    e.ToTable("OrderItems", "dbo");
                    e.HasKey(x => x.OrderItemId);
                    e.Property(x => x.ProductTitleSnapshot).HasMaxLength(200).IsRequired();
                    e.Property(x => x.UnitPrice).HasColumnType("decimal(18,4)");
                    e.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
                    e.Property(x => x.Discount).HasColumnType("decimal(18,4)");
                    e.Property(x => x.LineTotal).HasColumnType("decimal(18,4)");
                    e.Property(x => x.UnitCostSnapshot).HasColumnType("decimal(18,4)");
                    e.Property(x => x.LineCost).HasColumnType("decimal(18,4)");
                    e.Property(x => x.Note).HasMaxLength(500);

                    e.HasOne(x => x.Order)
                     .WithMany(x => x.Items)
                     .HasForeignKey(x => x.OrderId)
                     .OnDelete(DeleteBehavior.Cascade);

                    e.HasOne(x => x.Product)
                     .WithMany()
                     .HasForeignKey(x => x.ProductId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasIndex(x => x.OrderId);
                    e.HasIndex(x => x.ProductId);
                });

                // ================================================================
                // Payment
                // ================================================================
                b.Entity<Payment>(e =>
                {
                    e.ToTable("Payments", "dbo");
                    e.HasKey(x => x.PaymentId);
                    e.Property(x => x.Amount).HasColumnType("decimal(18,4)");
                    e.Property(x => x.Reference).HasMaxLength(100);

                    e.HasOne(x => x.Order)
                     .WithMany(x => x.Payments)
                     .HasForeignKey(x => x.OrderId)
                     .OnDelete(DeleteBehavior.Cascade);

                    e.HasIndex(x => x.OrderId);
                    e.HasIndex(x => new { x.PaidAt });
                });

                // ================================================================
                // LedgerCategory (dynamic)
                // ================================================================
                b.Entity<LedgerCategory>(e =>
                {
                    e.ToTable("LedgerCategories", "dbo");
                    e.HasKey(x => x.LedgerCategoryId);
                    e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                    e.Property(x => x.Code).HasMaxLength(50);
                    e.Property(x => x.Description).HasMaxLength(1000);

                    e.HasOne(x => x.Parent)
                     .WithMany(x => x.Children)
                     .HasForeignKey(x => x.ParentId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasIndex(x => x.Code)
                     .IsUnique()
                     .HasFilter("[Code] IS NOT NULL AND [IsDeleted] = 0");

                    e.HasIndex(x => x.Title);

                    // Seed system categories used by the order flow
                    e.HasData(
                        new LedgerCategory { LedgerCategoryId = 1, Title = "فروش", Code = "SALE", Direction = LedgerDirection.Income, IsSystem = true, IsActive = true, SortOrder = 1 },
                        new LedgerCategory { LedgerCategoryId = 2, Title = "استرداد", Code = "REFUND", Direction = LedgerDirection.Expense, IsSystem = true, IsActive = true, SortOrder = 2 },
                        new LedgerCategory { LedgerCategoryId = 3, Title = "خرید مواد اولیه", Code = "PUR-ING", Direction = LedgerDirection.Expense, IsActive = true, SortOrder = 10 },
                        new LedgerCategory { LedgerCategoryId = 4, Title = "حقوق پرسنل", Code = "SALARY", Direction = LedgerDirection.Expense, IsActive = true, SortOrder = 11 },
                        new LedgerCategory { LedgerCategoryId = 5, Title = "اجاره", Code = "RENT", Direction = LedgerDirection.Expense, IsActive = true, SortOrder = 12 },
                        new LedgerCategory { LedgerCategoryId = 6, Title = "قبوض", Code = "UTIL", Direction = LedgerDirection.Expense, IsActive = true, SortOrder = 13 },
                        new LedgerCategory { LedgerCategoryId = 7, Title = "تجهیزات", Code = "EQUIP", Direction = LedgerDirection.Expense, IsActive = true, SortOrder = 14 },
                        new LedgerCategory { LedgerCategoryId = 8, Title = "بازاریابی", Code = "MKT", Direction = LedgerDirection.Expense, IsActive = true, SortOrder = 15 },
                        new LedgerCategory { LedgerCategoryId = 9, Title = "سایر", Code = "OTHER", Direction = LedgerDirection.Both, IsActive = true, SortOrder = 99 }
                    );
                });

                // ================================================================
                // CashLedgerEntry
                // ================================================================
                b.Entity<CashLedgerEntry>(e =>
                {
                    e.ToTable("CashLedgerEntries", "dbo");
                    e.HasKey(x => x.CashLedgerEntryId);
                    e.Property(x => x.Amount).HasColumnType("decimal(18,4)");
                    e.Property(x => x.Description).HasMaxLength(1000);

                    e.HasOne(x => x.Branch)
                     .WithMany(x => x.CashLedger)
                     .HasForeignKey(x => x.BranchId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasOne(x => x.Category)
                     .WithMany(x => x.Entries)
                     .HasForeignKey(x => x.LedgerCategoryId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasOne<Order>()
                     .WithMany()
                     .HasForeignKey(x => x.OrderId)
                     .OnDelete(DeleteBehavior.SetNull);

                    e.HasOne<StockTransaction>()
                     .WithMany()
                     .HasForeignKey(x => x.StockTransactionId)
                     .OnDelete(DeleteBehavior.SetNull);

                    e.HasIndex(x => new { x.BranchId, x.OccurredAt });
                    e.HasIndex(x => new { x.BranchId, x.LedgerCategoryId, x.OccurredAt });
                    e.HasIndex(x => x.OrderId);
                });

                // ================================================================
                // AppUser + UserBranch
                // ================================================================
                b.Entity<AppUser>(e =>
                {
                    e.Property(x => x.FullName).HasMaxLength(200);
                    e.HasMany(x => x.Branches)
                     .WithOne(x => x.User)
                     .HasForeignKey(x => x.UserId)
                     .OnDelete(DeleteBehavior.Cascade);
                });

                b.Entity<UserBranch>(e =>
                {
                    e.ToTable("UserBranches", "dbo");
                    e.HasKey(x => x.UserBranchId);

                    e.HasOne(x => x.User)
                     .WithMany(x => x.Branches)
                     .HasForeignKey(x => x.UserId)
                     .OnDelete(DeleteBehavior.Cascade);

                    e.HasOne(x => x.Branch)
                     .WithMany()
                     .HasForeignKey(x => x.BranchId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasIndex(x => new { x.UserId, x.BranchId }).IsUnique();
                });

                // ================================================================
                // UserActionLog
                // ================================================================
                b.Entity<UserActionLog>(e =>
                {
                    e.ToTable("UserActionLogs", "dbo");
                    e.HasKey(x => x.UserActionLogId);
                    e.Property(x => x.Action).HasMaxLength(100).IsRequired();
                    e.Property(x => x.EntityName).HasMaxLength(100);
                    e.Property(x => x.EntityId).HasMaxLength(100);
                    e.Property(x => x.Details).HasMaxLength(4000);

                    e.HasIndex(x => new { x.UserId, x.OccurredAt });
                    e.HasIndex(x => new { x.BranchId, x.OccurredAt });
                });

                // ================================================================
                // Seed roles
                // ================================================================
                b.Entity<IdentityRole>().HasData(
                    new IdentityRole { Id = "role-owner", Name = "Owner", NormalizedName = "OWNER" },
                    new IdentityRole { Id = "role-manager", Name = "BranchManager", NormalizedName = "BRANCHMANAGER" },
                    new IdentityRole { Id = "role-cashier", Name = "Cashier", NormalizedName = "CASHIER" },
                    new IdentityRole { Id = "role-barista", Name = "Barista", NormalizedName = "BARISTA" },
                    new IdentityRole { Id = "role-account", Name = "Accountant", NormalizedName = "ACCOUNTANT" },
                    new IdentityRole { Id = "role-menu", Name = "MenuAdmin", NormalizedName = "MENUADMIN" }
                );
            }

            // ================================================================
            // Auto-fill CreatedAt / CreatedByUserId
            // ================================================================
            public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                var now = DateTime.UtcNow;
                var userId = _currentUser?.UserId;

                foreach (var entry in ChangeTracker.Entries())
                {
                    if (entry.State == EntityState.Added)
                    {
                        if (entry.Entity is IHasCreatedAt ca && ca.CreatedAt == default)
                            ca.CreatedAt = now;

                        if (entry.Entity is IHasCreatedBy cb && string.IsNullOrEmpty(cb.CreatedByUserId))
                            cb.CreatedByUserId = userId;
                    }
                }

                return base.SaveChangesAsync(cancellationToken);
            }

            public override int SaveChanges()
            {
                var now = DateTime.UtcNow;
                var userId = _currentUser?.UserId;

                foreach (var entry in ChangeTracker.Entries())
                {
                    if (entry.State == EntityState.Added)
                    {
                        if (entry.Entity is IHasCreatedAt ca && ca.CreatedAt == default)
                            ca.CreatedAt = now;

                        if (entry.Entity is IHasCreatedBy cb && string.IsNullOrEmpty(cb.CreatedByUserId))
                            cb.CreatedByUserId = userId;
                    }
                }

                return base.SaveChanges();
            }
        }
}
