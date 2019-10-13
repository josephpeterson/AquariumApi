using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Core.Services;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    public class SearchController : Controller
    {
        public readonly IAquariumService _aquariumService;
        private readonly IAccountService _accountService;
        public readonly IWebScraperService _webScraperService;
        public readonly ILogger<SpeciesController> _logger;
        public SearchController(IAquariumService aquariumService,ILogger<SpeciesController> logger,IAccountService accountService)
        {
            _aquariumService = aquariumService;
            _accountService = accountService;
            _logger = logger;
        }
        [HttpPost]
        [ProducesResponseType(typeof(List<Species>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Search")]
        public IActionResult Search([FromBody]SearchOptions  options)
        {
            try
            {
                _logger.LogInformation("GET /v1/Search called");
                options.AccountId = _accountService.GetCurrentUserId();
                var results = _aquariumService.PerformSearch(options);
                return new OkObjectResult(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Search endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        
    }
}