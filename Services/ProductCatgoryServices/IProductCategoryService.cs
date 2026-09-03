using Lumen.Models;

namespace Lumen.Services.ProductCatgoryServices
{
    public interface IProductCategoryService
    {
        long Create(ProductCategory model);
        Task<ProductCategory> Update(ProductCategory model);
        Task<bool> Delete(long productCategoryId);
        List<ProductCategory> GetAll(int pageNumber, int count, string? searchCommand);
        ProductCategory Get(long productCategoryId);
    }
}
