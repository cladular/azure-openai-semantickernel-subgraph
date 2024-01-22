provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "this" {
  name     = "rg-${var.deployment_name}-${var.location}"
  location = var.location
}

resource "azurerm_cognitive_account" "this" {
  name                = "cog-${var.deployment_name}-${var.location}"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  kind                = "OpenAI"
  sku_name            = "S0"
}

resource "azurerm_cognitive_deployment" "this" {
  cognitive_account_id = azurerm_cognitive_account.this.id
  name                 = var.deployment_name

  model {
    format  = "OpenAI"
    name    = "gpt-4-32k"
    version = "0613"
  }

  scale {
    type     = "Standard"
    capacity = 60
  }
}
