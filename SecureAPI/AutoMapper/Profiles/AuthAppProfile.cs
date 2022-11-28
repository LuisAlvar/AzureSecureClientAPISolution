using AutoMapper;
using SecureAPI.Entity;

namespace SecureAPI.AutoMapper.Profiles
{
  public class AuthAppProfile: Profile
  {
    public AuthAppProfile()
    {
      CreateMap<AzureAD, AuthAppConfig>();
    }
  }
}