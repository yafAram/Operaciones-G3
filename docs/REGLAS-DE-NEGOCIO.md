# Reglas de negocio y flujos

Este documento resume los comportamientos observables en el código. Los puntos marcados como pendientes requieren pruebas antes de considerarse garantizados.

## Pedidos

- Un pedido se asocia a un operador y puede asociarse a un cliente o cadena.
- El precio de una partida se resuelve considerando configuraciones específicas y precio general.
- La creación valida disponibilidad de inventario, registra partidas y movimientos de salida.
- La edición y cancelación deben restituir existencias en el almacén original. La implementación actual requiere completar esa garantía.
- Las operaciones de inventario y pedido deben ser atómicas.

## Inventario

- Las existencias se relacionan con producto y almacén.
- Los movimientos deben conservar tipo, cantidad y referencia operativa.
- Las pruebas de concurrencia deben verificar que dos pedidos no consuman las mismas últimas unidades.

## Entregas

- Los planes se asignan a un repartidor y una fecha.
- Los objetivos pueden referirse a clientes o cadenas.
- La ejecución registra paradas y estados.
- El estado de la parada de entrega debe ser la fuente de verdad para entrega, incidencia y reprogramación.
- Un plan confirmado requiere reglas claras para impedir modificaciones incompatibles con su estado.

## Precios

La precedencia esperada es precio específico del cliente, después precio de cadena y finalmente precio general. Los casos con configuraciones simultáneas deben quedar cubiertos por pruebas.

## Seguridad de operaciones

- El identificador del usuario que ejecuta una acción debe provenir del token validado.
- Los endpoints que aceptan identificadores de usuario, plan o pedido deben comprobar permisos y propiedad.
- Los datos recibidos del cliente no sustituyen las comprobaciones del servidor.

## Casos de prueba prioritarios

1. Pedido con stock suficiente e insuficiente.
2. Dos pedidos concurrentes que compiten por las últimas unidades.
3. Edición y cancelación con restitución en el almacén original.
4. Precedencia de precios cliente/cadena/general.
5. Operador que intenta consultar dashboard de otro operador.
6. Repartidor que intenta modificar un plan que no le pertenece.
7. Confirmación de plan vacío, bloqueado o duplicado.
8. Registro de entrega e incidencia con estados consistentes.
