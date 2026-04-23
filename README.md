# 🚀 Clean Architecture Ecommerce API

API RESTful desarrollada con **ASP.NET Core 8.0** y **Entity Framework Core** implementando **Clean Architecture** para gestión de un sistema de ecommerce.

## ✨ Características Principales

- ✅ **Clean Architecture** - Separación de capas (Presentation, Application, Infrastructure, Domain)
- ✅ **SQL Server** con Docker
- ✅ **JWT Authentication** - Autenticación basada en tokens
- ✅ **ASP.NET Identity** - Gestión de usuarios y roles
- ✅ **API Versioning** - Soporte para múltiples versiones (v1, v2)
- ✅ **Swagger/OpenAPI** - Documentación interactiva
- ✅ **AutoMapper** - Mapeo de DTOs
- ✅ **Response Caching** - Optimización de performance
- ✅ **CORS** - Configurado para desarrollo y producción

## 📋 Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com)
- SQL Server (disponible vía Docker)

## 🚀 Configuración Inicial

### 1. Clonar el repositorio

```bash
git clone <tu-repo>
cd ApiEcommerce/ApiEcommerce
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Instalar herramientas de Entity Framework Core

```bash
dotnet tool install --global dotnet-ef --version 8.0.0
```

Si ya lo tienes instalado:
```bash
dotnet tool update --global dotnet-ef --version 8.0.0
```

### 4. Levantar la Base de Datos con Docker

```bash
docker-compose up -d
```

Esto inicia un contenedor con SQL Server. Espera ~30 segundos a que se inicialice completamente.

### 5. Aplicar Migraciones

```bash
dotnet ef database update
```

Este comando:
- Crea la BD `ApiEcommerceNET8` si no existe
- Aplica todas las migraciones
- Crea las tablas de Identity
- Ejecuta el seed data (usuario admin)

### 6. Ejecutar la Aplicación

```bash
dotnet run
```

La API estará disponible en:
- 🌐 **URL**: `http://localhost:5078`
- 📚 **Swagger**: `http://localhost:5078/swagger`
- 🏠 **Home**: `http://localhost:5078/index.html`

## 🗄️ Base de Datos - Docker

### Configuración de SQL Server

**Archivo**: `docker-compose.yaml`

```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports:
      - "11433:1433"
    environment:
      ACCEPT_EULA: "Y"
      SA_PASSWORD: "MyStrongPass123"
    volumes:
      - sqlserver-data:/var/opt/mssql
```

**Credenciales por defecto:**
- **Server**: `localhost,11433`
- **Database**: `ApiEcommerceNET8`
- **User**: `SA`
- **Password**: `MyStrongPass123`

### Comandos Docker útiles

```bash
# Ver logs de SQL Server
docker-compose logs -f sqlserver

# Detener contenedor
docker-compose down

# Detener y eliminar volúmenes (⚠️ elimina datos)
docker-compose down -v

# Reiniciar
docker-compose restart
```

## 📁 Estructura de Proyecto (Clean Architecture)

```
ApiEcommerce/
├── Domain/                      # Entidades de dominio
│   └── Entities/
│       ├── Category.cs
│       ├── Product.cs
│       └── User.cs
│
├── Application/                 # Lógica de aplicación (Use Cases)
│   ├── DTOs/                   # Data Transfer Objects
│   ├── Interfaces/             # Contratos
│   ├── Mapping/                # AutoMapper profiles
│   └── UseCases/               # Casos de uso
│       ├── Auth/
│       ├── Categories/
│       └── Products/
│
├── Infrastructure/              # Implementación de persistencia
│   ├── Persistence/            # EF Core
│   │   ├── CategoryRepository.cs
│   │   ├── ProductRepository.cs
│   │   └── UserRepository.cs
│   └── Services/               # Servicios externos
│       └── IdentityService.cs
│
├── Presentation/                # Controladores HTTP
│   ├── AuthCleanController.cs
│   ├── CategoriesCleanController.cs
│   └── ProductsCleanController.cs
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Migrations/                  # EF Core Migrations ⭐ IMPORTANTE
├── wwwroot/                     # Archivos estáticos
├── Program.cs                   # Configuración y punto de entrada
└── appsettings.json            # Configuración
```

## 🔐 Autenticación - JWT

### Endpoints de Autenticación

**Register - Crear usuario:**
```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "email": "usuario@example.com",
  "password": "Password123!"
}
```

