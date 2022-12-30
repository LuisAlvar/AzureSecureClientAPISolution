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
You need an Microsoft Azure account and need to create a new tenant or a tenant other than your Default Directory as of 11/27/2022. 

You will need to switch to your new tenant or existing directory other than your Default Directory. 

Once switched to the proper tenant directory, go to **Azure Active Directory**. 
Here you will navigate to **App registrations** within the side menu. 

On the right side window within **App registrations** click on the + sign for **New registration**. 

#### Register an application 
1. Give your applicaiton a name lol 
2. Next select: Accounts in this organizational directory only 
3. Redirect UR: Leave this as is


## Azure .NET Core Web API 
When you resigter the Web API ensure you able to see the following values on the **Overview** window: 
1. Application (client) ID 
2. Directory (tenant) ID
3. Set Application ID URL to the default 
  
To set the application id url: 
- Go to **Expose an API** window, on the top of the window set the applicaiton id url to the default value given by Azure. 

Next, we need to add an App role. We can do this manually or edit the manifest file at the appRoles level. 

	"appRoles": [],

	(Replace the above with the following json segment)

	"appRoles": [
		{
			"allowedMemberTypes": [
				"Application"
			],
			"description": "Daemon apps in this role can consume the web api.",
			"displayName": "DaemonAppRole",
			"id": "1cbd43bd-70ec-4cdb-ac90-0b9a3d108a55",
			"isEnabled": true,
			"lang": null,
			"origin": "Application",
			"value": "DaemonAppRole"
		}
	],

Or go to **App roles** click on **+ Create app role** and fill in the following fields.
1. Display name              <-- DaemonAppRole
2. Allowed member types      <---- Applications
3. Value                     <-- DaemonAppRole 
4. Description               <-- Daemon apps in this role can consume the web api. 
   
If you want to remove an app role, then you first need to disable it and then delete it. 

You go to the manifest file, you will see that Azure added the new app role to the json file. 


## Azure .NET Core Console App 

You will need to register the client app as well. Go through the same steps as above. 

At this point on the Overview we should see a client id and tenant id. 

Next go to **Certificated & secrets**, and under **Client secrets** click on **+ New client secret**. Go through the step. 
At the end of the process, you will see 
the description of your new client secret along with a **Value** and **Secret Id**. 

Please save this information on a file for future reference. Azure will redact the **Value** field for security purposes. 

Next, go to **API permissions** and click on **+ Add a permission**. Azure will prompt you to select an API, and we will click on the **My APIs** tab and click an the API you created above. 

Afterwards, you will need to click on **Grant admin consent for [tenant name]**.

## Overview
At this point we have fully setup Azure to authenicate and authorize us to use the API from a console application. 

# Dev Deployment Guide 
## Windows 
Within a Microsoft Windows environment we will need to add all of the sensitive azure information (i.e., identifiers) as Windows system variables.  

Search for > Edit the system environment variables > Environment Variables. Under the System Variables section click on **New...** 

Add the following properties with their approciate values for each project type. 

For the SecureAPI project 
1. SecureAPIApp_TenantId
2. SecureAPIApp_ResourceId (i.e., Your Application ID URL) with /.default

For the SecureClient project 
1. SecureClientApp_ClientId - Console App Registerd Client Id
2. SecureClientApp_TenantId - Console App Registered Tenant Id
3. SecureClientApp_ResourceId (aka your SecureAPIApp_ResourceId + /.default )
4. SecureClientApp_ClientSecret (You client secret Value field value) - you can allow regenerated 

SecureAPIApp_TenantId == SecureClientApp_TenantId should be the same. 

You can name these as you see please. 

Then run do allow you to run the following powershell script within each project. 
```ps1
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process -Force
```

Next run the following command within each project root directory
```bash 
dotnet user-secrets init
```

Then run the following powershell scripts within each root directory project. These scripts will add the system variables to user-secrets.

Then you can run both projects. First the API and then the console app. 

# UAT or Pre-Production Environment 
To locally test production functionality you can change the ASPNETCORE_ENVIRONMENT from **Development** to **Production** launchSettings.json 

```json
{
  "profiles": {
    "EnvironmentsSample": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```
Run the applicaiton using **dotnet run** command 

Or, we can create an docker image for a more production setting environment. 

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
* This is an inspire code walkthrough by Les Jackson.
  
[Safe sotrage of app secrets in development in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows])

[How to Set Docker Environment Variables](https://phoenixnap.com/kb/docker-environment-variables)