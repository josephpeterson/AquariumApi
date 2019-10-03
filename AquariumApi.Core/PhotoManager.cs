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
        void DeletePhoto(string path);
        Dictionary<string, string> GetImageSizes(string path);
        AquariumPhoto StoreAquariumPhoto(int aquariumId, byte[] buffer);
        FishPhoto StoreFishPhoto(int fishId,int aquariumId,Stream file);
        byte[] GetPhoto(string path);
    }
    public class PhotoManager: IPhotoManager
    {
        private readonly IAzureService _azureService;
        private IConfiguration _config;

        public PhotoManager(IConfiguration config,IAzureService azureService)
        {
            _azureService = azureService;
            _config = config;
        }
        public AquariumPhoto StoreAquariumPhoto(int aquariumId,byte[] buffer)
        {
            var now = DateTime.Now;
            var path = $"{_config["Photos:Path"]}/aquarium/{aquariumId}/" + now.Ticks + ".jpg";

            var exists = StorePhoto(path, buffer).Result;
            if (!exists)
                throw new FileNotFoundException();

            return new AquariumPhoto()
            {
                Date = now,
                AquariumId = aquariumId,
                Filepath = path
            };
        }
        public FishPhoto StoreFishPhoto(int fishId, int aquariumId, Stream file)
        {
            var now = DateTime.Now;
            var path = $"{_config["Photos:Path"]}/fish/{fishId}/" + now.Ticks + ".jpg";
            //StorePhoto(path, file);
            return new FishPhoto()
            {
                Date = now,
                FishId = fishId,
                Filepath = path,
                AquariumId = aquariumId
            };
        }
        public async Task<bool> StorePhoto(string path, byte[] buffer)
        {
            await _azureService.UploadFileToStorage(buffer, path);
            //StorePhotoLocally(file, path);
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
            return  _azureService.Exists(path);
        }

        public void DeletePhoto(string path)
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

        public byte[] GetPhoto(string path)
        {
            return _azureService.GetFileFromStorage(path).Result;
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
