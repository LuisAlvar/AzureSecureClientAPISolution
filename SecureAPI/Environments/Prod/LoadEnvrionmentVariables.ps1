
# Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process -Force

Function LoadEnvironmentVariables
{

  $ClassName = "AzureAD"
  
  Write-Host "###################### Load Environment Variables Production #######################"

  $AzureKeyVault = [System.Environment]::GetEnvironmentVariable('Azure_Key_Vault_Name','Machine')

  dotnet user-secrets clear --project ../../SecureAPI.csproj

  #Azure_Key_Vault_Name
  $Azure_Key_Vault_Name = [System.Environment]::GetEnvironmentVariable('Azure_Key_Vault_Name','Machine')
  $UserSecret = $ClassName + ":" + "Azure_Key_Vault_Name"
  Write-Host "--- Loading " $UserSecret
  dotnet user-secrets set $UserSecret $Azure_Key_Vault_Name --project ../../SecureAPI.csproj

  Write-Host "--- View the list of secrets"
  dotnet user-secrets list --project ../../SecureAPI.csproj


}

LoadEnvironmentVariables