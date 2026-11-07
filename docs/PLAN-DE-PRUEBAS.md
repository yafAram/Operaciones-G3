# Estrategia de pruebas

## Estado actual

La suite inicial se encuentra en `tests/G3Operaciones.Tests` y utiliza xUnit. Actualmente cubre reglas de dominio para pedidos y paradas de entrega. No utiliza base de datos ni realiza pruebas HTTP; por tanto, no representa todavía una suite completa de integración.

## Ejecutar

Desde la raíz del repositorio:

```bash
dotnet test tests/G3Operaciones.Tests/G3Operaciones.Tests.csproj --configuration Release
```

## Cobertura inicial

### Pedidos

- Estado inicial pendiente.
- Cálculo del total al agregar partidas.
- No permitir finalizar un pedido vacío.
- No permitir cancelar un pedido entregado.

### Entregas

- Estado inicial pendiente.
- Marcar una parada como entregada y registrar fecha de finalización.
- Marcar una parada como fallida y conservar el motivo.

## Cobertura prioritaria pendiente

1. Precedencia de precios: cliente, cadena y precio general.
2. Stock insuficiente y movimientos por almacén.
3. Edición y cancelación de pedido con restitución de existencias.
4. Pedidos simultáneos que compiten por el mismo inventario.
5. Autorización: un operador no puede suplantar a otro.
6. Acceso a planes y paradas de otro repartidor.
7. Ciclo de vida de planes confirmados y objetivos duplicados.
8. Contratos HTTP y respuestas de error.

## Tipos de pruebas recomendados

- **Unitarias:** entidades, reglas y servicios aislados.
- **Integración:** EF Core con SQL Server de prueba y transacciones reales.
- **API:** autenticación, autorización, validación de DTOs y códigos HTTP.
- **Concurrencia:** dos operaciones simultáneas sobre las mismas existencias.

La CI debe reportar el resultado real de las pruebas. No se deben marcar escenarios pendientes como cubiertos.
