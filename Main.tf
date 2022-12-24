variable "azure_target_zone" {
  type=string
  default = "west us 2"
  description = "Azure Availability Zone"
}

variable "image_build_id" {
  type=string
  description = "The number for the latest build image from pipeline"
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
  features {
    
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
    name = "SecureAPI"
    image = "luisenalvar/azsecureapi:${var.image_build_id}"
    cpu = "1.0"
    memory = "1.0"
    ports {
      port = 80
      protocol = "TCP"
    }
  }
}