using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AquariumApi.Core
{
    public interface IPhotoManager
    {
        void DeletePhoto(string path);
        void Expand(string path);
        AquariumPhoto StoreAquariumPhoto(int aquariumId, Stream file);
        FishPhoto StoreFishPhoto(int fishId, Stream file);
    }
    public class PhotoManager: IPhotoManager
    {
        private IConfiguration _config;

        public PhotoManager(IConfiguration config)
        {
            _config = config;
        }
        public AquariumPhoto StoreAquariumPhoto(int aquariumId,Stream file)
        {
            var now = DateTime.Now;
            var path = $"{_config["Path"]}/aquarium/{aquariumId}/" + now.Millisecond + ".jpg";
            StorePhoto(path, file);
            return new AquariumPhoto()
            {
                Date = now,
                AquariumId = aquariumId,
                Filepath = path
            };
        }
        public FishPhoto StoreFishPhoto(int fishId,Stream file)
        {
            var now = DateTime.Now;
            var path = $"{_config["Path"]}/fish/{fishId}/" + now.Millisecond + ".jpg";
            StorePhoto(path, file);
            return new FishPhoto()
            {
                Date = now,
                FishId = fishId,
                Filepath = path
            };
        }
        private void StorePhoto(string path, Stream file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (Stream output = File.OpenWrite(path))
                file.CopyTo(output);
            if (!File.Exists(path))
                throw new Exception("Could not save photo from request");
            Expand(path);
        }
        public void DeletePhoto(string path)
        {
            var basePath = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);
            var sizes = _config.GetSection("Photos:Sizes");
            foreach (var pair in sizes.AsEnumerable())
            {
                var destination = basePath + "/" + pair.Key + "/" + filename;
                if (File.Exists(destination))
                    File.Delete(destination);
            }
        }
        public void Expand(string path)
        {
            var sizes = _config.GetSection("Photos:Sizes");
            foreach(var pair in sizes.AsEnumerable())
            {
                var scale = Convert.ToDecimal(pair.Value);
                var destination = Path.GetDirectoryName(path) + "/" + pair.Key;
                using (var img = Image.FromFile(path))
                {
                    Directory.CreateDirectory(destination);
                    string filepath = destination + Path.GetFileName(path);
                    var w = Convert.ToInt16(img.Width * scale);
                    var h = Convert.ToInt16(img.Height * scale);
                    var downsized = ResizeImage(img, w, h);
                    downsized.Save(filepath, ImageFormat.Jpeg);
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
