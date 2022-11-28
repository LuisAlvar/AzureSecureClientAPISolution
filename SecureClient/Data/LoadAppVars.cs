using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SecureClient.Entity;
using SecureClient.Interface;

namespace SecureClient.Data
{
  public class LoadAppVars
  {
      public static AuthConfig ReadAppVarServices(IServiceCollection Services, string AppSettings, bool IsProduction)
      {
          AuthConfig config = AuthConfig.ReadJsonFromFile(AppSettings);
          bool isUserSecretServiceImplemented = Services.Any(s => s.ServiceType == typeof(SecureClient.Interface.IUserSecrets));
          
          if(!IsProduction && isUserSecretServiceImplemented)
          {
            IUserSecrets userSecretsService =  Services.BuildServiceProvider().GetService<SecureClient.Interface.IUserSecrets>();
            config.Merge(userSecretsService.GetAzureSecrets());
          }
          
          return config;
      }
  }
}