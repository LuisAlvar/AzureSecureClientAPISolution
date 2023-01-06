# Full .NET Core 3.1 SDK
# https://hub.docker.com/_/microsoft-dotnet-sdk
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
# ARG KEYVAULTNAME
ENV Azure_Key_Vault_Name ""
RUN echo "Environment variable Azure_Key_Vault_Name is set to $Azure_Key_Vault_Name"
WORKDIR /app

# Copy everything
COPY . ./

RUN dotnet restore 
RUN dotnet publish -c Release -o out 

#Build runtime image 
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
EXPOSE 80
COPY --from=build /app/out .
ENTRYPOINT [ "dotnet","SecureAPI.dll"]

# docker build -t luisenalvar/azsecureapi --build-arg KEYVAULTNAME=[value] .
# docker run --name test_azsecureapi -e "Azure_Key_Vault_Name=" luisenalvar/azsecureapi -p 8080:80 -d
# docker container inspect test_azsecureapi