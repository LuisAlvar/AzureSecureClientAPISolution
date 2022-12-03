
# Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process -Force

Function LoadEnvironmentVariables
{

  Write-Host "###################### Load Environment Variables Production #######################"

  $AzureKeyVault = [System.Environment]::GetEnvironmentVariable("Azure_Key_Vault_Name")

  if([string]::IsNullOrEmpty($AzureKeyVault))
  {
    Write-Host "Azure_Key_Vault_Name Does Not Exists"
    [System.Environment]::SetEnvironmentVariable('Azure_Key_Vault_Name','')
  }
  else 
  {
    Write-Host $AzureKeyVault
  }

}

LoadEnvironmentVariables