**Login - Obtener token:**
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "usuario@example.com",
  "password": "Password123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": "123",
    "email": "usuario@example.com"
  }
}
```

### Usar el Token

Incluye el token en el header `Authorization`:
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## 📚 Endpoints Principales

### Productos
```http
GET    /api/v1/products           # Listar todos
GET    /api/v1/products/{id}      # Obtener por ID
POST   /api/v1/products           # Crear (requiere auth)
PUT    /api/v1/products/{id}      # Actualizar (requiere auth)
DELETE /api/v1/products/{id}      # Eliminar (requiere auth)
```

### Categorías
```http
GET    /api/v1/categories         # Listar todas
GET    /api/v1/categories/{id}    # Obtener por ID
POST   /api/v1/categories         # Crear (requiere auth)
DELETE /api/v1/categories/{id}    # Eliminar (requiere auth)
```

### Autenticación
```http
POST   /api/v1/auth/register      # Registrarse
POST   /api/v1/auth/login         # Iniciar sesión
```

> 📌 **Nota**: La API usa **versionado** - cambiar `v1` a `v2` para usar versión 2

## 🔧 Migraciones de Base de Datos

### Crear una nueva migración

```bash
dotnet ef migrations add NombreMigracion
```

Ejemplo:
```bash
dotnet ef migrations add AddFirstNameLastNameToUser
```

### Aplicar todas las migraciones pendientes

```bash
dotnet ef database update
```

### Actualizar a una migración específica

```bash
dotnet ef database update NombreMigracion
```

### Ver historial de migraciones

```bash
dotnet ef migrations list
```

### Eliminar la última migración (si no está aplicada)

```bash
dotnet ef migrations remove
```

## 🛠️ Desarrollo

### Compilar el proyecto

```bash
dotnet build
```

### Limpiar archivos compilados

```bash
dotnet clean
```

### Restaurar paquetes NuGet

```bash
dotnet restore
```

### Ver información del SDK

```bash
dotnet --info
```

## 📦 Paquetes Instalados

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| Microsoft.EntityFrameworkCore | 8.0.0 | ORM |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.0 | Proveedor SQL Server |
| Microsoft.EntityFrameworkCore.Tools | 8.0.0 | Migraciones |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.0 | JWT |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.0 | Identidad |
| Swashbuckle.AspNetCore | 6.6.2 | Swagger/OpenAPI |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | Mapeo de objetos |

## ⚙️ Configuración

### appsettings.json

```json
{
  "ConnectionStrings": {
    "ConexionSql": "Server=localhost,11433;Database=ApiEcommerceNET8;User ID=SA;Password=MyStrongPass123;"
  },
  "JWT": {
    "Issuer": "ApiEcommerce",
    "Audience": "ApiEcommerceUsers"
  },
  "ApiSettings": {
    "SecretKey": "tu-clave-secreta-muy-larga-y-segura"
  }
}
```

> ⚠️ **Importante**: En producción, usa variables de entorno o Azure Key Vault para secretos

## 🚨 Solución de Problemas

### Docker no inicia SQL Server
```bash
# Ver logs
docker-compose logs sqlserver

# Reiniciar
docker-compose down
docker-compose up -d
```

### Migraciones pendientes
```bash
# Ver estado
dotnet ef database update

# Si falla, verifica la conexión
# Asegúrate que Docker esté corriendo y SQL Server accesible
```

### Puerto 11433 ya en uso
Cambia el puerto en `docker-compose.yaml`:
```yaml
ports:
  - "11434:1433"  # Cambiar a otro puerto
```

## 📖 Comandos Rápidos

```bash
# Setup completo
dotnet restore && dotnet build && dotnet ef database update

# Desarrollo
dotnet run

# Con watch (recarga automática)
dotnet watch run

# Tests (si tienes proyecto de tests)
dotnet test
```

## 🤝 Contribución

1. Crea una rama para tu feature: `git checkout -b feature/nueva-funcionalidad`
2. Haz commit de tus cambios: `git commit -am 'Agregar nueva funcionalidad'`
3. Push a la rama: `git push origin feature/nueva-funcionalidad`
4. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo licencia MIT.

## 📞 Contacto

Clean Architecture Team
- Email: clean@architecture.com
- GitHub: [tu-usuario]

---

**Última actualización**: 22 de abril de 2026 ✅