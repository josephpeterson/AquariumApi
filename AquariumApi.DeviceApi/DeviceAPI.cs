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
        private ILogger<DeviceAPI> _logger;
        private IAquariumAuthService _aquariumAuthService;
        private readonly IDeviceConfigurationService _deviceConfiguration;

        public DeviceAPI(ILogger<DeviceAPI> logger,IAquariumAuthService aquariumAuthService, IDeviceConfigurationService deviceConfiguration,IDeviceService deviceService, ScheduleService scheduleService, IGpioService gpioService)
        {
            _logger = logger;
            _aquariumAuthService = aquariumAuthService;
            _deviceConfiguration = deviceConfiguration;
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _gpioService = gpioService;
        }

        public void Process()
        {
            _logger.LogInformation("\n\n\nDeviceAPI Starting...");
            _aquariumAuthService.Setup(Setup,CleanUp); //todo maybe move this to an AquariumServiceCollection??

            var configuredDevice = _deviceConfiguration.LoadDeviceConfiguration();
            if (configuredDevice == null)
            {
                _logger.LogInformation($"No device configuration found. Applying default configuration...");
                configuredDevice = new DeviceConfiguration();
                _deviceConfiguration.SaveDeviceConfiguration(configuredDevice);
            }
            else
            {
                _logger.LogInformation($"------------------------------------");
                _logger.LogInformation($"-+ Device Configuration");
                _logger.LogInformation($"-+ Sensors: {configuredDevice.Sensors.Count}");
                _logger.LogInformation($"-+ Tasks: {configuredDevice.Tasks.Count}");
                _logger.LogInformation($"-+ Schedules: {configuredDevice.Schedules.Count}");
                _logger.LogInformation($"-+ Aquarium: {configuredDevice.Aquarium?.Name}");
                _logger.LogInformation($"------------------------------------");
            }

            if (_aquariumAuthService.IsLoggedIn())
            {
                try
                {
                    _aquariumAuthService.RenewAuthenticationToken().Wait();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Could not get device information from AquariumService. Token may be expired");
                    _aquariumAuthService.Logout(); // this will delete the token
                }
            }
            else
                _logger.LogInformation("Connect your aquarium monitor account for more features! Connect to your device's hostname to link your account.");

            Setup();
        }

        public void Setup()
        {
            _logger.LogInformation("Setting up services...");
            try
            {
                _gpioService.Setup();
                _scheduleService.Setup();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            _logger.LogInformation("Schedule Service set up complete.");

        }
        public void CleanUp()
        {
            _logger.LogInformation("Cleaning up device services...");
            _gpioService.CleanUp();
            _scheduleService.CleanUp();
        }
    }
}
