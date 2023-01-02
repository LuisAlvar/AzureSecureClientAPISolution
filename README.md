# AzureSecureClientAPISolution
This is a template solution where we use Azure App Registration to secure both .NET Core web API and Console App. 

The main components of this repo: 
```bash 
dotnet new webapi --name SecureAPI --framework netcoreapp3.1 
dotnet new console --name SecureClient --framework netcoreapp3.1
```
Add these different projects to the main solution file 
```bash 
dotnet new sln --name AzureSecureSolution 
dotnet sln AzureSecureSolution.sln add **/*.csproj
```

# Azure Setup | App Registrations
You need an Microsoft Azure account and need to create a new tenant or a tenant other than your Default Directory as of 11/2022. 

You will need to switch to your new tenant or existing directory other than your Default Directory. 

Once switched to the proper tenant directory, go to **Azure Active Directory**. 
Here you will navigate to **App registrations** within the side menu. 

On the right side window within **App registrations** click on the + sign for **New registration**. 

### **Register an application steps**
1. Give your applicaiton a name
2. Next select: Accounts in this organizational directory only 
3. Redirect UR: Leave this as is

## **<ins>Azure .NET Core Web API</ins>**
When you resigter the Web API ensure you able to see the following values on the **Overview** window

### **Secure Registration Infomation** 
| |  |
|------|-------|
|Application (client) ID| 1fccf277-5847-46df-b6ad-bec55c884f1d |
|Directory (tenant) ID | 0deb5300-9432-4af9-a3b6-c7654ed83cba|

