-- Actividad 7: Maestro-Detalle de Ventas
-- Base de datos: minisuper  (crearla antes si no existe:  CREATE DATABASE minisuper;)
-- Estudiante: Ariel Aguilera
CREATE TABLE IF NOT EXISTS productos (
    id     SERIAL PRIMARY KEY,
    nombre VARCHAR(100)   NOT NULL,
    precio NUMERIC(10, 2) NOT NULL CHECK (precio >= 0)
);

CREATE TABLE IF NOT EXISTS clientes (
    id     SERIAL PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL
);

-- Tablas del módulo Maestro-Detalle (relación 1 a N)
CREATE TABLE IF NOT EXISTS ventas (
    id      SERIAL PRIMARY KEY,
    fecha   TIMESTAMP      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    cliente VARCHAR(150)   NOT NULL,
    total   NUMERIC(10, 2) NOT NULL
);

CREATE TABLE IF NOT EXISTS detalle_ventas (
    id              SERIAL PRIMARY KEY,
    id_venta        INT NOT NULL REFERENCES ventas(id) ON DELETE CASCADE,
    id_producto     INT NOT NULL,            -- referencia a productos(id)
    nombre_producto VARCHAR(100)   NOT NULL,
    cantidad        INT NOT NULL CHECK (cantidad > 0),
    precio_unitario NUMERIC(10, 2) NOT NULL,
    subtotal        NUMERIC(10, 2) NOT NULL
);

-- Datos de ejemplo
INSERT INTO productos (nombre, precio)
SELECT * FROM (VALUES
    ('Arroz 1 kg',        8.50),
    ('Aceite 1 L',       14.00),
    ('Azúcar 1 kg',       7.00),
    ('Leche 1 L',         7.50),
    ('Fideo 400 g',       5.00),
    ('Gaseosa 2 L',      12.00)
) AS v(nombre, precio)
WHERE NOT EXISTS (SELECT 1 FROM productos);

INSERT INTO clientes (nombre)
SELECT * FROM (VALUES ('Consumidor Final'), ('Juan Pérez'), ('María Gómez')) AS v(nombre)
WHERE NOT EXISTS (SELECT 1 FROM clientes);
