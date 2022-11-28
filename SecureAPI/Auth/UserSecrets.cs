using System;
using AutoMapper;
using Microsoft.Extensions.Options;
using SecureAPI.Entity;
using SecureAPI.Interface;

namespace SecureAPI.Auth
{
  public class UserSecrets : IUserSecrets
  {
    private readonly IOptions<AzureAD> _AD_Secrets;
    private readonly IMapper _mapper;

    public UserSecrets(IOptions<AzureAD> ADSecrets, IMapper Mapper)
    {
      _AD_Secrets = ADSecrets ?? throw new ApplicationException(nameof(ADSecrets));
      _mapper = Mapper;
    }

    public AuthAppConfig GetAzureSecrets()
    {
      return _mapper.Map<AuthAppConfig>(_AD_Secrets.Value);
    }

  }
}