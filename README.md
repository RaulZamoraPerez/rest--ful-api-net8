# API Ecommerce - .NET 8

API RESTful desarrollada con ASP.NET Core 8.0 y Entity Framework Core para gestión de un sistema de ecommerce.

## 📋 Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- SQL Server (a través de Docker)

## 🚀 Configuración Inicial

### 1. Restaurar dependencias del proyecto

```bash
dotnet restore
```

Este comando descargará automáticamente todos los paquetes NuGet necesarios.

### 2. Instalar herramientas globales de Entity Framework Core

```bash
dotnet tool install --global dotnet-ef --version 8.0.0
```

> **Nota**: Si ya tienes `dotnet-ef` instalado, puedes actualizarlo con:
> ```bash
> dotnet tool update --global dotnet-ef --version 8.0.0
> ```

### 3. Levantar Base de Datos con Docker

```bash
docker-compose up -d
```

Este comando iniciará un contenedor de SQL Server en segundo plano.

### 4. Aplicar migraciones a la base de datos

```bash
dotnet ef database update
```

## 📦 Paquetes Instalados

- **Microsoft.EntityFrameworkCore.Design** v8.0.0
- **Microsoft.EntityFrameworkCore.SqlServer** v8.0.0
- **Microsoft.EntityFrameworkCore.Tools** v8.0.0
- **Swashbuckle.AspNetCore** v6.6.2

## 🗄️ Migraciones de Base de Datos

### Crear una nueva migración

```bash
dotnet ef migrations add <NombreDeLaMigracion>
```

Ejemplo:
```bash
dotnet ef migrations add InitialMigration
```

### Aplicar migraciones a la base de datos

```bash
dotnet ef database update
```

### Ver el historial de migraciones

```bash
dotnet ef migrations list
```

### Revertir una migración

```bash
dotnet ef database update <NombreDeMigracionAnterior>
```

### Eliminar la última migración (si no se ha aplicado)

```bash
dotnet ef migrations remove
```

## 🔧 Cadena de Conexión

La aplicación se conecta a SQL Server usando la siguiente configuración (ver `appsettings.json`):

```
Server=localhost,11433
Database=ApiEcommerceNET8
User ID=SA
Password=MyStrongPass123
```

> **⚠️ Importante**: En producción, asegúrate de usar contraseñas seguras y almacenarlas de forma segura (Azure Key Vault, variables de entorno, etc.).

## 🏃 Ejecutar la Aplicación

```bash
dotnet run
```

La API estará disponible en:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

## 📁 Estructura del Proyecto

```
ApiEcommerce/
├── Controllers/       # Controladores de la API
├── Data/             # Contexto de base de datos
├── Models/           # Modelos de dominio
│   └── Dtos/        # Data Transfer Objects
├── Mapping/          # Configuraciones de AutoMapper
├── Migrations/       # Migraciones de Entity Framework
├── Repository/       # Repositorios e interfaces
└── Program.cs        # Punto de entrada de la aplicación
```

## 🛠️ Comandos Útiles

### Compilar el proyecto

```bash
dotnet build
```

### Limpiar archivos compilados

```bash
dotnet clean
```

### Restaurar dependencias

```bash
dotnet restore
```

### Ver información del SDK

```bash
dotnet --info
```

## 📝 Notas Adicionales

- Asegúrate de que Docker Desktop esté en ejecución antes de usar `docker-compose`
- El puerto 11433 debe estar disponible para SQL Server
- Verifica que las herramientas de EF Core estén en tu PATH después de la instalación

---

## 📚 Referencia: Instalación de Paquetes (Solo para información)

Estos paquetes ya están configurados en el proyecto. Se instalan automáticamente con `dotnet restore`.

### Paquetes de Entity Framework Core (ya instalados en el proyecto)

```bash
# Paquete de diseño (para migraciones)
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0

# Proveedor de SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0

# Herramientas de EF Core
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
```

### Herramientas globales (cada desarrollador debe instalarlas)

```bash
# Herramienta CLI de Entity Framework (necesaria para migraciones)
dotnet tool install --global dotnet-ef --version 8.0.0
```

> ℹ️ **Diferencia importante**: 
> - Los **paquetes NuGet** se guardan en el `.csproj` y se restauran automáticamente.
> - Las **herramientas globales** deben instalarse manualmente en cada máquina de desarrollo.