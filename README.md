🚀 G3Operaciones API

API backend para la plataforma G3Operaciones, desarrollada con ASP.NET Core (.NET 10), SQL Server 2022 y basada en Clean Architecture.

El sistema está orientado a la gestión operativa integral de:

Usuarios y roles

Clientes y cadenas

Productos

Inventarios y almacenes

Configuración de precios

Gestión avanzada de pedidos (Orders)

El proyecto prioriza control del dominio, trazabilidad, seguridad, mantenibilidad y escalabilidad real.

📌 Estado actual del proyecto

🟢 Fase actual: Núcleo del sistema completamente estructurado y módulos principales implementados.

Actualmente el proyecto cuenta con:

Arquitectura limpia aplicada correctamente

Separación por capas (Domain, Application, Infrastructure, API)

SQL Server 2022 configurado

Docker para entorno reproducible

Entity Framework Core con Fluent API por entidad

Migraciones estructurales listas

Seguridad con JWT implementada

Políticas de autorización por rol

Middleware global de manejo de excepciones

Swagger con autenticación configurada

Módulo completo de Orders integrado al dominio

El sistema ya no está en fase inicial.
Cuenta con una base sólida preparada para crecimiento empresarial.

🧱 Arquitectura

Se implementa Clean Architecture, organizada por capas dentro de un único proyecto para mantener simplicidad sin perder buenas prácticas.

Capas principales
Domain

Contiene:

Entidades

Enums

Reglas de negocio

Lógica central del dominio

No depende de frameworks externos.

Application

Contiene:

DTOs

Interfaces

Casos de uso

Validaciones

Excepciones personalizadas

Orquesta el comportamiento del dominio.

Infrastructure

Contiene:

Entity Framework Core

Configuraciones Fluent API

DbContext

Persistencia

Integraciones técnicas

Implementa las interfaces definidas en Application.

API (Controllers)

Contiene:

Endpoints REST

Autorización

Validaciones de entrada

Respuestas HTTP

Integración con JWT

📁 Estructura del proyecto

G3Operaciones.API

Controllers

Domain

Entities

Users

Clients

Products

Inventory

Pricing

Orders

Application

DTOs

Interfaces

Common

Infrastructure

Persistence

Configurations

ApplicationDbContext.cs

Middlewares

Filters

Configurations

Program.cs

appsettings.json

🗄️ Base de datos
Motor

SQL Server 2022 Developer Edition

ORM

Entity Framework Core

Características implementadas

DbContext centralizado

Configuración individual por entidad con IEntityTypeConfiguration

Conversión de enums a int

Índices estratégicos

Relaciones definidas explícitamente

Precisión decimal controlada

Eliminación en cascada donde corresponde

Migraciones estructurales listas

No se utiliza EnsureCreated() ni generación automática no controlada.

🐳 Docker

SQL Server corre dentro de contenedor Docker para garantizar:

Entornos reproducibles

Onboarding rápido

Aislamiento del entorno

Persistencia mediante volúmenes

Levantar base de datos
docker compose up -d

Puerto expuesto: 1433

🔐 Seguridad
Autenticación

Se implementa autenticación mediante:

JWT personalizado

Expiración configurable

Claims personalizados

No se utiliza ASP.NET Identity.

Autorización

Basada en políticas por rol.

Ejemplo:

[Authorize(Policy = "Administrador")]

Las políticas están centralizadas y configuradas en el pipeline.

📦 Módulos Implementados
👤 USERS

Permite:

Registro y gestión de usuarios

Gestión de roles

Manejo de credenciales

Auditoría de acciones

Proporciona la base para autenticación y autorización.

🏢 CLIENTS

Incluye:

Clientes individuales

Cadenas comerciales

Permite asociar pedidos y operadores a sucursales o cadenas específicas.

📦 PRODUCTS

Gestión de productos con:

Identificador

Nombre

Estado activo/inactivo

Preparado para integración con inventarios y precios dinámicos.

🏬 INVENTORY

Incluye:

Warehouses (almacenes)

Inventories (existencias por producto)

InventoryMovements (movimientos históricos)

Permite:

Control por almacén

Registro de entradas y salidas

Base para validación previa a pedidos

💰 PRICING

Configuración flexible de precios:

Precio general

Precio por cliente

Precio por cadena

Diseñado para expansión futura con reglas dinámicas.

📦 ORDERS (Módulo Completo)

Este es uno de los módulos más robustos del sistema.

Entidades principales
Order

Operador asignado

Sucursal o cadena

Estado

Total

Coordenadas (latitud/longitud)

Snapshot del nombre de sucursal

Fecha creación

Fecha cierre

OrderItem

Producto

Cantidad

Precio unitario

Subtotal

Snapshot del nombre del producto

OrderHistory

Permite trazabilidad completa:

Usuario que realizó el cambio

Qué cambió

Valor anterior

Valor nuevo

Fecha y hora

OrderCallLog

Registro operativo de llamadas:

Resultado de llamada

Fecha programada (si aplica)

Notas

OperatorAssignment

Controla:

Qué operador atiende qué sucursal o cadena

Día asignado

Activación o desactivación

Ventana de tiempo permitida

OrderScheduleConfiguration

Define:

Días de levantamiento de pedidos

Días de entrega

Ventanas horarias

Configuración activa/inactiva

Características del módulo Orders

Conversión de enums

Índices optimizados

Historial completo de modificaciones

Base para control de ventanas de 38 horas

Preparado para integración con generación de PDF y rutas

⚙️ Flujo General del Sistema

Usuario inicia sesión

Se genera JWT

Se valida acceso por rol

Operador consulta asignaciones activas

Se valida día permitido

Se genera pedido

Se registran items

Se registra historial

Se pueden modificar pedidos con trazabilidad

Se puede cancelar (desactivar) pedido

Se conserva historial completo

🔍 Swagger

Disponible en entorno de desarrollo.

Incluye:

Documentación automática

Soporte para autenticación JWT

Pruebas directas de endpoints

🧪 Testing

Preparado para:

xUnit

Moq

WebApplicationFactory

Enfoque en:

Reglas de negocio

Seguridad

Validaciones

Casos críticos del dominio

⚙️ Ejecución del proyecto
Requisitos

.NET SDK 10

Docker Desktop

SQL Server container activo

Ejecutar API
dotnet run
Crear migración
dotnet ef migrations add NombreMigracion
Aplicar migraciones
dotnet ef database update
📈 Estado actual del sistema

El proyecto evolucionó de:

Base técnica inicial
→ Arquitectura estructurada
→ Implementación de módulos funcionales
→ Seguridad completa
→ Módulo Orders avanzado

Actualmente se encuentra preparado para:

Desarrollo frontend

Dashboards operativos

Integración móvil

Escalamiento empresarial

Optimización de consultas

🧠 Principio rector

Base sólida primero.
Escalabilidad después.
Control del dominio siempre.