using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SecureAPI.Data;
using SecureAPI.Entity;

namespace SecureAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; private set; }
        public IWebHostEnvironment Environment { get; private set; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Setup AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            Console.WriteLine($"----> Added AutoMapper to services ... ");

            var appSettingsFile = ConfigurationAppSettings(AppPath: Directory.GetCurrentDirectory(), IsDevelopment: Environment.IsDevelopment());
            Console.WriteLine($"----> This is the appsettings file for this environment {appSettingsFile}");

            if (Environment.IsDevelopment())
            {
                services.Configure<SecureAPI.Entity.AzureAD>(Configuration.GetSection(nameof(SecureAPI.Entity.AzureAD)))
                .AddOptions()
                .AddScoped<SecureAPI.Interface.IUserSecrets, SecureAPI.Auth.UserSecrets>();
            }

            if (Environment.IsProduction())
            {
                services
                    .AddScoped<SecureAPI.Interface.IAzureKeyVaultService, SecureAPI.Auth.AzureKeyVaultService>();
            }

            AuthAppConfig appConfig = LoadAppConfig.Auth(AppServices: services, AppSettings: appSettingsFile, IsProduction: Environment.IsProduction());
            //Console.WriteLine(JsonConvert.SerializeObject(appConfig));
            //Console.WriteLine(appConfig.Authority);

            if(string.IsNullOrEmpty(appConfig.ResourceId) && string.IsNullOrEmpty(appConfig.TenantId)) throw new ApplicationException("Application is missing auth data to run API securely...");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt => {
                opt.Audience = appConfig.ResourceId;
                opt.Authority = appConfig.Authority;
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    
        private static string ConfigurationAppSettings(string AppPath, bool IsDevelopment)
        {
            string[] files = Directory.GetFiles(AppPath);
            string result = string.Empty;
            List<string> jsonFiles = new List<string>();

            if(files != null && files.Count() > 0)
            {
                jsonFiles = files.Where(f => f.ToLower().Contains(".json")).ToList();
            }
            else
            {
                throw new ArgumentException($"----> Unable to fetch files within the parameter AppPath: {AppPath}");
            }

            if(jsonFiles != null && jsonFiles.Count > 0)
            {
                if(IsDevelopment && jsonFiles.Where(f => f.ToLower().Contains("development")).Any())
                {
                    result = jsonFiles.Where(f => f.ToLower().Contains("development")).FirstOrDefault();
                }
                else if (!IsDevelopment)
                {
                    result = jsonFiles.Where(f => !f.ToLower().Contains("development")).FirstOrDefault();
                }
            }
            else  
            {
                throw new ApplicationException($"----> Unable to find any .json files with path: {AppPath}");
            }

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return string.Empty;
        }

    }
}
