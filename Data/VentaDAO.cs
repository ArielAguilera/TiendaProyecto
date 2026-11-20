using Npgsql;
using TiendaProyecto.Models;

namespace TiendaProyecto.Data
{
    public class VentaDAO
    {
        public int RegistrarVenta(Venta venta)
        {
            // Defensa en la capa de datos (la interfaz ya valida, pero el DAO no debe confiar)
            if (venta.Detalles.Count == 0)
                throw new InvalidOperationException("La venta no tiene artículos.");
            if (venta.Detalles.Any(d => d.Cantidad <= 0))
                throw new InvalidOperationException("Hay artículos con cantidad inválida.");

            using var conexion = new NpgsqlConnection(Conexion.Cadena);
            conexion.Open();

            // Inicia la transacción: todo se confirma junto o nada se guarda
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // 1. Insertar la cabecera y recuperar el ID generado (RETURNING id)
                const string sqlCabecera =
                    "INSERT INTO ventas (fecha, cliente, total) VALUES (@fecha, @cliente, @total) RETURNING id";
                int idVentaGenerado;

                using (var cmdCab = new NpgsqlCommand(sqlCabecera, conexion, transaccion))
                {
                    cmdCab.Parameters.AddWithValue("fecha", venta.Fecha);
                    cmdCab.Parameters.AddWithValue("cliente", venta.Cliente);
                    cmdCab.Parameters.AddWithValue("total", venta.Total);
                    idVentaGenerado = Convert.ToInt32(cmdCab.ExecuteScalar());
                }

                // 2. Insertar cada línea del detalle ligada al ID de la cabecera
                const string sqlDetalle = @"INSERT INTO detalle_ventas
                        (id_venta, id_producto, nombre_producto, cantidad, precio_unitario, subtotal)
                    VALUES (@idVenta, @idProd, @nomProd, @cant, @precio, @sub)";

                foreach (var item in venta.Detalles)
                {
                    using var cmdDet = new NpgsqlCommand(sqlDetalle, conexion, transaccion);
                    cmdDet.Parameters.AddWithValue("idVenta", idVentaGenerado);
                    cmdDet.Parameters.AddWithValue("idProd", item.IdProducto);
                    cmdDet.Parameters.AddWithValue("nomProd", item.NombreProducto);
                    cmdDet.Parameters.AddWithValue("cant", item.Cantidad);
                    cmdDet.Parameters.AddWithValue("precio", item.PrecioUnitario);
                    cmdDet.Parameters.AddWithValue("sub", item.Subtotal);
                    cmdDet.ExecuteNonQuery();
                }

                // 3. Todo salió bien: confirmar
                transaccion.Commit();
                venta.Id = idVentaGenerado;
                return idVentaGenerado;
            }
            catch (Exception)
            {
                // Algo falló: deshace cabecera y detalles ya insertados
                transaccion.Rollback();
                throw;
            }
        }
    }
}
