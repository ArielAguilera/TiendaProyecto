namespace TiendaProyecto.Data
{
    public static class Conexion
    {
        public static string Cadena =>
            Environment.GetEnvironmentVariable("TIENDA_CONN")
            ?? "Host=localhost;Port=5432;Database=minisuper;Username=postgres;Password=tu_password";
    }
}
