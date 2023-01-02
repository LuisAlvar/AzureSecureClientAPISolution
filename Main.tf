variable "azure_target_zone" {
  type=string
  default = "West US 2"
  description = "Azure Availability Zone"
}

variable "imagebuildid" {
  type=string
  description = "The number for the latest build image from pipeline"
}

variable "LogAnalyticsWorkSpaceId" {
  type=string
}

variable "LogAnalyticsWorkSpaceKey" {
  type=string
}

variable "UserAssignedAzObjectId" {
  type=string
}

variable "AzureKeyVaultName" {
  type=string
}

terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "~> 3.0.2"
    }
  }
}

provider "azurerm" {
  features {}
}

terraform {
  backend "azurerm" {
    resource_group_name = "tf_api_blobstorage"
    storage_account_name = "aztfweatherblobstorage"
    container_name = "tfstatefile"
    key = "terraform.tfstate"
  }
}

resource "azurerm_resource_group" "aztf_secure_api" {
  name = "aztfsecureapirg"
  location = var.azure_target_zone
}

resource "azurerm_container_group" "aztf_secure_api_container" {
  name                = "SecureAPI"
  location            = azurerm_resource_group.aztf_secure_api.location
  resource_group_name = azurerm_resource_group.aztf_secure_api.name
  ip_address_type     = "Public"
  dns_name_label      = "archtechorgac"
  os_type             = "Linux"
  
  container {
    name = "azsecureapi"
    image = "luisenalvar/azsecureapi:${var.imagebuildid}"
    cpu = "1.0"
    memory = "1.0"
    ports {
      port = 80
      protocol = "TCP"
    }
    environment_variables = {
      KEYVAULTNAME = var.AzureKeyVaultName
    }
  }

  identity {
    type = "UserAssigned"
    #if you manually copy the id from Azure portal it contains /resourcegroups/, but the pipeline is looking for resourceGroups
    identity_ids = [var.UserAssignedAzObjectId]
  }

  diagnostics {
    log_analytics {
      workspace_id = var.LogAnalyticsWorkSpaceId
      workspace_key = var.LogAnalyticsWorkSpaceKey
    }
  }
}


