using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AquariumApi.DeviceApi.Clients;
using AquariumApi.DeviceApi.Extensions;
using AquariumApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace AquariumApi.DeviceApi
{
    public class Startup
    {
        private IDeviceService _deviceService;
        private ScheduleService _scheduleService;

        public IConfigurationRoot Configuration { get; }
        private ILogger<Startup> _logger;

        public Startup(IConfiguration configuration)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                    .SetBasePath(currentDirectory)
                    .AddJsonFile($"config.json", optional: false)
                    .AddEnvironmentVariables();
            Configuration = builder.Build();

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        [System.Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);




            services.AddLogging();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AquariumDevice API", Version = "v1" });
            });
            services.AddAquariumDevice();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                Microsoft.Extensions.Hosting.IHostingEnvironment env,
                IDeviceService deviceService,
                ScheduleService scheduleService, ILogger<Startup> logger)
        {
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _logger = logger;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            // global cors policy
            app.UseCors(x => x
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());






            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
              name: "default",
              template: "{controller}/{action=Index}/{id?}");
            });
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                    spa.UseAngularCliServer(npmScript: "start");
            });

            app.UseStaticFiles();
            app.UseHttpsRedirection();


            DeviceBootstrap();
        }
        private void DeviceBootstrap()
        {
            //Attempt to contact aquarium service
            var response = _deviceService.PingAquariumService().Result;

            if (response != null)
            {

                //Check for any existing schedules
                var sa = response.Aquarium.Device.ScheduleAssignments;
                if (sa != null)
                {
                    _logger.LogInformation("Schedules found from service.");
                    var schedules = sa.Select(s => s.Schedule).ToList();
                    _scheduleService.SaveSchedulesToCache(schedules);
                    _scheduleService.StartAsync(new System.Threading.CancellationToken()).Wait();
                }
                else
                    _logger.LogInformation("No schedules deployed on this device.");

                //Send current hardware
                _logger.LogInformation("Scanning hardware...");
                _deviceService.ApplyDeviceHardware().Wait();
            }


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
        }

    }
}
