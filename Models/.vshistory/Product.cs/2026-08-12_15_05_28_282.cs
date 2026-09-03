using System.ComponentModel.DataAnnotations.Schema;

namespace Lumen.Models
{
    public class Product : BaseEntity
    {
        public long ProductId { get; set; }
        public string Title { get; set; }
        public bool IsAvailable { get; set; }
        public long ProductCategoryId { get; set; }
        public DateTime CreateDate { get; set; }
        public long EcoPrice { get; set; }
        public long? BusinessPrice { get; set; }


        [NotMapped]
        public string? CategoryName { get; set; }
    }
}
