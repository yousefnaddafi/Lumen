using Lumen.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

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


        public ProjectDBContext(DbContextOptions<ProjectDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Media>().ToTable("Medias", "dbo").HasKey(z => z.MediaId);
            modelbuilder.Entity<Product>().ToTable("Products", "dbo").HasKey(z => z.ProductId);
            modelbuilder.Entity<ProductCategory>().ToTable("ProductCategories", "dbo").HasKey(z => z.ProductCategoryId);

            modelbuilder.Entity<Product>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
        }
    }
}
