using Microsoft.Extensions.Configuration;
using MMALSharp;
using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IPhotoManager
    {
        Task<string> TakePhoto();
    }
    public class PhotoManager : IPhotoManager
    {
        private readonly IConfiguration _config;
        public PhotoManager(IConfiguration config)
        {
            _config = config;
        }
        public async Task<string> TakePhoto()
        {
            MMALCameraConfig.Debug = true;
            var path = "temp";
            var ext = "jpg";
            MMALCamera cam = MMALCamera.Instance;
            using (var imgCaptureHandler = new ImageStreamCaptureHandler(path, ext))
            {
                cam.ForceStop(cam.Camera.StillPort);
                await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
                cam.Cleanup();
                return imgCaptureHandler.GetFilepath();
            }
            //return $"{path}.{ext}";
        }
    }
}
