using AutoMapper;
using SecureAPI.Entity;

namespace SecureAPI.AutoMapper.Profiles
{
  public class AuthAppProfile: Profile
  {
    public AuthAppProfile()
    {
      CreateMap<AzureAD, AuthAppConfig>();
      
      CreateMap<AzureKeyVault, AuthAppConfig>()
      .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.BaseAPIResourceId))
      .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.SecureDirectoryId));

    }
  }
}