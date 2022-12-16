using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using SecureAPI.Entity;
using SecureAPI.Interface;

namespace SecureAPI.Auth
{
  public class AzureKeyVaultService : IAzureKeyVaultService
  {
    public readonly ILogger<AzureKeyVaultService> _logger;
    private string KeyValueName { get; set; } = string.Empty;
    private string BaseAzureVaultURL = "https://{0}.vault.azure.net";
    private SecretClient _client;
    private string AzureVaultURL { 
      get
      {
        return String.Format(CultureInfo.InvariantCulture, BaseAzureVaultURL, KeyValueName);
      }
    }

    public AzureKeyVaultService(ILogger<AzureKeyVaultService> Logger)
    {
      Console.WriteLine($"----> Apply AzureKeyValueService ...");
      _logger = Logger;
      KeyValueName = Environment.GetEnvironmentVariable("Azure_Key_Vault_Name", EnvironmentVariableTarget.Machine);
    }

    public AuthAppConfig LoadSecrets()
    {
      AuthAppConfig config = new AuthAppConfig();

      if(!string.IsNullOrEmpty(AzureVaultURL))
      {
        Console.WriteLine($"----> Calling Azure Portal Key Valut ... ");
        _client = new SecretClient(new Uri(AzureVaultURL), new DefaultAzureCredential());
        Console.WriteLine("----> SecretClient object established ... ");
      }

      if(_client != null)
      {
        AzureKeyVault vault = new AzureKeyVault();

        List<PropertyInfo> properties = typeof(AzureKeyVault).GetProperties().ToList();

        properties.ForEach(p => {
          Console.WriteLine(p.Name);
          KeyVaultSecret secret =  _client.GetSecret(p.Name.ToString());
          vault.GetType().GetProperty(p.Name).SetValue(vault, secret.Value);
        });


      }

      return new AuthAppConfig();
    }
    
  }

}
