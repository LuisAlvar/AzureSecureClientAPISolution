using System;
using Microsoft.Extensions.Configuration;
using SecureAPI.Entity;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using SecureAPI.Interface;
using System.Linq;

namespace SecureAPI.Data
{
  public class LoadAppConfig 
  {
    public static AuthAppConfig Auth(IServiceCollection AppServices, string AppSettings,  bool IsProduction)
    {
      AuthAppConfig config = AuthAppConfig.ReadJsonFromFile(AppSettings);

      bool isUserSecretServiceImplemented = AppServices.Any(s => s.ServiceType == typeof(SecureAPI.Interface.IUserSecrets));
      bool isAzureKeyVaultServiceImplemented = AppServices.Any(s => s.ServiceType == typeof(SecureAPI.Interface.IAzureKeyVaultService));

      if(!IsProduction && isUserSecretServiceImplemented)
      {
        IUserSecrets userSecretsService = AppServices.BuildServiceProvider().GetService<SecureAPI.Interface.IUserSecrets>();
        config.Merge(userSecretsService.GetAzureSecrets());
      }
      
      if(IsProduction && isAzureKeyVaultServiceImplemented)
      {
        IAzureKeyVaultService azKeyVaultService = AppServices.BuildServiceProvider().GetService<SecureAPI.Interface.IAzureKeyVaultService>();
        azKeyVaultService.LoadSecrets();
      }

      return config;
    }
  }
}