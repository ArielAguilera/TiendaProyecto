using Npgsql;
using TiendaProyecto.Models;

namespace TiendaProyecto.Data
{
    // Lectura de catálogos para llenar los combos del formulario
    public class ProductoDAO
    {
        public List<Producto> ListarProductos()
        {
            var lista = new List<Producto>();
            using var con = new NpgsqlConnection(Conexion.Cadena);
            con.Open();
            using var cmd = new NpgsqlCommand("SELECT id, nombre, precio FROM productos ORDER BY nombre", con);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Producto
                {
                    Id = rd.GetInt32(0),
                    Nombre = rd.GetString(1),
                    Precio = rd.GetDecimal(2)
                });
            }
            return lista;
        }

        public List<string> ListarClientes()
        {
            var lista = new List<string>();
            using var con = new NpgsqlConnection(Conexion.Cadena);
            con.Open();
            using var cmd = new NpgsqlCommand("SELECT nombre FROM clientes ORDER BY (nombre <> 'Consumidor Final'), nombre", con);
            using var rd = cmd.ExecuteReader();
            while (rd.Read()) lista.Add(rd.GetString(0));
            if (lista.Count == 0) lista.Add("Consumidor Final");
            return lista;
        }
    }
}
