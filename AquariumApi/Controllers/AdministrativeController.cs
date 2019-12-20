using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("/v1/admin")]
    public class AdministrativeController : Controller
    {
        private readonly IConfiguration _config;
        public readonly IAquariumService _aquariumService;
        private readonly IAdministrativeService _administrativeService;
        public readonly ILogger<SnapshotController> _logger;
        private readonly IAzureService _azureService;

        public AdministrativeController(IConfiguration config, IAquariumService aquariumService, IAdministrativeService administrativeService, ILogger<SnapshotController> logger,IAzureService azureService)
        {
            _config = config;
            _aquariumService = aquariumService;
            _administrativeService = administrativeService;
            _logger = logger;
            _azureService = azureService;
        }
        [HttpGet]
        [Route("Log")]
        public string GetApplicationLog()
        {
            try
            {
                return System.IO.File.ReadAllText("AquariumApi.log");
            }
            catch(FileNotFoundException) {
                return "No error log found";
            }
        }
        [HttpGet]
        [Route("Log/Delete")]
        public IActionResult DeleteApplicationLog()
        {
            try
            {
                System.IO.File.Delete(_config["DashboardLogFilePath"]);
                return new OkResult();
            }
            catch (FileNotFoundException)
            {
                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
            }
        }

        [HttpGet]
        [Route("Users")]
        public IActionResult GetAquariumUsers()
        {
            try
            {
                var accounts = _administrativeService.GetAquariumUsers();
                return new OkObjectResult(accounts);
            }
            catch
            {
                return new BadRequestResult();
            }
        }
        [HttpGet]
        [Route("Bugs")]
        public IActionResult GetBugList()
        {
            try
            {
                var reports = _administrativeService.GetBugReports();
                return new OkObjectResult(reports);
            }
            catch(Exception ex)
            {
                _logger.LogError($"GET /v1/Admin/Bugs endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }


        [HttpGet]
        [Route("Test")]
        public IActionResult TestMethod()
        {
            try
            {
                var data = System.IO.File.ReadAllBytes("config.json");
                //_azureService.UploadFileToStorage(data,"test.json");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Admin/Bugs endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

    }
}