using Lumen.Models.Enums;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Lumen.Models
{
    public class Media : BaseEntity
    {
        public long MediaId { get; set; }
        public long ObjectId { get; set; }
        public string PictureUrl { get; set; }
        public MediaTypes Type { get; set; }
        public int? Priority { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? Title { get; set; }
    }
    public class OutPutSaveImage
    {
        public string ImageName { get; set; }
        public bool IsSuccess { get; set; }
    }
    public class FileData
    {
        public string Base64 { get; set; }
        public int Priority { get; set; }
    }
    public class MediaInfo
    {
        public MediaTypes Type { get; set; }
        public string FilePath { get; set; }
    }
}
