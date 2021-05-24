using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IExceptionService
    {
        Task Flush();
        Task Throw(Exception exception);
        List<BaseException> GetExceptions();
    }
    public class ExceptionService : IExceptionService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ExceptionService> _logger;
        private readonly IAquariumClient _aquariumClient;
        private List<BaseException> Exceptions = new List<BaseException>();

        public ExceptionService(IConfiguration config,
            ILogger<ExceptionService> logger, 
            IAquariumClient aquariumClient)
        {
            _config = config;
            _logger = logger;
            _aquariumClient = aquariumClient;
        }
        public async Task Flush()
        {
            await _aquariumClient.DispatchExceptions(Exceptions);
            Exceptions.Clear();
        }
        public async Task Throw(Exception exception)
        {
            var ex = new BaseException()
            {
                Type = ExceptionTypes.Device,
                Date = DateTime.Now,
                Source = exception
            };
            Exceptions.Add(ex);

            try { 
                await Flush();
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        List<BaseException> IExceptionService.GetExceptions()
        {
            return Exceptions.OrderByDescending(ex => ex.Date).ToList();
        }
    }
    
}
