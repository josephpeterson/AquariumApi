using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IPhotoManager
    {
        void DeletePhoto(int photoId);
        Task<PhotoContent> StorePhoto(byte[] buffer);
    }
    public class PhotoManager: IPhotoManager
    {
        private readonly IAquariumDao _aquariumDao;
        private readonly IAzureService _azureService;
        private IConfiguration _config;

        public PhotoManager(IConfiguration config,IAzureService azureService,IAquariumDao aquariumDao)
        {
            _aquariumDao = aquariumDao;
            _azureService = azureService;
            _config = config;
        }
        public void DeletePhoto(int photoId)
        {
            var photo = _aquariumDao.GetPhoto(photoId);
            _aquariumDao.DeletePhoto(photoId);

            var path = photo.Filepath;
            var basePath = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);
            var sizes = _config.GetSection("Photos:Sizes").Get<List<decimal>>();
            foreach (var s in sizes)
            {
                var destination = Path.GetDirectoryName(path) + "/x" + s + "/" + filename;
                _azureService.DeleteFileFromStorageContainer(destination);
            }
            _azureService.DeleteFileFromStorageContainer(path);
        }
        public async Task<PhotoContent> StorePhoto(byte[] buffer)
        {
            var now = DateTime.Now.ToUniversalTime();
            var path = $"{_config["Photos:Path"]}/" + now.Ticks + ".jpg";

            var content = _aquariumDao.CreatePhotoReference();
            content.Date = now;
            content.Filepath = path;
            content.Exists = true;

            await _azureService.UploadFileToStorageContainer(buffer, path);
            if(Convert.ToBoolean(_config["Photos:ExpandSizes"]))
            {
                ExpandPhotoSizes(buffer, path);
            }
            if(_azureService.ExistsInStorageContainer(path))
                _aquariumDao.UpdatePhotoReference(content);
            return content;
        }
        private async void ExpandPhotoSizes(byte[] buffer,string path)
        {
            var sizes = _config.GetSection("Photos:Sizes").Get<List<decimal>>();
            foreach (var s in sizes)
            {
                var destination = Path.GetDirectoryName(path) + "/" + s + "/";

                using (var ms = new MemoryStream(buffer))
                {
                    using (var img = Image.FromStream(ms))
                    {
                        Directory.CreateDirectory(destination);
                        string filepath = destination + Path.GetFileName(path);
                        var w = Convert.ToInt16(img.Width * s);
                        var h = Convert.ToInt16(img.Height * s);
                        var downsized = ResizeImage(img, w, h);
                        var newImage = new MemoryStream();
                        downsized.Save(newImage, ImageFormat.Jpeg);
                        await _azureService.UploadFileToStorageContainer(newImage.ToArray(), filepath);
                    }
                }

            }
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            //destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
