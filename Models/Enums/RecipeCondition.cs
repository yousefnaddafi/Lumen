namespace Lumen.Models.Enums
{
    public enum RecipeCondition
    {
        Always = 1,        // consumed on every sale of the parent
        TakeawayOnly = 2,  // only when order channel is Takeaway or Delivery
        DineInOnly = 3     // only when order channel is InStore
    }
}
