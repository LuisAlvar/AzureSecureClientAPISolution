using AutoMapper;
using SecureClient.Entity;

namespace SecureClient.AutoMapper.Profiles
{
  public class AppAuthProfile: Profile
  {
    public AppAuthProfile()
    {
      CreateMap<AzureAD, AuthConfig>();
    }
  }
}