# 🚀 G3Operaciones API

API backend para la plataforma **G3Operaciones**, desarrollada con **ASP.NET (Core 10)**, **SQL Server** y **Clean Architecture**, orientada a escalabilidad, mantenibilidad y control total del dominio del negocio.

---

## 📌 Estado actual del proyecto

🟢 **Fase:** Base técnica / Infraestructura inicial

Actualmente el proyecto cuenta con:

- Arquitectura limpia definida y aplicada
- API funcional levantando correctamente
- Swagger / OpenAPI configurado
- Conexión a SQL Server configurada
- SQL Server contenerizado con Docker
- Migraciones preparadas (no ejecutadas)
- Documentación técnica activa

No se han creado entidades ni tablas aún (esto es intencional).

---

## 🧱 Arquitectura

Se utiliza **Clean Architecture**, implementada **por carpetas dentro de un solo proyecto**, evitando sobreingeniería sin sacrificar buenas prácticas.

### Principios clave

- Separación de responsabilidades
- Bajo acoplamiento
- Alta cohesión
- Independencia de frameworks
- Código testeable y escalable

---

## 📁 Estructura del proyecto

```
G3Operaciones.API
│
├── Controllers        # Endpoints REST
├── Domain             # Entidades y reglas de negocio
├── Application        # Casos de uso, DTOs, interfaces
├── Infrastructure     # EF Core, DB, servicios técnicos
│   └── Persistence
├── Middlewares        # Middlewares personalizados
├── Filters            # Filtros globales
├── Configurations     # Configuraciones centralizadas
├── Program.cs
└── appsettings.json
```

---

## 🗄️ Base de datos

### Motor

- **SQL Server 2022 – Developer Edition**

### ORM

- **Entity Framework Core**

### Estado actual

- Conexión configurada
- DbContext registrado
- Sin migraciones ejecutadas
- Sin creación automática de tablas

Esto garantiza control total del esquema cuando el dominio esté definido.

---

## 🐳 Docker

SQL Server corre en un contenedor Docker para garantizar:

- Entornos reproducibles
- Onboarding rápido
- Eliminación de dependencias locales

### docker-compose.yml (SQL Server)

- Puerto: `1433`
- Usuario: `sa`
- Persistencia con volúmenes
- Red dedicada

El contenedor acepta **conexiones remotas**.

---

## 🔐 Seguridad (estado actual)

- JWT aún no implementado
- Autorización preparada en pipeline
- Se implementará autenticación propia (no Identity)

---

## 📦 Paquetes NuGet principales

- Entity Framework Core
- SQL Server Provider
- Swagger (Swashbuckle)
- FluentValidation
- AutoMapper
- Serilog
- Polly
- xUnit / Moq (testing)

Todos seleccionados por estabilidad y compatibilidad con .NET moderno.

---

## ⚙️ Configuración de ejecución

### Requisitos

- .NET SDK 10
- Docker Desktop

### Levantar SQL Server

```bash
docker compose up -d
```

### Ejecutar la API

```bash
dotnet run
```

Swagger disponible en entorno de desarrollo.

---

## 🔍 Validación de conexión a BD

La conectividad se valida mediante:

- `Database.CanConnect()`
- Endpoints de health (solo DEV)

No se utilizan:
- EnsureCreated()
- Migraciones automáticas forzadas

---

## 🧪 Testing (planificado)

- Pruebas unitarias por capa
- Pruebas de integración con WebApplicationFactory
- Cobertura enfocada en reglas de negocio

---

## 📖 Documentación

Existe un **manual técnico vivo**, donde se documenta:

- Decisiones arquitectónicas
- Infraestructura
- Procesos
- Evolución del sistema

Este README es un resumen operativo.

---

## 🧭 Próximos pasos

1. Definir entidades del dominio
2. Configurar DbSets y Fluent API
3. Crear primera migración
4. Implementar autenticación JWT
5. Definir repositorios

---

## 🧠 Filosofía del proyecto

> "Primero una base sólida, luego velocidad."

G3Operaciones prioriza calidad técnica, control del dominio y crecimiento sostenible desde el día uno.
