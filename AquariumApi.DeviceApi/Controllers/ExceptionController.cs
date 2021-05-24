using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class ExceptionController : ControllerBase
    {
        private IConfiguration _config;
        private ILogger<ExceptionController> _logger;
        private IExceptionService _exceptionService;

        public ExceptionController(IConfiguration config, 
            ILogger<ExceptionController> logger,
            IExceptionService exceptionService)
        {
            _config = config;
            _logger = logger;
            _exceptionService = exceptionService;

        }
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetExceptions()
        {
            try
            {
                _logger.LogInformation($"GET /v1/Exception called");
                var exceptions = _exceptionService.GetExceptions();
                return new OkObjectResult(exceptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Exception caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
    }
}
