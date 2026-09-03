namespace Lumen.Models
{
    public class EntityUrls
    {
        public static string MediaBaseUrl = "wwwroot/Media/Gallery/";

        public static string Product { get; set; } = MediaBaseUrl + "Product";
        public static string ProductCategory { get; set; } = MediaBaseUrl + "ProductCategory";
    }
}
