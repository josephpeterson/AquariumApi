using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    public class SnapshotController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<SnapshotController> _logger;
        public SnapshotController(IAquariumService aquariumService, ILogger<SnapshotController> logger)
        {
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetSnapshots([FromQuery] int count, [FromQuery] int offset = 0)
        {
            var data = _aquariumService.GetAllAquariums();
            return new OkObjectResult(data);
        }
    }
}