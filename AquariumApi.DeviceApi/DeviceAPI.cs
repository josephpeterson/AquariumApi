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
            var response = _deviceService.PingAquariumService().Result;
            AquariumDevice device = null;

            if (response != null)
            {
                //Send current hardware
                _logger.LogInformation("Scanning hardware...");
                _deviceService.ApplyDeviceHardware().Wait();
                device = _deviceService.GetDeviceFromService().Result;
            }
            //Get detailed device information
            if(device != null)
            {
                //apply gpio infomation
                _logger.LogInformation("Setting up GPIO information...");
                var sensors = device.Sensors;
                _logger.LogInformation($"{sensors.Count()} sensors found...");
                sensors.ToList().ForEach(s =>
                {
                    _gpioService.RegisterDevicePin(s);
                });








                //check if ato is enabled
                _logger.LogInformation("Checking if ATO is enabled...");
                try
                {
                    _atoService.Setup(device);
                } catch(Exception e)
                {
                    _logger.LogError("Could not run ATO setup");
                    _logger.LogError(e.Message);
                }



                //Check schedules
                _logger.LogInformation("Checking schedule information...");
                var sa = device.ScheduleAssignments;
                if (sa != null)
                {
                    _logger.LogInformation($"{sa.Count()} Schedules found");
                    var schedules = sa.Select(s => s.Schedule).ToList();
                    _scheduleService.SaveSchedulesToCache(schedules);
                    _scheduleService.StartAsync(new System.Threading.CancellationToken()).Wait();
                }
                else
                    _logger.LogInformation("No schedules are deployed on this device.");
            }



            /*
            //Attempt to enable wiringPi
            _logger.LogInformation("*** Attempting to enable WiringPi ***");
            try
            {
                //Pi.Init<BootstrapWiringPi>();

                _logger.LogInformation("WiringPi Enabled");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not enable wiring: { ex.Message } Details: { ex.ToString() }");
            }
            */
        }

    }
}
