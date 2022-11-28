using SecureAPI.Entity;

namespace SecureAPI.Interface
{
  public interface IUserSecrets
  {
    public AuthAppConfig GetAzureSecrets();
  }
}