
# Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process -Force
Function LoadUserSecretsDev
{
  $ClassName = "AzureAD"

  Write-Host "###################### Load User Secret For Dev #######################"
  
  Write-Host "--- Removing all existing user-secrets"
  dotnet user-secrets clear

  #ClientId
  $Azure_ClientId = [System.Environment]::GetEnvironmentVariable('SecureClientApp_ClientId','Machine')
  $UserSecret = $ClassName + ":" + "ClientId"
  
  Write-Host "--- Loading " $UserSecret
  dotnet user-secrets set $UserSecret $Azure_ClientId

  #TenantId
  $Azure_TenantId = [System.Environment]::GetEnvironmentVariable('SecureClientApp_TenantId','Machine')
  $UserSecret = $ClassName + ":" + "TenantId"

  Write-Host "--- Loading " $UserSecret
  dotnet user-secrets set $UserSecret $Azure_TenantId

  #ResourceId
  $Azure_ResourceId = [System.Environment]::GetEnvironmentVariable('SecureClientApp_ResourceId','Machine')
  $UserSecret = $ClassName + ":" + "ResourceId"

  Write-Host "--- Loading " $UserSecret
  dotnet user-secrets set $UserSecret $Azure_ResourceId

  #ClientSecret
  $Azure_ClientSecret = [System.Environment]::GetEnvironmentVariable('SecureClientApp_ClientSecret','Machine')
  $UserSecret = $ClassName + ":" + "ClientSecret"

  Write-Host "--- Loading " $UserSecret
  dotnet user-secrets set $UserSecret $Azure_ClientSecret

  Write-Host "--- View the list of secrets"
  dotnet user-secrets list
}

LoadUserSecretsDev