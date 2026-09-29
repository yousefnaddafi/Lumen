using Lumen.InfraStructure;
using Lumen.Models;

namespace Lumen.Services.RecipeServices
{
    public class RecipeService : IRecipeService
    {
        private readonly IProjectEFRepository<ProductRecipe> _repo;
        private readonly ProjectDBContext _db;

        public RecipeService(IProjectEFRepository<ProductRecipe> repo, ProjectDBContext db)
        {
            _repo = repo;
            _db = db;
        }

        public List<ProductRecipe> GetForProduct(long parentProductId) =>
            _repo.GetQuery().Where(r => r.ParentProductId == parentProductId).ToList();

        public Dictionary<long, List<ProductRecipe>> GetForProducts(IEnumerable<long> parentProductIds)
        {
            var ids = parentProductIds.Distinct().ToList();
            return _repo.GetQuery()
                .Where(r => ids.Contains(r.ParentProductId))
                .ToList()
                .GroupBy(r => r.ParentProductId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void SaveRecipe(long parentProductId, List<ProductRecipe> rows)
        {
            using var tx = _db.Database.BeginTransaction();
            try
            {
                // Delete all existing rows for this product, then insert the new set
                var existing = _repo.GetQuery().Where(r => r.ParentProductId == parentProductId).ToList();
                foreach (var e in existing)
                {
                    e.IsDeleted = true;
                    _repo.VoidUpdate(e);
                }

                foreach (var r in rows)
                {
                    r.ParentProductId = parentProductId;
                    r.IsDeleted = false;
                    if (r.ProductRecipeId > 0)
                    {
                        // can't reuse a soft-deleted row ID meaningfully; treat as new
                        r.ProductRecipeId = 0;
                    }
                    _repo.Insert(r);
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
