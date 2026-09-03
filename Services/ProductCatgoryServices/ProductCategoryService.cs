using Lumen.InfraStructure;
using Lumen.Models;

namespace Lumen.Services.ProductCatgoryServices
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProjectEFRepository<ProductCategory> ProductCategoryRepository;
        private readonly IProjectEFRepository<Product> productRepository;
        public ProductCategoryService(IProjectEFRepository<ProductCategory> ProductCategoryRepository, IProjectEFRepository<Product> productRepository)
        {
            this.ProductCategoryRepository = ProductCategoryRepository;
            this.productRepository = productRepository;
        }
        public long Create(ProductCategory model)
        {
            var creation = ProductCategoryRepository.Insert(model);
            return creation.ProductCategoryId;
        }
        public async Task<ProductCategory> Update(ProductCategory model)
        {
            var dbProductCategory = await ProductCategoryRepository.Update(model);
            return dbProductCategory;
        }
        public async Task<bool> Delete(long ProductCategoryId)
        {
            try
            {
                var dbProductCategory = ProductCategoryRepository.GetQuery().FirstOrDefault(z => z.ProductCategoryId == ProductCategoryId);
                if (dbProductCategory == null)
                {
                    return false;
                }
                else
                {
                    var dbProducts = productRepository.GetQuery().Where(z => z.ProductCategoryId == dbProductCategory.ProductCategoryId).ToList();
                    if (dbProducts.Count > 0)
                    {
                        return false;
                    }
                    else
                    {
                        await ProductCategoryRepository.Delete(dbProductCategory);
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<ProductCategory> GetAll(int pageNumber, int count, string? searchCommand)
        {
            searchCommand = searchCommand ?? "";
            var dbProductCategorys = ProductCategoryRepository.GetQuery().Where(z => z.Title.Contains(searchCommand)).ToList();
            var ProductCategorysCount = dbProductCategorys.Count();
            if (pageNumber != 0 && count != 0)
            {
                dbProductCategorys = dbProductCategorys.OrderByDescending(z => z.ProductCategoryId).Skip((pageNumber - 1) * count).Take(count).ToList();
            }
            dbProductCategorys = dbProductCategorys.OrderBy(z => z.ProductCategoryId).ToList();
            //var result = new
            //{
            //    ProductCategorys = dbProductCategorys,
            //    Counts = ProductCategorysCount
            //};
            return dbProductCategorys;
        }
        public ProductCategory Get(long ProductCategoryId)
        {
            var dbProductCategory = ProductCategoryRepository.GetQuery().FirstOrDefault(z => z.ProductCategoryId == ProductCategoryId);
            return dbProductCategory;
        }
    }
}
