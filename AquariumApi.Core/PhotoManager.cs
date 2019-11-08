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
        Dictionary<string, string> GetImageSizes(string path);
        FishPhoto AddFishPhoto(int fishId, byte[] buffer);
        FishPhoto AddFishPhoto(int fishId, Stream stream);
        AquariumPhoto AddAquariumPhoto(int aquariumId, byte[] buffer);
        void DeletePhoto(int photoId);
        byte[] GetPhoto(int photoId);
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
        public AquariumPhoto AddAquariumPhoto(int aquariumId, byte[] buffer)
        {

            var content = StorePhoto(buffer).Result;
            if (!content.Exists)
            {
                //todo delete reference
                throw new Exception("Could not store photo in storage location");
            }
            return _aquariumDao.AddAquariumPhoto(new AquariumPhoto
            {
                AquariumId = aquariumId,
                PhotoId = content.Id
            });
        }
        public FishPhoto AddFishPhoto(int fishId, byte[] buffer)
        {

            var content = StorePhoto(buffer).Result;
            if (!content.Exists)
            {
                //todo delete reference
                throw new Exception("Could not store photo in storage location");
            }
            return _aquariumDao.AddFishPhoto(new FishPhoto
            {
                FishId = fishId,
                PhotoId = content.Id
            });
        }
        public FishPhoto AddFishPhoto(int fishId, Stream stream)
        {

            var content = StorePhoto(stream).Result;
            if (!content.Exists)
            {
                //todo delete reference
                throw new Exception("Could not store photo in storage location");
            }
            return _aquariumDao.AddFishPhoto(new FishPhoto
            {
                FishId = fishId,
                PhotoId = content.Id
            });
        }
        public void DeletePhoto(int photoId)
        {
            var photo = _aquariumDao.GetPhoto(photoId);
            _aquariumDao.DeletePhoto(photoId);
            DeletePhotoByPath(photo.Filepath);
        }
        public byte[] GetPhoto(int photoId)
        {
            var photo = _aquariumDao.GetPhoto(photoId);
            if (!photo.Exists)
                throw new Exception("Photo does not exist");
            try
            {
                return _azureService.GetFileFromStorage(photo.Filepath).Result;
            }
            catch
            {
                photo.Exists = false;
                _aquariumDao.UpdatePhotoReference(photo);
                throw new Exception("Photo does not exist");
            }
        }

        private async Task<PhotoContent> StorePhoto(byte[] buffer)
        {
            var now = DateTime.Now.ToUniversalTime();
            var path = $"{_config["Photos:Path"]}/" + now.Ticks + ".jpg";

            var content = _aquariumDao.CreatePhotoReference();
            content.Date = now;
            content.Filepath = path;
            content.Exists = true;

            await _azureService.UploadFileToStorage(buffer, path);
            if(Convert.ToBoolean(_config["Photos:ExpandSizes"]))
            {
                ExpandPhotoSizes(buffer, path);
            }
            if(_azureService.Exists(path))
                _aquariumDao.UpdatePhotoReference(content);
            return content;
        }
        private async Task<PhotoContent> StorePhoto(Stream stream)
        {
            var now = DateTime.Now.ToUniversalTime();
            var path = $"{_config["Photos:Path"]}/" + now.Ticks + ".jpg";

            var content = _aquariumDao.CreatePhotoReference();
            content.Date = now;
            content.Filepath = path;
            content.Exists = true;

            await _azureService.UploadFileToStorage(stream, path);
            if (Convert.ToBoolean(_config["Photos:ExpandSizes"]))
            {
                var p = await _azureService.GetFileFromStorage(path);
                ExpandPhotoSizes(p, path);
            }
            if (_azureService.Exists(path))
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
                        await _azureService.UploadFileToStorage(newImage.ToArray(), filepath);
                    }
                }

            }
        }

        private void DeletePhotoByPath(string path)
        {
            var basePath = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);
            var sizes = _config.GetSection("Photos:Sizes").Get<List<decimal>>();
            foreach (var s in sizes)
            {
                var destination = Path.GetDirectoryName(path) + "/x" + s + "/" + filename;
                _azureService.DeleteFileFromStorage(destination);
            }
            _azureService.DeleteFileFromStorage(path);
        }

        
        //deprecated, needs to be updated to support azure
        public Dictionary<string, string> GetImageSizes(string path)
        {
            var result = new Dictionary<string, string>();
            var filename = Path.GetFileName(path);
            var sizes = _config.GetSection("Photos:Sizes").Get<List<decimal>>();
            foreach (var s in sizes)
            {
                var destination = Path.GetDirectoryName(path) + "/x" + Convert.ToInt16(s * 100) + "/" + filename;
                if (File.Exists(destination))
                    result.Add(s.ToString(), destination);
            }
            return result;
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
        private void StorePhotoLocally(Stream file, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (Stream output = File.OpenWrite(path))
                file.CopyTo(output);
            if (!File.Exists(path))
                throw new Exception("Could not save photo from request");
        }
        public void DeletePhotoLocally(string path)
        {
            var basePath = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);
            var sizes = _config.GetSection("Photos:Sizes").Get<List<decimal>>();
            foreach (var s in sizes)
            {
                var destination = Path.GetDirectoryName(path) + "/x" + Convert.ToInt16(s * 100) + "/" + filename;
                if (File.Exists(destination))
                    File.Delete(destination);
            }
            if (File.Exists(path))
                File.Delete(path);
        }

    }
}
