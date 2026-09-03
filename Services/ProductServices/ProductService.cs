using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Models.Enums;
using Lumen.Services.MediaServices;
using Microsoft.EntityFrameworkCore;

namespace Lumen.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IProjectEFRepository<ProductCategory> ProductCategoryRepository;
        private readonly IProjectEFRepository<Product> productRepository;
        private readonly IProjectEFRepository<Media> _mediaRepository;
        private readonly IMediaService _mediaService;
        public ProductService(IMediaService mediaService,IProjectEFRepository<Media> mediaRepository,
            IProjectEFRepository<ProductCategory> ProductCategoryRepository,
            IProjectEFRepository<Product> productRepository)
        {
            this.ProductCategoryRepository = ProductCategoryRepository;
            this.productRepository = productRepository;
            _mediaRepository = mediaRepository;
            _mediaService = mediaService;
        }
        public async Task<long> Create(Product model)
        {
            var dbProduct = productRepository.GetQuery().FirstOrDefault(z => z.Title.Trim() == model.Title.Trim());
            if (dbProduct != null)
            {
                return 0;
            }
            var creation = productRepository.Insert(model);
            if (!string.IsNullOrEmpty(model.Image))
            {
                var outPut = _mediaService.SaveImage(model.Image, EntityUrls.Product);
                if (outPut.IsSuccess)
                {
                    Media dbMedia = new Media()
                    {
                        IsDeleted = false,
                        ObjectId = creation.ProductId,
                        PictureUrl = outPut.ImageName,
                        Type = MediaTypes.Product,
                        MediaId = 0,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    };
                    
                        await _mediaRepository.InsertAsync(dbMedia);
                        model.Image = outPut.ImageName;
                }
            }
            return creation.ProductId;
        }
        public async Task<Product> Update(Product model)
        {
            var dbLastPic = _mediaRepository.GetQuery()
                .FirstOrDefault(z => z.ObjectId == model.ProductId && z.Type == MediaTypes.Product);

            if (!string.IsNullOrEmpty(model.Image))
            {
                var outPut = _mediaService.SaveImage(model.Image, EntityUrls.Product);
                if (outPut.IsSuccess)
                {
                    Media dbMedia = new Media()
                    {
                        IsDeleted = false,
                        ObjectId = model.ProductId,
                        PictureUrl = outPut.ImageName,
                        Type = MediaTypes.Product,
                        MediaId = 0,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    };

                    if (dbLastPic != null)
                    {
                        dbLastPic.PictureUrl = outPut.ImageName;
                        dbLastPic.UpdateDate = DateTime.Now;
                        await _mediaRepository.Update(dbLastPic);
                        model.Image = outPut.ImageName;
                    }
                    else
                    {
                        await _mediaRepository.InsertAsync(dbMedia);
                        model.Image= outPut.ImageName;
                    }
                }
            }

            var dbProduct = await productRepository.Update(model);
            return dbProduct;
        }
        public async Task<bool> Delete(long productId)
        {
            try
            {
                var dbProduct = productRepository.GetQuery().FirstOrDefault(z => z.ProductId == productId);
                if (dbProduct == null)
                {
                    return false;
                }
                else
                {
                    await productRepository.Delete(dbProduct);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<Product> GetAll( long? productCategoryId, bool? isAvailable)
        {
            var dbProducts = productRepository.GetQuery().ToList();

            
            if (productCategoryId != null && productCategoryId != 0)
            {
                dbProducts = dbProducts.Where(z => z.ProductCategoryId == productCategoryId).ToList();
            }
            
            if (isAvailable != null)
            {
                dbProducts = dbProducts.Where(z => z.IsAvailable == isAvailable).ToList();
            }

            var productsCount = dbProducts.Count();
            
            var dbProductCategories = ProductCategoryRepository.GetQuery().Where(z => dbProducts.Select(x => x.ProductCategoryId).Contains(z.ProductCategoryId)).ToList();
            foreach (var dbProduct in dbProducts)
            {
                var dbProfile = _mediaRepository.GetQuery().FirstOrDefault(z => z.ObjectId == dbProduct.ProductId && z.Type == MediaTypes.Product);
                if (dbProfile != null)
                {
                    dbProduct.Image= "media/gallery/Product/" + dbProfile.PictureUrl;
                }
                dbProduct.CategoryName = dbProductCategories.FirstOrDefault(z => z.ProductCategoryId == dbProduct.ProductCategoryId)?.Title;
            }

            return dbProducts;
        }
        public Product Get(long ProductId)
        {
            var dbProduct = productRepository.GetQuery().FirstOrDefault(z => z.ProductId == ProductId);
            dbProduct.CategoryName = ProductCategoryRepository.GetQuery().FirstOrDefault(z => z.ProductCategoryId == dbProduct.ProductCategoryId)?.Title;
            return dbProduct;
        }
    }
}
