using Lumen.Models;
using Lumen.Models.Enums;

namespace Lumen.Services.MediaServices
{
    public interface IMediaService
    {
        Task<Dictionary<string, string>> Upload(List<IFormFile> files, int objectId, MediaTypes type);
        int Create(Media media);
        Task<List<Media>> GetByObjectId(int objectId, MediaTypes type);
        Task DeleteByMediaId(int mediaId);
        Task DeleteByObjectId(int objectId, MediaTypes type);
        OutPutSaveImage SaveImage(string base64, string path);
    }
}
