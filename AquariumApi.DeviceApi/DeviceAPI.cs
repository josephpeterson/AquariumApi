using AquariumApi.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public class DeviceAPI
    {
        private IDeviceService _deviceService;
        private ScheduleService _scheduleService;
        private IGpioService _gpioService;
        private IATOService _atoService;
        private ILogger<DeviceAPI> _logger;
        private IAquariumAuthService _aquariumAuthService;

        public DeviceAPI(ILogger<DeviceAPI> logger,IAquariumAuthService aquariumAuthService, IDeviceService deviceService, ScheduleService scheduleService, IGpioService gpioService,IATOService atoService)
        {
            _logger = logger;
            _aquariumAuthService = aquariumAuthService;
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _gpioService = gpioService;
            _atoService = atoService;
        }

        public void Process()
        {
            _logger.LogInformation("\n\n\nDeviceAPI Starting...");
            _aquariumAuthService.Setup(Setup,CleanUp); //todo maybe move this to an AquariumServiceCollection??
            if(_aquariumAuthService.GetToken() != null)
            {
                try
                {
                    _aquariumAuthService.RenewAuthenticationToken().Wait();
                    Setup();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Could not get device information from AquariumService. Token may be expired");
                    _aquariumAuthService.Logout(); // this will delete the token
                }
            }
            else
                _logger.LogInformation("Please navigate to your device's hostname to log into the Aquarium Service with your account.");

        }

        public void Setup()
        {
            _logger.LogInformation("Setting up Schedule Service...");
            try
            {
                _scheduleService.Setup();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
        public void CleanUp()
        {
            _logger.LogInformation("Cleaning up device services...");
            _deviceService.CleanUp();
            _gpioService.CleanUp();
            _atoService.CleanUp();
            _scheduleService.CleanUp();
        }
    }
}