[These GUIDs are generated randomly from an online geneartor - for demonstaration purposes only disclaimer](https://guidgenerator.com/online-guid-generator.aspx)

To set the Application ID URL: 
- When you create the App registration it will automatically generate the client id.
- The tenand id is basically your current directory identifier. 
- To set your Application ID URL, you will need to follow the next steps. 
- Go to **Expose an API** window, on the top of the window, click on set the Application ID URL to the default value given by Azure. 

### **Updated Secure Registration Infomation** 
| |  |
|------|-------|
|Application (client) ID| 1fccf277-5847-46df-b6ad-bec55c884f1d |
|Directory (tenant) ID | 0deb5300-9432-4af9-a3b6-c7654ed83cba|
|Application ID URL| api://0e61624c-3cd3-4379-8ede-e6b1bc670c79|

### **Add App Role to Web API**
Application roles are used to assign permissions to users or applications. 
* Check out for more information: [Roles using Azure AD App Roles](https://learn.microsoft.com/en-us/azure/architecture/multitenant-identity/app-roles#roles-using-azure-ad-app-roles)

Next, we need to add an App role: We can do this manually or edit the manifest file at the appRoles level. 

	"appRoles": [],

	(Replace the above with the following json segment)

	"appRoles": [
		{
			"allowedMemberTypes": [
				"Application"
			],
			"description": "Daemon apps in this role can consume the web api.",
			"displayName": "DaemonAppRole",
			"id": "a3789253-df9b-445f-a09a-2b977a874d48",
			"isEnabled": true,
			"lang": null,
			"origin": "Application",
			"value": "DaemonAppRole"
		}
	],

Go to **App roles** click on **+ Create app role** and fill in the following fields.

| |Field|Value| 
|-|-----|------|
|1|Display Name| <-- DaemonAppRole
|2|Allowed member types| <-- Applications 
|3|Value| <-- DaemonApprole
|4|Description| <-- Daemon apps in this role can consume the web api.

### **Remove App Roles to Web API**
If you want to **remove an app role**, then you first need to disable it and then delete it.

### **View App Roles to Web API**
You go to the manifest file, you will see that Azure added the new app role to the json file. 


## **<ins>Azure .NET Core Console App</ins>** 
You will need to register the client console app as well. Go through the same steps as above. 

At this point on the Overview we should see a client id and tenant id. 
### **Secure Registration Infomation** 
| |  |
|------|-------|
|Application (client) ID| fd04f998-92aa-427d-b642-50ab4bda86d3 |
|Directory (tenant) ID | 0deb5300-9432-4af9-a3b6-c7654ed83cba|

### **Create a secret** 
Next go to **Certificated & secrets**, and under **Client secrets** click on **+ New client secret**. Go through the step. 
At the end of the process, you will see 
the description of your new client secret along with a **Value** and **Secret Id**. 

|Name| Value | Secret Id|
|------|-------|--------|
|ConsoleAPIClientSecret| 7d4f2708X=?ae5cf48ed4ad753111e42798da|4c547662-a306-4ede-8157-2334125fd621

[These GUIDs are generated randomly from an online geneartor - for demonstaration purposes only disclaimer](https://guidgenerator.com/online-guid-generator.aspx)

Azure will redact the **Value** field for security purposes, so copy the value for the **Value** field and this is your so call Client Secret for the Console App. 

### **Grant Console App Access to API**
Next, go to **API permissions** and click on **+ Add a permission**. Azure will prompt you to select an API, and we will click on the **My APIs** tab and click an the API you created above. 

Afterwards, you will need to click on **Grant admin consent for [tenant name]**.

# Overview
At this point we have fully setup Azure to authenicate and authorize us to use the API from a console application. Now, add the following logic.

# Dev Deployment Guide 
## **<ins>Windows</ins>** 
Within a Microsoft Windows environment we will need to add all of the sensitive azure information (i.e., identifiers) as Windows system variables.  

### **<ins>Add Azure Inforamtion to System/User Environment in Windows<ins>**
Search for > Edit the system environment variables > Environment Variables. Under the System Variables section click on **New...** 

Add the following properties with their approciate values for each project type. 

### **Secure Registration Infomation for Web API** 
| |  |
|------|-------|
|Application (client) ID| 1fccf277-5847-46df-b6ad-bec55c884f1d |
|Directory (tenant) ID | 0deb5300-9432-4af9-a3b6-c7654ed83cba|
|Application ID URL| api://0e61624c-3cd3-4379-8ede-e6b1bc670c79|

For the SecureAPI project 
1. SecureAPIApp_TenantId
2. SecureAPIApp_ResourceId (i.e., Your Application ID URL) without /.default

### **Secure Registration Infomation for Console App** 
| |  |
|------|-------|
|Application (client) ID| fd04f998-92aa-427d-b642-50ab4bda86d3 |
|Directory (tenant) ID | 0deb5300-9432-4af9-a3b6-c7654ed83cba|
|Client Secret|7d4f2708X=?ae5cf48ed4ad753111e42798da|

For the SecureClient project 
1. SecureClientApp_ClientId - Console App Registerd Client Id
2. SecureClientApp_TenantId - Console App Registered Tenant Id
3. SecureClientApp_ResourceId (aka your SecureAPIApp_ResourceId + /.default )
4. SecureClientApp_ClientSecret (Your client secret Value field value) - you alway delete and get a new one.
  
SecureAPIApp_TenantId == SecureClientApp_TenantId should be the same. 

<ins>**You can name these as you see please.**</ins>
### **<ins>Load System/User Environment Windows as User Secrets within .NET Core Environment</ins>**

If this is the first attempt loading User Secrets to .NET Core, then run the following:
Next run this command within each project root directory for SecureAPI and SecureClient
```bash 
dotnet user-secrets init
```

Open powershell as admin and run the following powershell command within the root directory of this repo. 
```ps1
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process -Force
```
Then run the following powershell scripts within each root directory project. These scripts will add the system variables to user-secrets.

```ps1
./SecureAPI/Environments/LoadUserSecretsForAzureAD.ps1
./SecureClient/LoadUserSecretsForAzureAD.ps1
```

Then you can run both projects. First the API and then the console app. 

# UAT or Pre-Production Environment(s)

## <ins>**Azure Key Vault**<ins> 
The main requirement for using Azure Key Vault in your code is to have access to your azure account, this way you can use the DefaultAzureCredential object or need for the token route option. 
```cs
new SecretClient(new Uri(AzureVaultURL), new DefaultAzureCredential());
```
So, in a UAT local environment you can change the ASPNETCORE_ENVRIONMENT to Production. Then login into your azure account and set which subscription account.
```bash 
az login
az account set --subscription "21848119-f256-48cb-8092-a5f72366974c"
```
To locally test production functionality you can change the ASPNETCORE_ENVIRONMENT from **Development** to **Production** on launchSettings.json and execute the dotnet-run command
```json
{
  "profiles": {
    "EnvironmentsSample": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Production"
      }
    }
  }
}
```
Now, this is a manualy process still. To start we need to create a docker image. 
When you create an Azure Key Vault you give it a name which becomes part of the Vault URL. For example, testazuekeyvault: https://testazurekeyvault.vault.azure.net/.


## Types of Docker Environment Variables 
There are two enviroment variable types: 
* ARG variables usually store important high-level configuration parameters, such as the version of the OS or a library. They are build-time variables, i.e., their only purpose is to assist in building a Docker image. 
  
* ENV variables store values such as secrets, API keys, and database URLs. They persist inside the image and the containers created from that template. Users can override ENV values in the command line or proivde an new value in an env file. 
  
	

# Next Steps
The objective for this project to add these project in a production environment. 
I need to research how Key Valute on Azure works. 

[Azure Key Vault Docs](https://learn.microsoft.com/en-us/azure/key-vault/)

az login

az account set --subscription "35akss-subscription-id"

Azure_Key_Vault_Name - for production will need to be a environment variable group in the devops pipeline


# Web Resources

[Secure a .NET Core API with Bearer Authentication](https://www.youtube.com/watch?v=3PyUjOmuFic)
  
[Safe sotrage of app secrets in development in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows])

[How to Set Docker Environment Variables](https://phoenixnap.com/kb/docker-environment-variables)

[These GUIDs are generated randomly from an online geneartor - for demonstaration purposes only disclaimer](https://guidgenerator.com/online-guid-generator.aspx)