using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    public class AquariumController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<SnapshotController> _logger;
        public AquariumController(IAquariumService aquariumService, ILogger<SnapshotController> logger)
        {
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [HttpGet]
        [Route("/v1/Aquariums")]
        [ProducesResponseType(typeof(List<Aquarium>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("GET /v1/Aquariums called");
                var aquariums = _aquariumService.GetAllAquariums();
                return new OkObjectResult(aquariums);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Aquariums endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}