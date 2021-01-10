using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AquariumApi.Controllers.Form
{
    [Route("/v1/Form/Select")]
    public class FormSelectController : Controller
    {
        private IAquariumService _aquariumService;
        private ILogger<FormSelectController> _logger;

        public FormSelectController(IAquariumService aquariumService, ILogger<FormSelectController> logger)
        {
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [HttpGet, Route("{selectType}")]
        public IActionResult DeleteFishPhoto(string selectType)
        {
            try
            {
                var options = _aquariumService.GetSelectOptionsBySelectType(selectType);
                _logger.LogInformation($"GET /Form/Select/{selectType} called");
                return new OkObjectResult(options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /Form/Select/{selectType} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

    }
}
