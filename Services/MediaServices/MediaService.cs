using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Models.Enums;

namespace Lumen.Services.MediaServices
{
    public class MediaService : IMediaService
    {
        private readonly IProjectEFRepository<Media> mediaRepository;

        public MediaService(IProjectEFRepository<Media> mediaRepository)
        {
            this.mediaRepository = mediaRepository;
        }

        public async Task<Dictionary<string, string>> Upload(List<IFormFile> files, int objectId, MediaTypes type)
        {
            var data = new Dictionary<string, string>();

            var now = DateTime.Now;
            var folder = Path.Combine(now.Year.ToString(), now.Month.ToString(), now.Day.ToString());
            string currentDirectory = Directory.GetCurrentDirectory();

            if (!Directory.Exists(Path.Combine(currentDirectory, "wwwroot", folder)))
                Directory.CreateDirectory(Path.Combine(currentDirectory, "wwwroot", folder));

            foreach (var file in files)
            {
                string newFileName = $"{now.Hour}-{now.Minute}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var stream = new FileStream(Path.Combine(currentDirectory, "wwwroot", folder, newFileName), FileMode.Create))
                    await file.CopyToAsync(stream);
                var val = "/" + Path.Combine(folder, newFileName).Replace("\\", "/");
                data.Add(file.FileName, val);
                await mediaRepository.InsertAsync(new Media()
                {
                    IsDeleted = false,
                    ObjectId = objectId,
                    PictureUrl = val,
                    Type = type,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    MediaId = 0,
                    Priority = null
                });
            }

            return data;
        }

        public int Create(Media media)
        {
            try
            {
                mediaRepository.Insert(media);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }


        }

        public Task<List<Media>> GetByObjectId(int objectId, MediaTypes type)
                => Task.FromResult(mediaRepository.GetQuery().Where(z => z.ObjectId == objectId & z.Type == type).ToList());

        public async Task DeleteByMediaId(int mediaId)
        {
            var media = mediaRepository.GetQuery().FirstOrDefault(z => z.MediaId == mediaId);
            await mediaRepository.Delete(media);
        }
        public async Task DeleteByObjectId(int objectId, MediaTypes type)
        {
            var mediae = mediaRepository.GetQuery().Where(z => z.ObjectId == objectId & z.Type == type).ToList();

            foreach (var item in mediae)
               await mediaRepository.Delete(item);

        }
        public OutPutSaveImage SaveImage(string base64, string path)
        {
            OutPutSaveImage outPutSave = new OutPutSaveImage();
            try
            {
                if (base64.StartsWith("data:image/png;base64,"))
                {
                    base64 = base64.Split(",")[1];
                }
                byte[] contents = Convert.FromBase64String(base64);

                string fileName = System.Guid.NewGuid() + ".jpg";
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), path, fileName);

                System.IO.File.WriteAllBytes(imagePath, contents);
                outPutSave.ImageName = fileName;
                outPutSave.IsSuccess = true;
                return outPutSave;
            }
            catch (Exception ex)
            {
                outPutSave.IsSuccess = false;
                outPutSave.ImageName = "";
                return outPutSave;
            }

        }
    }
}
