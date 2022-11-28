using System;
using System.IO;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Newtonsoft.Json;

namespace SecureClient.Entity
{
  public class AuthConfig
  {
    public string Instance { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Authority 
    {
      get
      {
        return String.Format(CultureInfo.InvariantCulture, Instance, TenantId);
      }
    }
    public string ClientSecret { get; set; } = string.Empty;
    public string BaseAddress { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
  
    public static AuthConfig ReadJsonFromFile(string path)
    {
      IConfiguration Configuration;

      var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(path);

      Configuration = builder.Build();

      return Configuration.Get<AuthConfig>();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="authConfig"></param>
    public void Merge(AuthConfig authConfig)
    {
      PropertyInfo[] piCurrentProperties =  this.GetType().GetProperties();
      foreach (var item in piCurrentProperties)
      {
        if(item.GetValue(this) == null || string.IsNullOrEmpty(item.GetValue(this).ToString()))
        {
          item.SetValue(this, item.GetValue(authConfig));
        }
      }
    }
  }

}