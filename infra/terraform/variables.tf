variable "resource_group_name" {
  description = "Nombre del resource group"
  type        = string
  default     = "rg-perfeck-rankings"
}

variable "location" {
  description = "Region de Azure"
  type        = string
  default     = "eastus"
}

variable "sql_admin_login" {
  description = "Login administrador de Azure SQL"
  type        = string
  sensitive   = true
}

variable "sql_admin_password" {
  description = "Password administrador de Azure SQL"
  type        = string
  sensitive   = true
}
