using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SecureAPI.Entity;
using SecureAPI.Interface;

namespace SecureAPI.Auth
{
  public class AzureKeyVaultService : IAzureKeyVaultService
  {
    public readonly ILogger<AzureKeyVaultService> _logger;
    private readonly IMapper _mapper;

    private string KeyValueName { get; set; } = string.Empty;
    private string BaseAzureVaultURL = "https://{0}.vault.azure.net";
    private SecretClient _client;
    private string AzureVaultURL { 
      get
      {
        return String.Format(CultureInfo.InvariantCulture, BaseAzureVaultURL, KeyValueName);
      }
    }

    public AzureKeyVaultService(ILogger<AzureKeyVaultService> Logger, IMapper Mapper)
    {
      Console.WriteLine($"----> Apply AzureKeyValueService ...");
      _logger = Logger;
      _mapper = Mapper;
      KeyValueName = Environment.GetEnvironmentVariable("Azure_Key_Vault_Name", EnvironmentVariableTarget.Machine) 
        ?? Environment.GetEnvironmentVariable("Azure_Key_Vault_Name" , EnvironmentVariableTarget.Process);
      Console.WriteLine($"---->  AzureKeyValueService Name -- {KeyValueName}");
    }

    public AuthAppConfig LoadSecrets(string VaultName)
    {
      AuthAppConfig config = new AuthAppConfig();

      if(string.IsNullOrEmpty(KeyValueName))
      {
        KeyValueName = VaultName;
        Console.WriteLine($"---->The Vault Azure is set from appsetting.json {KeyValueName}");
      }

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
          KeyVaultSecret secret =  _client.GetSecret(p.Name.ToString());
          Console.WriteLine($"{p.Name} -- {secret.Value}");
          vault.GetType().GetProperty(p.Name).SetValue(vault, secret.Value);
        });

        Console.WriteLine($"----> Done fetching azure secrets");


        Console.WriteLine($"---> vault object -- " + JsonConvert.SerializeObject(vault).ToString());

        config = _mapper.Map<AuthAppConfig>(vault);
        
        Console.WriteLine($"---> config object -- " + JsonConvert.SerializeObject(config).ToString());

      }
      
      return config;
    }
    
  }

}
