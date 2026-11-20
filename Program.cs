using TiendaProyecto.Forms;

namespace TiendaProyecto
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmVentas());
        }
    }
}
