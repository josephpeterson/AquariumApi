using AquariumApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMALSharp;
using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IPhotoManager
    {
        void TakePhoto(CameraConfiguration config);
    }
    public class PhotoManager : IPhotoManager
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PhotoManager> _logger;

        public PhotoManager(IConfiguration config, ILogger<PhotoManager> logger)
        {
            _config = config;
            _logger = logger;
        }
        public void TakePhoto(CameraConfiguration config)
        {
            //Directory.CreateDirectory(Path.GetDirectoryName(config.Output));
            if (File.Exists(config.Output))
                File.Delete(config.Output);
            _logger.LogInformation($"Taking Photo: " + config);
            $"/usr/bin/raspistill {config}".Bash();
        }
    }
}
