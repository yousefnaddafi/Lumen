using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Services.BranchAvailabilityServices;
using Lumen.Services.BranchServices;
using Lumen.Services.CustomerServices;
using Lumen.Services.InventoryServices;
using Lumen.Services.MediaServices;
using Lumen.Services.OrderServices;
using Lumen.Services.ProductCatgoryServices;
using Lumen.Services.ProductServices;
using Lumen.Services.RecipeServices;
using Lumen.Services.UserServices;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace Lumen.Extensions
{
    public static class DependencyExtensions
    {
        public static void AddDependency(this IServiceCollection services)
        {
            AddRepositories(services);
            AddServices(services);
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IProjectEFRepository<Media>, ProjectEFRepository<Media>>();
            services.AddScoped<IProjectEFRepository<Product>, ProjectEFRepository<Product>>();
            services.AddScoped<IProjectEFRepository<ProductCategory>, ProjectEFRepository<ProductCategory>>();
            services.AddScoped<IProjectEFRepository<Branch>, ProjectEFRepository<Branch>>();
            services.AddScoped<IProjectEFRepository<Customer>, ProjectEFRepository<Customer>>();
            services.AddScoped<IProjectEFRepository<Order>, ProjectEFRepository<Order>>();
            services.AddScoped<IProjectEFRepository<ProductCategory>, ProjectEFRepository<ProductCategory>>();
            services.AddScoped<IProjectEFRepository<BranchInventory>, ProjectEFRepository<BranchInventory>>();
            services.AddScoped<IProjectEFRepository<LedgerCategory>, ProjectEFRepository<LedgerCategory>>();
            services.AddScoped<IProjectEFRepository<OrderItem>, ProjectEFRepository<OrderItem>>();
            services.AddScoped<IProjectEFRepository<Payment>, ProjectEFRepository<Payment>>();
            services.AddScoped<IProjectEFRepository<StockTransaction>, ProjectEFRepository<StockTransaction>>();
            services.AddScoped<IProjectEFRepository<UserActionLog>, ProjectEFRepository<UserActionLog>>();
            services.AddScoped<IProjectEFRepository<UserBranch>, ProjectEFRepository<UserBranch>>();
            services.AddScoped<IProjectEFRepository<CashLedgerEntry>, ProjectEFRepository<CashLedgerEntry>>();
            services.AddScoped<IProjectEFRepository<ProductRecipe>, ProjectEFRepository<ProductRecipe>>();
            services.AddScoped<IProjectEFRepository<BranchAvailabilityService>, ProjectEFRepository<BranchAvailabilityService>>();

        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IMediaService, MediaService>();
            //services.AddTransient<ISMSService, SMSService>();
            services.AddScoped<CurrentUserState>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserAdminService, UserAdminService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IStockTransactionService, StockTransactionService>();
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<IBranchAvailabilityService, BranchAvailabilityService>();
            //services.AddTransient(typeof(IGenerateJwtService), typeof(GenerateJwtService));
            //services.AddTransient(typeof(IConfirmationCodeSetting), typeof(ConfirmationCodeSetting));
            //services.AddTransient(typeof(IEncryptService), typeof(EncryptService));
            //services.AddTransient(typeof(IDecryptService), typeof(DecryptService));
        }
    }
}
