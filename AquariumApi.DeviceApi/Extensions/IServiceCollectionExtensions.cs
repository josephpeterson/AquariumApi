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
      services.AddSingleton<IGpioService, GpioService>();
      services.AddTransient<IAquariumClient, AquariumClient>();
      services.AddSingleton<IDeviceService, DeviceService>();
      services.AddSingleton<IQueueService, QueueService>();
      services.AddSingleton<IAquariumAuthService, AquariumAuthService>();

    }
    /* Background services */
    private static void RegisterHostedServices(IServiceCollection services)
    {
      services.AddSingleton<ScheduleService>();
    }
   
  }
}
