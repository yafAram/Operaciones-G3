# G3Operaciones API

<div align="center">

### API backend para gestión operativa de pedidos, usuarios, clientes, inventario, precios y órdenes

![C#](https://img.shields.io/badge/C%23-Backend-68217A?style=for-the-badge\&logo=csharp\&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-.NET_10-512BD4?style=for-the-badge\&logo=dotnet\&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge\&logo=microsoftsqlserver\&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity_Framework_Core-ORM-512BD4?style=for-the-badge\&logo=dotnet\&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge\&logo=jsonwebtokens\&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge\&logo=docker\&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=for-the-badge\&logo=swagger\&logoColor=000)

</div>

---

## Descripción general

**G3Operaciones API** es una API backend desarrollada con **ASP.NET Core**, **C#**, **SQL Server 2022** y **Entity Framework Core**, orientada a la gestión operativa de pedidos, inventarios, clientes, productos, precios, usuarios y roles.

El proyecto fue construido como un MVP backend para una plataforma de operaciones empresariales, aplicando principios de **Clean Architecture**, separación por capas, autenticación con **JWT**, autorización basada en roles y documentación de endpoints mediante **Swagger/OpenAPI**.

El objetivo principal es proporcionar una base backend mantenible, segura y preparada para crecer hacia módulos operativos más avanzados como dashboards, integración móvil, generación de reportes y trazabilidad completa de pedidos.

---

## Objetivo del proyecto

El sistema busca centralizar y controlar procesos operativos relacionados con:

* Gestión de usuarios y roles.
* Administración de clientes y cadenas comerciales.
* Registro y control de productos.
* Gestión de almacenes e inventarios.
* Configuración de precios.
* Creación, seguimiento y modificación de pedidos.
* Historial de cambios y trazabilidad operativa.
* Seguridad mediante autenticación y autorización.

---

## Stack tecnológico

### Backend

* C#
* ASP.NET Core
* .NET 10
* Entity Framework Core
* APIs REST
* Swagger / OpenAPI

### Base de datos

* SQL Server 2022
* Migraciones con Entity Framework Core
* Fluent API
* Relaciones explícitas
* Índices estratégicos
* Conversión de enums

### Seguridad

* JWT Authentication
* Role-Based Access Control
* Claims personalizados
* Políticas de autorización por rol
* Middleware global de excepciones

### Infraestructura y herramientas

* Docker
* Docker Compose
* Git
* GitHub
* Visual Studio
* Postman
* SQL Server Management Studio

---

## Arquitectura

El proyecto aplica una estructura basada en **Clean Architecture**, organizada por capas para separar responsabilidades y mantener el dominio independiente de detalles técnicos.

```txt
API.G3Operaciones.Garcias.MX/
│
├── Controllers/
│   └── Endpoints REST y validaciones de entrada
│
├── Domain/
│   └── Entities/
│       ├── Users/
│       ├── Clients/
│       ├── Products/
│       ├── Inventory/
│       ├── Pricing/
│       └── Orders/
│
├── Application/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Common/
│   └── Casos de uso y reglas de aplicación
│
├── Infrastructure/
│   ├── Persistence/
│   ├── Configurations/
│   └── Implementación de persistencia e integraciones
│
├── Middlewares/
│   └── Manejo global de excepciones
│
├── Migrations/
│   └── Migraciones de Entity Framework Core
│
├── Program.cs
└── appsettings.json
```

---

## Responsabilidad de cada capa

### Domain

Contiene la lógica central del negocio:

* Entidades.
* Enums.
* Reglas de negocio.
* Modelos principales del dominio.

Esta capa no depende directamente de frameworks externos.

### Application

Orquesta los casos de uso del sistema:

* DTOs.
* Interfaces.
* Validaciones.
* Excepciones personalizadas.
* Contratos entre dominio e infraestructura.

### Infrastructure

Implementa detalles técnicos:

* Entity Framework Core.
* DbContext.
* Configuraciones Fluent API.
* Persistencia.
* Implementaciones de repositorios o servicios externos.

### API / Controllers

Expone la funcionalidad mediante endpoints REST:

* Controladores.
* Validaciones de entrada.
* Respuestas HTTP.
* Autenticación y autorización.
* Documentación con Swagger.

---

## Módulos implementados

### Users

Módulo encargado de la gestión de usuarios, roles y credenciales.

Incluye:

* Registro y administración de usuarios.
* Manejo de credenciales.
* Asignación de roles.
* Base para autenticación y autorización.
* Auditoría de acciones.

---

### Clients

Módulo orientado a la administración de clientes y cadenas comerciales.

Incluye:

* Clientes individuales.
* Cadenas comerciales.
* Relación con pedidos.
* Asociación con operadores y sucursales.

---

### Products

Módulo encargado de la gestión de productos.

Incluye:

* Identificador de producto.
* Nombre.
* Estado activo/inactivo.
* Preparación para integración con inventarios y precios.

---

### Inventory

Módulo para control de existencias y almacenes.

Incluye:

* Almacenes.
* Existencias por producto.
* Movimientos históricos de inventario.
* Registro de entradas y salidas.
* Base para validaciones previas a pedidos.

---

### Pricing

Módulo para configuración flexible de precios.

Incluye:

* Precio general.
* Precio por cliente.
* Precio por cadena.
* Preparación para reglas de precios dinámicas.

---

### Orders

Módulo principal del sistema, enfocado en la creación, seguimiento y trazabilidad de pedidos.

Incluye:

* Creación de pedidos.
* Asignación de operador.
* Asociación con sucursal o cadena.
* Estados del pedido.
* Total del pedido.
* Coordenadas de ubicación.
* Snapshot de nombre de sucursal.
* Fecha de creación y cierre.
* Items del pedido.
* Historial de modificaciones.
* Registro de llamadas operativas.
* Configuración de horarios y ventanas de atención.

---

## Características técnicas destacadas

* Arquitectura limpia aplicada por capas.
* API REST desarrollada con ASP.NET Core.
* Persistencia con Entity Framework Core.
* Configuración de entidades mediante Fluent API.
* Migraciones estructurales.
* SQL Server 2022 como motor de base de datos.
* Docker para entorno reproducible.
* JWT personalizado para autenticación.
* Autorización basada en políticas por rol.
* Middleware global para manejo de excepciones.
* Swagger configurado para pruebas de endpoints.
* Módulo de pedidos con historial y trazabilidad.
* Preparado para testing con xUnit, Moq y WebApplicationFactory.

---

## Flujo general del sistema

```txt
Usuario inicia sesión
        ↓
Se genera token JWT
        ↓
Se valida acceso según rol
        ↓
El operador consulta asignaciones activas
        ↓
Se valida día o ventana permitida
        ↓
Se crea un pedido
        ↓
Se agregan productos al pedido
        ↓
Se registra historial de cambios
        ↓
El pedido puede modificarse, cerrarse o cancelarse
        ↓
Se conserva trazabilidad operativa
```

---

## Base de datos

El proyecto utiliza **SQL Server 2022** y **Entity Framework Core**.

Características implementadas:

* DbContext centralizado.
* Configuración individual por entidad.
* Migraciones estructurales.
* Relaciones explícitas.
* Índices estratégicos.
* Precisión decimal controlada.
* Conversión de enums.
* Eliminación en cascada donde corresponde.

---

## Seguridad

La seguridad se implementa mediante **JWT** y políticas de autorización por rol.

### Autenticación

* Generación de token JWT.
* Expiración configurable.
* Claims personalizados.
* Validación de credenciales.
* Protección de endpoints privados.

### Autorización

* Acceso basado en roles.
* Políticas centralizadas.
* Uso de atributos de autorización en controladores.

Ejemplo:

```csharp
[Authorize(Policy = "Administrador")]
```

---

## Docker

El proyecto está preparado para trabajar con SQL Server en contenedor mediante Docker.

Beneficios:

* Entorno reproducible.
* Onboarding rápido.
* Aislamiento de dependencias.
* Persistencia mediante volúmenes.
* Configuración local más limpia.

Comando sugerido:

```bash
docker compose up -d
```

---

## Requisitos previos

Antes de ejecutar el proyecto, asegúrate de tener instalado:

* .NET SDK 10
* Docker Desktop
* SQL Server Management Studio, opcional
* Visual Studio 2022 o VS Code
* Git
* Postman, opcional

---

## Instalación y ejecución

### 1. Clonar el repositorio

```bash
git clone https://github.com/yafAram/Operaciones-G3.git
```

### 2. Entrar al proyecto

```bash
cd Operaciones-G3/API.G3Operaciones.Garcias.MX
```

### 3. Restaurar dependencias

```bash
dotnet restore
```

### 4. Levantar base de datos con Docker

```bash
docker compose up -d
```

> Si el archivo `docker-compose.yml` está en otra carpeta, ejecuta el comando desde la ubicación correspondiente.

### 5. Configurar cadena de conexión

Configura tu `appsettings.Development.json` o variables de entorno.

Ejemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=G3OperacionesDb;User Id=sa;Password=TuPasswordSeguro123;TrustServerCertificate=True;"
  }
}
```

> No subas credenciales reales al repositorio.

### 6. Aplicar migraciones

```bash
dotnet ef database update
```

### 7. Ejecutar la API

```bash
dotnet run
```

### 8. Abrir Swagger

```txt
https://localhost:PUERTO/swagger
```

---

## Endpoints principales

> Los endpoints exactos pueden variar según los controladores implementados.

### Autenticación y usuarios

```http
POST /api/auth/login
POST /api/users
GET  /api/users
GET  /api/users/{id}
PUT  /api/users/{id}
```

### Clientes

```http
GET  /api/clients
POST /api/clients
GET  /api/clients/{id}
PUT  /api/clients/{id}
```

### Productos

```http
GET  /api/products
POST /api/products
GET  /api/products/{id}
PUT  /api/products/{id}
```

### Inventario

```http
GET  /api/warehouses
POST /api/warehouses
GET  /api/inventory
POST /api/inventory-movements
```

### Pedidos

```http
GET  /api/orders
POST /api/orders
GET  /api/orders/{id}
PUT  /api/orders/{id}
POST /api/orders/{id}/items
POST /api/orders/{id}/cancel
```

---

## Testing

El proyecto está preparado para incorporar pruebas con:

* xUnit.
* Moq.
* WebApplicationFactory.
* Pruebas de reglas de negocio.
* Pruebas de seguridad.
* Pruebas de validaciones.
* Pruebas de casos críticos del dominio.

Comando sugerido cuando exista el proyecto de pruebas:

```bash
dotnet test
```

---

## Estado del proyecto

Estado actual: **MVP backend en desarrollo avanzado**.

El proyecto cuenta con:

* Núcleo estructurado.
* Separación por capas.
* Base de datos configurada.
* Migraciones.
* Seguridad con JWT.
* Políticas por rol.
* Swagger.
* Módulos principales del dominio.
* Módulo Orders con trazabilidad operativa.

---

## Próximas mejoras

* Agregar proyecto de pruebas automatizadas.
* Documentar endpoints con ejemplos de request/response.
* Agregar seeders para datos iniciales.
* Implementar logging estructurado.
* Agregar manejo avanzado de auditoría.
* Preparar despliegue en entorno cloud.
* Crear dashboard frontend.
* Integrar generación de PDF para pedidos.
* Optimizar consultas críticas.
* Agregar paginación, filtros y ordenamiento en endpoints principales.

---

## Buenas prácticas aplicadas

* Separación por capas.
* Dominio independiente.
* DTOs para entrada y salida de datos.
* Configuración Fluent API.
* Middleware global de errores.
* Autenticación con JWT.
* Autorización por roles.
* Uso de migraciones.
* Documentación con Swagger.
* Configuración reproducible con Docker.

---

## Notas para revisión técnica

Este proyecto demuestra práctica en:

* Desarrollo backend con ASP.NET Core.
* Diseño de APIs REST.
* Modelado de dominio.
* Entity Framework Core.
* SQL Server.
* Seguridad con JWT.
* Autorización basada en roles.
* Clean Architecture.
* Docker.
* Documentación técnica.

---

## Autor

**Yafte Aram Mercado Meza**
Junior Backend Developer

* GitHub: [github.com/yafAram](https://github.com/yafAram)
* Email: [yaftearam34@gmail.com](mailto:yaftearam34@gmail.com)


---

## Licencia

Este proyecto fue desarrollado como MVP académico/profesional para fines de aprendizaje, práctica técnica y demostración de habilidades backend.

---

<div align="center">

### Base sólida primero. Escalabilidad después. Control del dominio siempre.

</div>
