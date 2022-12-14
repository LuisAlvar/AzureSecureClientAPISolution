
# Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process -Force
Function LoadUserSecretsDev
{
  $ClassName = "AzureAD"

  Write-Host "###################### Load User Secret For Dev #######################"
  
  Write-Host "--- Removing all existing user-secrets"
  dotnet user-secrets clear --project ../../SecureAPI.csproj

  #ResourceId
  $Azure_ResourceId = [System.Environment]::GetEnvironmentVariable('SecureAPIApp_ResourceId','Machine')
  $UserSecret = $ClassName + ":" + "ResourceId"
  
  Write-Host "--- Loading " $UserSecret
  dotnet user-secrets set $UserSecret $Azure_ResourceId --project ../../SecureAPI.csproj

  #TenantId
  $Azure_TenantId = [System.Environment]::GetEnvironmentVariable('SecureAPIApp_TenantId','Machine')
  $UserSecret = $ClassName + ":" + "TenantId"

  Write-Host "--- Loading " $UserSecret
  dotnet user-secrets set $UserSecret $Azure_TenantId --project ../../SecureAPI.csproj

  Write-Host "--- View the list of secrets"
  dotnet user-secrets list --project ../../SecureAPI.csproj
}

LoadUserSecretsDev