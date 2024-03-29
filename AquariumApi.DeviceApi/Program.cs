﻿using System;
using System.IO;
using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog.Web;

namespace AquariumApi.DeviceApi
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static DeviceConfiguration DeviceConfiguration { get; set; }
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                var builder = new ConfigurationBuilder()
                .AddJsonFile("config.json");
                Configuration = builder.Build();
                DeviceConfiguration = DeviceConfigurationService.LoadDeviceConfiguration(Configuration);
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                logger.Error(e, "Stopped program because of exception");
                throw;
            }
        }



        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://*:{DeviceConfiguration.Settings.Port}")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseNLog();
    }
}
