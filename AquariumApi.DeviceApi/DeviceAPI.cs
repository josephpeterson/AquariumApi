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

        public DeviceAPI(IDeviceService deviceService, ScheduleService scheduleService, IGpioService gpioService,IATOService atoService,ILogger<DeviceAPI> logger)
        {
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _gpioService = gpioService;
            _atoService = atoService;
            _logger = logger;
        }

        public void Process()
        {
            _logger.LogInformation("\n\n\nDeviceAPI Starting...");
            //Attempt to contact aquarium service
            try
            {
                var response = _deviceService.PingAquariumService().Result;
                _logger.LogInformation("Device information found for aquarium \"" + response.Aquarium.Name + "\"");
                Setup(response.Aquarium);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not get device information from AquariumService: { ex.Message } Details: { ex.ToString() }");
            }
        }
         
        public void Setup(Aquarium aquarium)
        {
            var device = aquarium.Device;

            _deviceService.Setup(Setup);

            _logger.LogInformation("Setting up GPIO Service...");
            try
            {
                _gpioService.Setup(device);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            _logger.LogInformation("Setting up ATO Service...");
            try
            {
                _atoService.Setup(device);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            _logger.LogInformation("Checking schedule information...");
            try
            {
                _scheduleService.Setup(device);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
