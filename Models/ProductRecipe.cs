using Lumen.Models.Enums;

namespace Lumen.Models
{
    public class ProductRecipe : BaseEntity
    {
        public long ProductRecipeId { get; set; }

        public long ParentProductId { get; set; }      // the menu item
        public long ComponentProductId { get; set; }   // the raw material / packaging

        public decimal Quantity { get; set; }
        public RecipeCondition Condition { get; set; } = RecipeCondition.Always;

        public string? Note { get; set; }

        // Navigation
        public Product ParentProduct { get; set; } = null!;
        public Product ComponentProduct { get; set; } = null!;
    }
}
