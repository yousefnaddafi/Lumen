using Lumen.Models;
using Microsoft.EntityFrameworkCore;

namespace Lumen.InfraStructure
{
    public class ProjectDBContext : DbContext
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(
        //        "yourConnectionString",
        //        options => options.CommandTimeout(120) // 2 minutes
        //    );
        //}
        public ProjectDBContext()
        {

        }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        // ----- Branches -----
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<BranchInventory> BranchInventories { get; set; } = null!;
        public DbSet<StockTransaction> StockTransactions { get; set; } = null!;
        //public DbSet<InventoryTransfer> InventoryTransfers { get; set; } = null!;

        // ----- Orders -----
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;

        // ----- Accounting -----
        public DbSet<LedgerCategory> LedgerCategories { get; set; } = null!;
        public DbSet<CashLedgerEntry> CashLedgerEntries { get; set; } = null!;

        // ----- Users -----
        public DbSet<AppUser> AppUsers { get; set; } = null!;
        public DbSet<UserBranch> UserBranches { get; set; } = null!;
        public DbSet<UserActionLog> UserActionLogs { get; set; } = null!;



        public ProjectDBContext(DbContextOptions<ProjectDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Media>().ToTable("Medias", "dbo").HasKey(z => z.MediaId);
            modelbuilder.Entity<Product>().ToTable("Products", "dbo").HasKey(z => z.ProductId);
            modelbuilder.Entity<ProductCategory>().ToTable("ProductCategories", "dbo").HasKey(z => z.ProductCategoryId);

            modelbuilder.Entity<Product>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Media>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
        }
    }
}
