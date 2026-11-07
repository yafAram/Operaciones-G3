# Guía de contribución

Esta guía define cómo organizar cambios en G3 Operaciones, mantener commits trazables y validar cada modificación antes de integrarla.

## Ramas

Usa ramas cortas y enfocadas, creadas desde la versión más reciente de `main`:

| Prefijo | Uso | Ejemplo |
|---|---|---|
| `feat/` | Nueva funcionalidad | `feat/order-export` |
| `fix/` | Corrección de defectos | `fix/order-stock-reversal` |
| `security/` | Correcciones de seguridad | `security/remove-exposed-secrets` |
| `docs/` | Documentación | `docs/api-setup-guide` |
| `test/` | Pruebas automatizadas | `test/order-service-tests` |
| `chore/` | Mantenimiento e infraestructura | `chore/ci-workflows` |

No reutilices una rama para cambios sin relación. Actualiza la rama con `main` antes de integrar.

## Convención de commits

Usa mensajes breves, en modo imperativo y con prefijos consistentes:

- `feat:` funcionalidad nueva.
- `fix:` corrección de un defecto.
- `security:` endurecimiento o corrección de seguridad.
- `test:` pruebas automatizadas.
- `docs:` documentación.
- `refactor:` reestructuración sin cambio funcional intencional.
- `chore:` mantenimiento.

Ejemplos:

```text
fix: restore stock to the original warehouse when cancelling an order
test: cover price resolution precedence
docs: describe local database setup
```

No incluyas secretos, datos personales ni archivos generados en un commit.

## Pull requests y revisión

Aunque los cambios pequeños puedan prepararse en ramas independientes, la integración a `main` debe ser revisable. Toda solicitud de integración debe explicar:

1. Problema que resuelve y comportamiento esperado.
2. Archivos y módulos afectados.
3. Pruebas ejecutadas y resultado real.
4. Impacto en configuración, datos o migraciones.
5. Consideraciones de seguridad y reversión.
6. Riesgos conocidos o trabajo pendiente.

No afirmes que una prueba pasó si no se ejecutó. No integres cambios con verificaciones obligatorias fallidas.

## Validación local

Desde la carpeta del proyecto API:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test
```

Si todavía no existe un proyecto de pruebas, indícalo explícitamente. La compilación por sí sola no demuestra que las reglas de negocio funcionen.

## Configuración y secretos

Usa .NET User Secrets para desarrollo local y secretos de entorno para CI/CD y despliegues. Nunca guardes contraseñas, claves JWT, tokens, certificados ni cadenas de conexión reales en `appsettings.json`, archivos `.env` o documentación.

Consulta la [Política de seguridad](docs/SEGURIDAD.md).

## Calidad del cambio

- Mantén el alcance de cada rama pequeño.
- Conserva las convenciones y carpetas existentes.
- Evita introducir dependencias sin una necesidad clara.
- Actualiza la documentación cuando cambie el comportamiento.
- Añade pruebas para reglas nuevas o corregidas.
- No subas carpetas `bin/`, `obj/`, logs ni archivos locales.
