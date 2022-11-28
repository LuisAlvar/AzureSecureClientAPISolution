Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process -Force

 

[https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows]

```bash 
dotnet user-secrets initi
```

Set a Secret:
```bash
dotnet user-secrets set "Movies:ServiceApiKey" "12345"
```

Runing this command display the following message
```bash 
dotnet user-secrets list
```

Remove all secrets:
Run the following command from the directory in which thr projecct file exists: 
```bash 
dotnet user-secrets clear 
```

Remove a Particular secret
```bash 
dotnet user-secrets remove "Movies:ServiceApiKey" "12345"
```

Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process -Force

