# Seguridad

La seguridad del sistema es una responsabilidad continua. Esta guía describe prácticas mínimas para desarrollo, revisión y despliegue.

## Reporte de vulnerabilidades

No publiques credenciales, tokens, datos personales, respaldos de bases de datos ni detalles explotables en Issues o discusiones públicas. Comunica cualquier hallazgo al responsable del repositorio mediante un canal privado y confiable.

## Política de secretos

Nunca deben versionarse:

- Contraseñas de SQL Server u otros servicios.
- Claves de firma JWT.
- API keys, tokens de acceso o refresh tokens.
- Certificados y claves privadas.
- Cadenas de conexión de entornos reales.
- Datos de clientes, usuarios o información comercial confidencial.

Para desarrollo local utiliza .NET User Secrets. Para CI/CD utiliza secretos protegidos del entorno. Para producción utiliza un gestor de secretos y permisos mínimos.

## Cómo responder ante una credencial expuesta

1. **Revoca o rota primero** la credencial en el servicio que la valida.
2. Identifica aplicaciones, entornos y usuarios que dependan de ella.
3. Sustituye el valor por un secreto nuevo y específico del entorno.
4. Revisa historial Git, forks, clones, logs, artefactos de CI y publicaciones.
5. Evalúa la reescritura del historial. Coordina con colaboradores antes de reescribir y hacer push forzado, porque clones y referencias existentes pueden conservar objetos anteriores.
6. Verifica que la credencial anterior ya no funcione.
7. Documenta la remediación sin copiar el valor expuesto.

Eliminar una clave del último commit no la invalida ni elimina automáticamente de los commits anteriores. La rotación es obligatoria.

## Configuración local con User Secrets

Desde la carpeta del proyecto API:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<cadena-local>"
dotnet user-secrets set "JwtSettings:Secret" "<clave-aleatoria-de-al-menos-32-bytes>"
```

No compartas la salida de `dotnet user-secrets list`, ya que muestra los valores almacenados.

## Alta inicial del administrador

El seeder crea roles, pero no incluye credenciales predeterminadas. En una base de datos nueva, puede crearse el primer administrador mediante configuración temporal:

```powershell
dotnet user-secrets set "BootstrapAdmin:Enabled" "true"
dotnet user-secrets set "BootstrapAdmin:Username" "<usuario>"
dotnet user-secrets set "BootstrapAdmin:Password" "<secreto-aleatorio-de-16-o-mas-caracteres>"
dotnet user-secrets set "BootstrapAdmin:FirstName" "<nombre>"
dotnet user-secrets set "BootstrapAdmin:LastName" "<apellido>"
dotnet user-secrets set "BootstrapAdmin:BirthDate" "yyyy-MM-dd"
```

La cuenta solo se crea si no hay usuarios registrados y su credencial exige cambiar la contraseña en el primer inicio de sesión. Después de confirmar el alta, desactiva `BootstrapAdmin:Enabled` y elimina el secreto de contraseña. No reutilices contraseñas que hayan sido expuestas.


## Controles de autenticación y autorización

Antes de publicar una versión:

- Obtén la identidad del usuario desde claims de un JWT validado, no desde un identificador controlado por el cliente.
- Comprueba rol y propiedad del recurso en operaciones que reciben IDs.
- Responde 401 cuando no existe autenticación válida y 403 cuando el usuario autenticado carece de permiso.
- No devuelvas stack traces, mensajes internos, rutas del servidor ni cadenas de conexión.
- Aplica rate limiting a los endpoints de autenticación.
- Limita CORS a los orígenes autorizados.
- Valida expiración, emisor, audiencia y firma del JWT.
- Mantén una única fuente de verdad para el estado de entregas.

## Revisión antes de publicar

- [ ] Las credenciales expuestas fueron rotadas.
- [ ] No existen secretos en archivos actuales.
- [ ] Se revisó el historial Git y los artefactos públicos.
- [ ] No se publican datos personales o comerciales.
- [ ] Los endpoints sensibles validan identidad, rol y propiedad.
- [ ] Los errores no filtran detalles internos.
- [ ] Las dependencias se revisaron y actualizaron según compatibilidad.
- [ ] Los flujos de CI no imprimen secretos.
