namespace TiendaProyecto.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;           // valor por defecto: ahora
        public string Cliente { get; set; } = "Consumidor Final";
        public List<DetalleVenta> Detalles { get; set; } = new();

        // Cálculo automático del total con LINQ
        public decimal Total => Detalles.Sum(d => d.Subtotal);
    }
}
