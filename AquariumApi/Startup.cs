﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using AquariumApi.Core;
using AquariumApi.Core.Services;
using AquariumApi.DataAccess;
using AquariumApi.DataAccess.AutoMapper;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Aquarium API", Version = "v1" });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
                //c.AddSecurityRequirement(security);
            });


            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            RegisterServices(services);

            //Authorization
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]))
                };
            });


            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddNewtonsoftJson(o =>
                {
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
                /* This no longer supports self referencing loop. We had to fall back. 
                 * Eventually fix the loops and remove Microsoft.AspNetCore.Mvc.NewtonsoftJson
                .AddJsonOptions(options => {
                    //options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
                */
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

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

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseRouting();
             app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                );
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });
            app.UseHttpsRedirection();
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddDbContext<DbAquariumContext>(options => options
                .UseSqlServer(Configuration["Database:dbAquarium"]));
            //.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));
            services.AddTransient<IAquariumDao, AquariumDao>();
            services.AddTransient<IAquariumService, AquariumService>();
            services.AddTransient<IFishService, FishService>();
            services.AddTransient<IAdministrativeService, AdministrativeService>();
            services.AddTransient<IWebScraperService, WebScraperService>();
            services.AddTransient<IDeviceClient, DeviceClient>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IAzureService, AzureService>();
            services.AddTransient<IActivityService, ActivityService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IPhotoManager, PhotoManager>();
            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddTransient<IEmailerService, EmailerService>();
            services.AddTransient<IIntelligenceService, IntelligenceService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<IAquariumDeviceInteractionService,AquariumDeviceInteractionService>();
        }
    }
}