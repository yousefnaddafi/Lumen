using Lumen.InfraStructure;
using Lumen.Models;

namespace Lumen.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IProjectEFRepository<ProductCategory> ProductCategoryRepository;
        private readonly IProjectEFRepository<Product> productRepository;
        public ProductService(IProjectEFRepository<ProductCategory> ProductCategoryRepository, IProjectEFRepository<Product> productRepository)
        {
            this.ProductCategoryRepository = ProductCategoryRepository;
            this.productRepository = productRepository;
        }
        public long Create(Product model)
        {
            var dbProduct = productRepository.GetQuery().FirstOrDefault(z => z.Title.Trim() == model.Title.Trim());
            if (dbProduct != null)
            {
                return 0;
            }
            var creation = productRepository.Insert(model);
            return creation.ProductId;
        }
        public async Task<Product> Update(Product model)
        {
            var dbProduct = await productRepository.Update(model);
            return dbProduct;
        }
        public async Task<bool> Delete(long productId)
        {
            try
            {
                var dbProduct = productRepository.GetQuery().FirstOrDefault(z => z.ProductId == productId);
                if (dbProduct == null)
                {
                    return false;
                }
                else
                {
                    await productRepository.Delete(dbProduct);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<Product> GetAll(int pageNumber, int count, string? searchCommand, long? brandId, long? productCategoryId, bool? VIPIncluded, bool? isAvailable)
        {
            searchCommand = searchCommand ?? "";
            var dbProducts = productRepository.GetQuery().Where(z => z.Title.Contains(searchCommand)).ToList();

            if (brandId != null && brandId != 0)
            {
                dbProducts = dbProducts.Where(z => z.BrandId == brandId).ToList();
            }
            if (productCategoryId != null && productCategoryId != 0)
            {
                dbProducts = dbProducts.Where(z => z.ProductCategoryId == productCategoryId).ToList();
            }
            if (VIPIncluded != null)
            {
                dbProducts = dbProducts.Where(z => z.IsVIP == VIPIncluded).ToList();
            }
            if (isAvailable != null)
            {
                dbProducts = dbProducts.Where(z => z.IsAvailable == isAvailable).ToList();
            }

            var productsCount = dbProducts.Count();
            if (pageNumber != 0 && count != 0)
            {
                dbProducts = dbProducts.OrderByDescending(z => z.ProductId).Skip((pageNumber - 1) * count).Take(count).ToList();
            }
            var dbBrands = brandRepository.GetQuery().Where(z => dbProducts.Select(x => x.BrandId).Contains(z.BrandId)).ToList();
            var dbProductCategories = ProductCategoryRepository.GetQuery().Where(z => dbProducts.Select(x => x.ProductCategoryId).Contains(z.ProductCategoryId)).ToList();
            foreach (var dbProduct in dbProducts)
            {
                dbProduct.BrandName = dbBrands.FirstOrDefault(z => z.BrandId == dbProduct.BrandId)?.Title;
                dbProduct.CategoryName = dbProductCategories.FirstOrDefault(z => z.ProductCategoryId == dbProduct.ProductCategoryId)?.Title;
            }

            return dbProducts;
        }
        public Product Get(long ProductId)
        {
            var dbProduct = productRepository.GetQuery().FirstOrDefault(z => z.ProductId == ProductId);
            dbProduct.BrandName = brandRepository.GetQuery().FirstOrDefault(z => z.BrandId == dbProduct.BrandId)?.Title;
            dbProduct.CategoryName = ProductCategoryRepository.GetQuery().FirstOrDefault(z => z.ProductCategoryId == dbProduct.ProductCategoryId)?.Title;
            return dbProduct;
        }
    }
}
