-- PRUEBA 2: Verificación de integridad

-- 1) Última venta (cabecera)
SELECT * FROM ventas ORDER BY id DESC LIMIT 1;

-- 2) Sus líneas de detalle (reemplacen 1 por el id obtenido arriba)
SELECT * FROM detalle_ventas WHERE id_venta = 1;

-- 3) Comparación automática: la suma de subtotales debe ser igual al total
SELECT v.id,
       v.total                 AS total_cabecera,
       SUM(d.subtotal)         AS suma_detalle,
       v.total = SUM(d.subtotal) AS coincide
FROM ventas v
JOIN detalle_ventas d ON d.id_venta = v.id
GROUP BY v.id, v.total
ORDER BY v.id DESC
LIMIT 1;
