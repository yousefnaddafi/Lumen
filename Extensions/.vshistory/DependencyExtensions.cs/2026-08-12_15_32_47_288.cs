using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Services.MediaServices;
using Lumen.Services.ProductCatgoryServices;
using Lumen.Services.ProductServices;
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
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IMediaService, MediaService>();
            //services.AddTransient<ISMSService, SMSService>();

            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IProductCategoryService, ProductCategoryService>();

            services.AddTransient(typeof(IGenerateJwtService), typeof(GenerateJwtService));
            services.AddTransient(typeof(IConfirmationCodeSetting), typeof(ConfirmationCodeSetting));
            services.AddTransient(typeof(IEncryptService), typeof(EncryptService));
            services.AddTransient(typeof(IDecryptService), typeof(DecryptService));
        }
    }
}
