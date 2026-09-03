using Lumen.Models;

namespace Lumen.Services.ProductServices
{
    public interface IProductService
    {
        long Create(Product model);
        Task<Product> Update(Product model);
        Task<bool> Delete(long productId);
        List<Product> GetAll(int pageNumber, int count, string? searchCommand, long? brandId, long? productCategoryId, bool? VIPIncluded, bool? isAvailable);
        Product Get(long ProductId);
    }
}
