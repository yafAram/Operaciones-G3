# G3 Operaciones API

API REST para administrar procesos operativos de una empresa: clientes y cadenas, usuarios y roles, productos, almacenes, inventario, precios, pedidos y planificación de entregas.

El repositorio presenta un MVP backend construido con C# y ASP.NET Core 10. Su objetivo es demostrar diseño de APIs, modelado de reglas de negocio, persistencia relacional, autenticación, autorización y trazabilidad.

> **Estado:** MVP en desarrollo. La documentación describe el código disponible en el repositorio; las pruebas automatizadas, el despliegue y la infraestructura Docker se incorporarán cuando estén implementados y verificados.

## Índice

- [Problema y alcance](#problema-y-alcance)
- [Capacidades del sistema](#capacidades-del-sistema)
- [Tecnologías](#tecnologías)
- [Arquitectura y estructura](#arquitectura-y-estructura)
- [Flujos de negocio](#flujos-de-negocio)
- [Seguridad](#seguridad)
- [Requisitos](#requisitos)
- [Configuración local](#configuración-local)
- [Base de datos y migraciones](#base-de-datos-y-migraciones)
- [Ejecución y exploración de la API](#ejecución-y-exploración-de-la-api)
- [Calidad y CI](#calidad-y-ci)
- [Documentación adicional](#documentación-adicional)
- [Limitaciones conocidas y próximos pasos](#limitaciones-conocidas-y-próximos-pasos)

## Problema y alcance

La operación comercial requiere coordinar información que suele estar repartida entre usuarios, sucursales, catálogos, precios, inventarios, pedidos y entregas. G3 Operaciones propone centralizar esos procesos en una API con reglas de negocio explícitas y permisos según el rol.

El alcance actual es un MVP backend. No debe interpretarse como un servicio productivo desplegado ni como un sistema con disponibilidad, monitoreo o pruebas de carga certificadas.

## Capacidades del sistema

| Módulo | Responsabilidad |
|---|---|
| Usuarios y roles | Administración de usuarios, credenciales y permisos |
| Clientes y cadenas | Registro de clientes, sucursales y agrupaciones comerciales |
| Productos | Catálogo de productos |
| Inventario | Almacenes, existencias y movimientos |
| Precios | Configuraciones generales, por cliente y por cadena |
| Pedidos | Creación, edición, cancelación, detalle e historial operativo |
| Entregas | Planificación de rutas, paradas, confirmación e incidencias |
| Auditoría | Registro de acciones relevantes del sistema |

### Reglas de negocio relevantes

- La resolución de precios contempla configuraciones por cliente, cadena y precio general.
- La creación de pedidos valida existencias y registra movimientos de inventario.
- Los planes de entrega agrupan objetivos por repartidor y fecha.
- Los endpoints están protegidos mediante autenticación JWT y autorización por roles.

Estas reglas deben mantenerse cubiertas por pruebas de unidad e integración conforme se incorpore la suite de pruebas.

## Tecnologías

- **Lenguaje:** C#
- **Plataforma:** .NET 10 / ASP.NET Core
- **Persistencia:** Entity Framework Core
- **Base de datos:** SQL Server
- **Autenticación:** JWT Bearer
- **Validación:** FluentValidation
- **Documentación de API:** Swagger / OpenAPI
- **Control de versiones y automatización:** Git y GitHub Actions

Las dependencias se declaran en `API.G3Operaciones.Garcias.MX/API.G3Operaciones.Garcias.MX.csproj`.

## Arquitectura y estructura

El código se organiza como un monolito modular dentro de un único proyecto .NET. Separa dominio, casos de uso, infraestructura y exposición HTTP por carpetas. Es una base de organización por capas; todavía existen servicios de Application que dependen directamente de `ApplicationDbContext`, por lo que no se presenta como una Clean Architecture estricta con dependencias completamente invertidas.

```text
API.G3Operaciones.Garcias.MX/
├── Controllers/       # Endpoints HTTP y autorización
├── Application/       # DTOs, interfaces, validaciones y servicios de aplicación
├── Domain/            # Entidades y enums del negocio
├── Infrastructure/    # EF Core, configuración, seguridad e integraciones
├── Middlewares/       # Manejo transversal de solicitudes y errores
├── Migrations/        # Historial de cambios del esquema de EF Core
├── Properties/
├── Program.cs         # Composición de dependencias y pipeline HTTP
└── appsettings*.json  # Configuración no sensible
```

### Decisiones de diseño

- Un único proyecto ejecutable para reducir complejidad operativa en la etapa MVP.
- Módulos agrupados por capacidad de negocio.
- DTOs para separar contratos HTTP de entidades persistidas.
- EF Core y migraciones para evolucionar el esquema.
- JWT Bearer para identificar al usuario y políticas/roles para controlar el acceso.

## Flujos de negocio

### Creación de pedido

1. El cliente HTTP envía los datos del pedido.
2. La API valida el modelo y las reglas de aplicación.
3. Se consulta la disponibilidad de inventario.
4. Se resuelve el precio aplicable al producto y al cliente/cadena.
5. Se crea el pedido y sus partidas.
6. Se registra la salida de inventario y la auditoría.
7. Se confirma la transacción.

> La consistencia ante pedidos simultáneos y la preservación del almacén al editar o cancelar son puntos de revisión pendientes; no se consideran resueltos hasta añadir pruebas de concurrencia e integración.

### Planificación de entregas

1. Se crea un plan asociado a un repartidor y una fecha.
2. Se agregan clientes o cadenas como objetivos.
3. Se resuelven las sucursales aplicables.
4. Se confirma el plan y se trabaja con sus paradas.
5. Se registran entregas e incidencias.

El estado operativo de una entrega debe tener una única fuente de verdad en el módulo de paradas de entrega.

## Seguridad

La API utiliza JWT Bearer y autorización por roles. La clave de firma y la cadena de conexión se suministran mediante configuración externa; no deben almacenarse en el repositorio.

Para el entorno local se recomienda .NET User Secrets. Consulta la [guía de seguridad](docs/SEGURIDAD.md).

Medidas que deben verificarse antes de cualquier despliegue:

- Rotación de credenciales que hayan estado expuestas.
- Revisión del historial Git, no solo de los archivos actuales.
- Validación de identidad y propiedad del recurso en endpoints por identificador.
- CORS restringido a los orígenes reales del frontend.
- Limitación de intentos de autenticación.
- Respuestas de error que no expongan excepciones internas.
- Configuración de secretos y permisos mínimos en cada entorno.

## Requisitos

- .NET SDK 10.
- SQL Server accesible desde el equipo.
- Git.
- Visual Studio 2022 compatible con .NET 10 o Visual Studio Code con C# Dev Kit.

No se incluye actualmente una definición Docker Compose verificada. Por ello, la base de datos debe proporcionarse por separado.

## Configuración local

### 1. Clonar el repositorio

```bash
git clone https://github.com/yafAram/Operaciones-G3.git
cd Operaciones-G3/API.G3Operaciones.Garcias.MX
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Configurar secretos locales

El proyecto tiene un identificador de User Secrets. Configura la cadena de conexión y una clave JWT nueva, aleatoria y de al menos 32 bytes:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=G3OperacionesDb;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;"
dotnet user-secrets set "JwtSettings:Secret" "REEMPLAZA_POR_UN_SECRETO_ALEATORIO_DE_AL_MENOS_32_BYTES"
```

#### Primer administrador (solo para una base de datos sin usuarios)

El seeder ya no crea cuentas predeterminadas. Para una instalación nueva, configura temporalmente los siguientes valores con datos propios y una contraseña aleatoria de al menos 16 caracteres:

```powershell
dotnet user-secrets set "BootstrapAdmin:Enabled" "true"
dotnet user-secrets set "BootstrapAdmin:Username" "TU_USUARIO_ADMIN"
dotnet user-secrets set "BootstrapAdmin:Password" "TU_PASSWORD_TEMPORAL_ALEATORIO"
dotnet user-secrets set "BootstrapAdmin:FirstName" "NOMBRE"
dotnet user-secrets set "BootstrapAdmin:LastName" "APELLIDO"
dotnet user-secrets set "BootstrapAdmin:BirthDate" "1990-01-01"
```

El administrador se crea únicamente cuando la base de datos no tiene usuarios. La credencial queda marcada para cambio de contraseña en el primer inicio de sesión. Después de confirmar que la cuenta fue creada, desactiva y elimina los secretos de bootstrap:

```powershell
dotnet user-secrets set "BootstrapAdmin:Enabled" "false"
dotnet user-secrets remove "BootstrapAdmin:Password"
```

No compartas ni publiques los valores reales. La clave JWT y la contraseña de bootstrap deben ser nuevas; nunca reutilices valores que hayan estado en el historial del repositorio.

### 4. Ejecutar migraciones

Con SQL Server disponible y la configuración correcta:

```bash
dotnet ef database update
```

Si la herramienta EF Core no está instalada:

```bash
dotnet tool install --global dotnet-ef
```

### 5. Ejecutar la API

```bash
dotnet run
```

Swagger está habilitado únicamente en el entorno Development. Consulta la URL HTTPS que muestre la consola y agrega `/swagger`.

## Base de datos y migraciones

El contexto principal es `Infrastructure/Persistence/ApplicationDbContext.cs`. Las migraciones se encuentran en `Migrations/`.

Para crear una migración después de modificar el modelo:

```bash
dotnet ef migrations add NombreDescriptivo
dotnet ef database update
```

Revisa el SQL generado y la compatibilidad de los cambios antes de aplicar migraciones en una base con información.

## Ejecución y exploración de la API

Los controladores definen las rutas y permisos efectivos. Entre los recursos principales se encuentran:

- `/api/auth` — autenticación.
- `/api/users` y `/api/roles` — usuarios y roles.
- `/api/clients` y `/api/chains` — clientes y cadenas.
- `/api/products` — productos.
- `/api/warehouses` — almacenes e inventario.
- `/api/orders` — pedidos y asignaciones de operadores.
- `/api/delivery/operator` — operaciones de repartidor.

Consulta Swagger para conocer verbos, parámetros, DTOs y respuestas reales de cada endpoint. No se publica una tabla de ejemplos ficticios ni se garantiza que rutas genéricas de otros módulos estén implementadas.

## Calidad y CI

La configuración de GitHub Actions se encuentra en `.github/workflows/`. Los flujos deben restaurar dependencias, compilar y ejecutar las pruebas disponibles. Un build exitoso no equivale a una suite de pruebas funcionales.

Cuando se agregue o amplíe la suite:

```bash
dotnet test
```

Las pruebas críticas recomendadas cubren autorización por rol y propiedad, jerarquía de precios, stock insuficiente, cancelación y edición de pedidos, transacciones, concurrencia y ciclo de vida de entregas.

## Documentación adicional

- [Arquitectura y decisiones técnicas](docs/ARQUITECTURA.md)
- [Reglas de negocio y flujos](docs/REGLAS-DE-NEGOCIO.md)
- [Plan de pruebas](docs/PLAN-DE-PRUEBAS.md)
- [Seguridad y gestión de secretos](docs/SEGURIDAD.md)
- [Guía de contribución](CONTRIBUTING.md)

## Limitaciones conocidas y próximos pasos

1. Rotar las credenciales expuestas y revisar el historial Git completo.
2. Completar pruebas unitarias y de integración con SQL Server.
3. Corregir autorización por propiedad de recursos en pedidos y entregas.
4. Garantizar atomicidad y concurrencia de movimientos de inventario.
5. Separar contratos de persistencia para reducir dependencias entre Application e Infrastructure.
6. Añadir Docker Compose reproducible, health checks y despliegue solo después de probarlos.
7. Documentar contratos HTTP con ejemplos generados o verificados desde los DTOs reales.
8. Añadir observabilidad, rate limiting y políticas CORS por entorno.

## Uso y derechos

El repositorio no incluye actualmente un archivo de licencia. Su visibilidad pública permite consultar el código, pero no concede por sí sola derechos para reutilizar, distribuir o explotar código, marcas, información o materiales pertenecientes a terceros. Antes de publicar o reutilizar componentes vinculados a una organización, confirma que cuentas con autorización.

## Autor

**Yafte Aram Mercado Meza** — Desarrollo Backend

- GitHub: [@yafAram](https://github.com/yafAram)

---

Este repositorio es una muestra de desarrollo backend y modelado de procesos operativos. La información comercial, los datos de clientes y las credenciales reales deben permanecer fuera del repositorio público.
