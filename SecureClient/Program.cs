using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using SecureClient.Entity;
using SecureClient.Data;
using Newtonsoft.Json;

using System.Threading.Tasks;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;

namespace SecureClient
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("---> SecureClient startup ... ");

            var strEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            Console.WriteLine($"---> scanning target environment ... ");

            var isDevelopment = string.IsNullOrEmpty(strEnvironmentVariable) || strEnvironmentVariable.ToLower() == "development";

            Console.WriteLine($"---> analysizing current environment ... ");

            string appSettingsFile = ConfigurationAppSettings(Directory.GetCurrentDirectory(), isDevelopment);

            if(string.IsNullOrEmpty(appSettingsFile)) throw new ApplicationException("Unable to select a proper appsettings.jso file");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(appSettingsFile, optional: false, reloadOnChange: true);

            Console.WriteLine($"---> setting configuration builder to current direction {Directory.GetCurrentDirectory()} ... ");
            Console.WriteLine($"---> setting configuration builder to target {appSettingsFile} file ... ");

            if(isDevelopment)
            {
                Console.WriteLine($"---> within target Development environment add UserSecrets to the IConfigurationBuilder ...");
                builder.AddUserSecrets<Program>();
            }

            Configuration = builder.Build();
            Console.WriteLine("---> IConfigurationRoot has been established ...");

            IServiceCollection services = new ServiceCollection();

            //Setup AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            Console.WriteLine("---> added AutoMapper to the list of services ...");

            if (isDevelopment)
            {
                //Services for Development only 
                services
                    .Configure<SecureClient.Entity.AzureAD>(Configuration.GetSection(nameof(SecureClient.Entity.AzureAD)))
                    .AddOptions()
                    .AddSingleton<SecureClient.Interface.IUserSecrets, SecureClient.Data.UserSecrets>();
                Console.WriteLine("---> added IUserSecrets to the list of services ...");
            }

            AuthConfig appAuthVars = LoadAppVars.ReadAppVarServices(Services: services, AppSettings: appSettingsFile, IsProduction: !isDevelopment);
            Console.WriteLine("---> able to load all for the application variables to provide design functionality ...");
            Console.WriteLine(JsonConvert.SerializeObject(appAuthVars));

            Console.WriteLine("Making the Azure Call ....");
            AzureClientConnectionAsync(appAuthVars).GetAwaiter().GetResult();
        }

        private static string ConfigurationAppSettings(string AppPath, bool isDevelopment)
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
                if(isDevelopment && jsonFiles.Where(f => f.ToLower().Contains("development")).Any())
                {
                    result = jsonFiles.Where(f => f.ToLower().Contains("development")).FirstOrDefault();
                }
                else if (!isDevelopment)
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

        private static async Task AzureClientConnectionAsync(AuthConfig AppConfiguration)
        {
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(AppConfiguration.ClientId)
            .WithClientSecret(AppConfiguration.ClientSecret)
            .WithAuthority(new Uri(AppConfiguration.Authority))
            .Build();

            string[] ResouceIds = new string[] {AppConfiguration.ResourceId};
            AuthenticationResult  result = null;

            
            Console.WriteLine("Waiting for Token.......");

            try
            {
                result = await app.AcquireTokenForClient(ResouceIds).ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"--- Token Aquired: {result.AccessToken}");
                Console.ResetColor();
            }
            catch (System.Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            
            Console.WriteLine("Creating Http Client for API Call.......");


            if (!string.IsNullOrEmpty(result.AccessToken))
            {
                var httpClient = new HttpClient();
                var defaultRequestHeaders = httpClient.DefaultRequestHeaders;

                if (defaultRequestHeaders.Accept == null || defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
                {
                    httpClient.DefaultRequestHeaders.Accept
                    .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                }

                defaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", result.AccessToken);

                HttpResponseMessage response = await httpClient.GetAsync(AppConfiguration.BaseAddress);


                if(response.IsSuccessStatusCode)
                {
                    
                    Console.WriteLine("Waiting on API Response .......");
                    Console.ForegroundColor = ConsoleColor.Green;
                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                }
                else    
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to call API: {response.StatusCode}");
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Content: {content}");
                }
                Console.ResetColor();

            }
        }
    }
}
