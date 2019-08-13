using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;

namespace AquariumApi.DeviceApi
{
    public class Startup
    {
        private IDeviceService _deviceService;
        private IScheduleService _scheduleService;

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

        private async void DeviceBootstrap()
        {   
            try
            {
                var device = await _deviceService.PingAquariumService();
                _logger.LogInformation("[Connected to Aquarium Service]");

                _logger.LogInformation("Device Information: "
                    + $"\nAquarium Name: {device.Aquarium.Name}"
                    + $"\nEnabled Photo: {device.EnabledPhoto}"
                    + $"\nEnabled Temperature: {device.EnabledTemperature}"
                    );

                //Start schedule service
                _scheduleService.Start();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Could not get device information from AquariumService: { ex.Message } Details: { ex.ToString() }");
            }

            try
            {
                //Pi.Init<BootstrapWiringPi>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not enable wiring: { ex.Message } Details: { ex.ToString() }");
            }
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        [System.Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "AquariumDevice API", Version = "v1" });
            });


            RegisterServices(services);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<IHardwareService, HardwareService>();
            services.AddTransient<ISerialService, SerialService>();
            services.AddTransient<IAquariumClient, AquariumClient>();
            services.AddSingleton<IDeviceService, DeviceService>();
            services.AddSingleton<IScheduleService, ScheduleService>();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDeviceService deviceService, ILogger<Startup> logger,IScheduleService scheduleService)
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

            app.UseStaticFiles();



            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });

            app.UseMvc(routes =>
            {
                // default routes plus any other custom routes
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapSpaFallbackRoute(
                name: "spa-fallback",
                defaults: new { controller = "Home", action = "Spa" });
            });
            app.UseHttpsRedirection();

            DeviceBootstrap();
        }
    }
}
