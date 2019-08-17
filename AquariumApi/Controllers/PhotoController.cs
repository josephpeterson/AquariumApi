using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AquariumApi.Controllers
{
    public class PhotoController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<PhotoController> _logger;
        public PhotoController(IAquariumService aquariumService, ILogger<PhotoController> logger)
        {
            _aquariumService = aquariumService;
            _logger = logger;
        }

        [HttpGet]
        [Route("/v1/Fish/Photo/{photoId}/{size}")]
        public IActionResult GetSnapshotPhoto(int photoId, string size = "")
        {
            try
            {
                var data = _aquariumService.GetFishPhotoById(photoId);

                byte[] b;
                if (size == "medium")
                {
                    var destination = Path.GetDirectoryName(data.Filepath) + "/medium/" + Path.GetFileName(data.Filepath);
                    b = System.IO.File.ReadAllBytes(destination);
                }
                else if (size == "small")
                {
                    var destination = Path.GetDirectoryName(data.Filepath) + "/thumbnail/" + Path.GetFileName(data.Filepath);
                    b = System.IO.File.ReadAllBytes(destination);
                }
                else
                    b = System.IO.File.ReadAllBytes(data.Filepath);

                return File(b, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Snapshot/Photo/{photoId}/ endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }

        }
    }
}