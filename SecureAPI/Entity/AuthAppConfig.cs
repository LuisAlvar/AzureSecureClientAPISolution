using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SecureAPI.Entity
{
  public class AuthAppConfig
  {
    public string ResourceId { get; set; } = string.Empty;
    public string InstanceId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    
    public string Authority 
    {
      get
      {
        return String.Format(CultureInfo.InvariantCulture, InstanceId, TenantId);
      }
    }

    public static AuthAppConfig ReadJsonFromFile(string Path)
    {
      IConfiguration Configuration;

      var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(Path);

      Configuration = builder.Build();

      AuthAppConfig data = Configuration
        .GetSection(nameof(AuthAppConfig))
        .Get<AuthAppConfig>();

      return data;
    }

    public void Merge(AuthAppConfig authAppConfig)
    {
      PropertyInfo[] piCurrentProperties =  this.GetType().GetProperties();
      foreach (var item in piCurrentProperties)
      {
        if(item.GetValue(this) == null || string.IsNullOrEmpty(item.GetValue(this).ToString()))
        {
          item.SetValue(this, item.GetValue(authAppConfig));
        }
      }
    }
  }
}