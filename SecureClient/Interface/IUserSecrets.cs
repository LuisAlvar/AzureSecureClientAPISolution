namespace SecureClient.Interface
{
  public interface IUserSecrets
  {
    public SecureClient.Entity.AuthConfig GetAzureSecrets();
  }
}