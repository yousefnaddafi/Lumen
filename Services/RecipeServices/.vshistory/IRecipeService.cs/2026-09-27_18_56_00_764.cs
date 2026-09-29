using Lumen.Models;

namespace Lumen.Services.RecipeServices
{
    public interface IRecipeService
    {
        List<ProductRecipe> GetForProduct(long parentProductId);
        void SaveRecipe(long parentProductId, List<ProductRecipe> rows);
        Dictionary<long, List<ProductRecipe>> GetForProducts(IEnumerable<long> parentProductIds);
    }
}
