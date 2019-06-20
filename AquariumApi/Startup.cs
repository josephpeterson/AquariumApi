using System.IO;
using AquariumApi.Core;
using AquariumApi.Core.Services;
using AquariumApi.DataAccess;
using AquariumApi.DataAccess.AutoMapper;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace AquariumApi
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
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
            services.AddLogging();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Aquarium API", Version = "v1" });
            });

            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            RegisterServices(services);


            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddDbContext<DbAquariumContext>(options => options
                .UseSqlServer(Configuration["Database:dbAquarium"])
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));
            services.AddTransient<IAquariumDao, AquariumDao>();
            services.AddTransient<IPhotoManager, PhotoManager>();
            services.AddTransient<IAquariumService, AquariumService>();
            services.AddTransient<IWebScraperService, WebScraperService>();
            services.AddSingleton<IConfiguration>(Configuration);
        }
    }
}