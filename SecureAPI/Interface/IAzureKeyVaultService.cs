using System.Threading.Tasks;
using SecureAPI.Entity;

namespace SecureAPI.Interface
{
  public interface IAzureKeyVaultService
  {
    public AuthAppConfig LoadSecrets(string VaultName);
  }
}