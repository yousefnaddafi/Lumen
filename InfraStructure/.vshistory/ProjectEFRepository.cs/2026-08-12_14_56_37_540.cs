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
        public DbSet<User> Users { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageRecipient> MessageRecipients { get; set; }
        public DbSet<RanginNegaranERP.Models.Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Utility> Utilities { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<WarehouseItem> WarehouseItems { get; set; }


        public ProjectDBContext(DbContextOptions<ProjectDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<User>().ToTable("Users", "dbo").HasKey(z => z.UserId);
            modelbuilder.Entity<UserType>().ToTable("UserTypes", "dbo").HasKey(z => z.UserTypeId);
            modelbuilder.Entity<Media>().ToTable("Medias", "dbo").HasKey(z => z.MediaId);
            modelbuilder.Entity<Message>().ToTable("Messages", "dbo").HasKey(z => z.MessageId);
            modelbuilder.Entity<MessageRecipient>().ToTable("MessageRecipients", "dbo").HasKey(z => z.MessageRecipientId);
            modelbuilder.Entity<RanginNegaranERP.Models.Group>().ToTable("Groups", "dbo").HasKey(z => z.GroupId);
            modelbuilder.Entity<UserGroup>().ToTable("UserGroups", "dbo").HasKey(z => z.UserGroupId);
            modelbuilder.Entity<Report>().ToTable("Reports", "dbo").HasKey(z => z.ReportId);
            modelbuilder.Entity<Shop>().ToTable("Shops", "dbo").HasKey(z => z.ShopId);
            modelbuilder.Entity<Utility>().ToTable("Utilities", "dbo").HasKey(z => z.UtilityId);
            modelbuilder.Entity<Product>().ToTable("Products", "dbo").HasKey(z => z.ProductId);
            modelbuilder.Entity<ProductCategory>().ToTable("ProductCategories", "dbo").HasKey(z => z.ProductCategoryId);
            modelbuilder.Entity<Brand>().ToTable("Brands", "dbo").HasKey(z => z.BrandId);
            modelbuilder.Entity<Order>().ToTable("Orders", "dbo").HasKey(z => z.OrderId);
            modelbuilder.Entity<OrderItem>().ToTable("OrderItems", "dbo").HasKey(z => z.OrderItemId);
            modelbuilder.Entity<ProductPrice>().ToTable("ProductPrices", "dbo").HasKey(z => z.ProductPriceId);
            modelbuilder.Entity<WarehouseItem>().ToTable("WarehouseItems", "dbo").HasKey(z => z.WarehouseItemId);

            modelbuilder.Entity<User>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Report>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Shop>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Utility>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Order>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Brand>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Product>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<ProductPrice>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
        }
    }
}
