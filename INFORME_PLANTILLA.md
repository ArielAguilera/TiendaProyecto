# CARÁTULA
**Proyecto:** Tienda / Minisuper – Módulo Registro de Ventas (Maestro-Detalle)
**Actividad:** 7 – Operación Maestro-Detalle: Registro Transaccional de Ventas
**Repositorio GitHub:** https://github.com/USUARIO/REPO
**Tag de versión:** https://github.com/USUARIO/REPO/releases/tag/v0.2-maestro-detalle  (o captura)
**Integrantes:**
1. Nombre Apellido
2. Nombre Apellido
3. Nombre Apellido
4. Nombre Apellido
**Docente / Materia / Fecha:**

---

# 1. RESUMEN TÉCNICO DEL MÓDULO

## Tablas creadas (relación 1:N)
- `ventas` (Maestro): `id` (PK serial), `fecha` (timestamp, por defecto la fecha actual), `cliente`, `total` (numeric 10,2).
- `detalle_ventas` (Detalle): `id` (PK), `id_venta` (FK → `ventas.id`, `ON DELETE CASCADE`), `id_producto`,
  `nombre_producto`, `cantidad` (CHECK > 0), `precio_unitario`, `subtotal`.
- Una venta tiene muchas líneas de detalle; cada línea pertenece a una sola venta.

## Manejo de los detalles en memoria
Los artículos se guardan en una `BindingList<DetalleVenta>` enlazada a un `DataGridView` mediante un `BindingSource`.
Al agregar o quitar un ítem se recalcula `lblTotal` usando `Venta.Total` (LINQ: `Detalles.Sum(d => d.Subtotal)`),
y `Subtotal` es una propiedad calculada (`Cantidad * PrecioUnitario`). Nada se escribe en la base de datos hasta
presionar "Confirmar Venta".

## Persistencia transaccional
`VentaDAO.RegistrarVenta` abre una `NpgsqlTransaction`: inserta la cabecera (`RETURNING id`), luego cada detalle con ese id,
y hace `Commit()`. Si cualquier paso falla, `Rollback()` deshace todo, evitando ventas huérfanas.

## Validaciones
No se permite confirmar una venta sin artículos ni agregar cantidades ≤ 0; se muestran mensajes constructivos sin cerrar la aplicación.

---

# 2. EVIDENCIAS
**Captura 1 (Prueba 1):** [interfaz con cabecera, 3+ productos, total y mensaje de confirmación]
**Captura 2 (Prueba 2):** [consulta SQL de `ventas` y `detalle_ventas`, con suma = total]
**Captura 3 (Prueba 3):** [mensaje de venta vacía / cantidad inválida]
**Captura 4 (GitHub):** [historial de commits y/o tag v0.2-maestro-detalle]
