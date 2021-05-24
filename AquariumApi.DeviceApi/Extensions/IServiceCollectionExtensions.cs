using AquariumApi.DeviceApi.Clients;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi.Extensions
{
  public static class IServiceCollectionExtensions
  {
    public static IServiceCollection AddAquariumDevice(this IServiceCollection services)
    {
      RegisterServices(services);
      RegisterHostedServices(services);
      return services;
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IHardwareService, HardwareService>();
        services.AddTransient<ISerialService, SerialService>();
        services.AddTransient<IAquariumClient, AquariumClient>();
        services.AddSingleton<IGpioService, GpioService>();
        services.AddSingleton<IDeviceService, DeviceService>();
        services.AddSingleton<IATOService, ATOService>();
        services.AddSingleton<IExceptionService, ExceptionService>();
        services.AddSingleton<IQueueService, QueueService>();
            services.AddSingleton<IAquariumAuthService, AquariumAuthService>();
            services.AddSingleton<DeviceAPI, DeviceAPI>();
    }
    /* Background services */
    private static void RegisterHostedServices(IServiceCollection services)
    {
      services.AddSingleton<ScheduleService>();
    }
   
  }
}
