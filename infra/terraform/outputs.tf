output "api_url" {
  description = "URL de la API desplegada"
  value       = "https://${azurerm_linux_web_app.api.default_hostname}"
}

output "frontend_url" {
  description = "URL del Frontend desplegado"
  value       = "https://${azurerm_linux_web_app.frontend.default_hostname}"
}

output "sql_server_fqdn" {
  description = "FQDN del servidor SQL"
  value       = azurerm_mssql_server.main.fully_qualified_domain_name
}

output "database_name" {
  description = "Nombre de la base de datos"
  value       = azurerm_mssql_database.main.name
}

output "resource_group" {
  description = "Resource Group"
  value       = azurerm_resource_group.main.name
}
