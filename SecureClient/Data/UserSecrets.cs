using SecureClient.Interface;
using Microsoft.Extensions.Options;
using System;
using AutoMapper;
using SecureClient.Entity;

namespace SecureClient.Data
{
  public class UserSecrets : IUserSecrets
  {
    private readonly SecureClient.Entity.AzureAD _AD_Secrets;
    private readonly IMapper _mapper;
    
    public UserSecrets(IOptions<SecureClient.Entity.AzureAD> ADSecrets, IMapper AutoMapper)
    {
      _AD_Secrets = ADSecrets.Value ?? throw new ArgumentNullException(nameof(ADSecrets));
      _mapper = AutoMapper;
    }

    public SecureClient.Entity.AuthConfig GetAzureSecrets()
    {
      return _mapper.Map<AuthConfig>(_AD_Secrets);
    }
  }
}