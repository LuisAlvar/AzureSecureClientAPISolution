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
    private string KeyValueName { get; set; } = Environment.GetEnvironmentVariable("Azure_Key_Vault_Name");
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
      _logger = Logger;
    }

    public AuthAppConfig LoadSecrets()
    {
      AuthAppConfig config = new AuthAppConfig();

      Console.WriteLine($"----> This is the Azure URL endpoint: {AzureVaultURL}");

      if(!string.IsNullOrEmpty(AzureVaultURL))
      {
        _client = new SecretClient(new Uri(AzureVaultURL), new DefaultAzureCredential());
      }

      if(_client != null)
      {
        AzureKeyVault vault = new AzureKeyVault();

        List<PropertyInfo> properties = typeof(AzureKeyVault).GetProperties().ToList();

        vault.BaseAPIresourceId = _client.GetSecret("BaseAPIresourceId").Value.ToString();

        // properties.ForEach(p => {
        //   Console.WriteLine(p.Name);
        //   vault.GetType().GetProperty(p.Name).SetValue(this, _client.GetSecret(p.Name.ToString()));
        // });

        Console.WriteLine(vault.BaseAPIresourceId);
        Console.WriteLine(vault.SecureDirectoryId);

      }

      return new AuthAppConfig();
    }
    
  }

}
