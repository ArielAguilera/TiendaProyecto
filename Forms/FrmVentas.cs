using System.ComponentModel;
using TiendaProyecto.Data;
using TiendaProyecto.Models;

namespace TiendaProyecto.Forms
{
    //Actividad 7. Desarrollo de un proceso Maestro Detalle(Ariel Aguilera)
    public class FrmVentas : Form
    {
        // ---- Estado en memoria ----
        private Venta _ventaActual = new();
        private readonly BindingList<DetalleVenta> _detalles = new();
        private readonly BindingSource _bs = new();
        private readonly VentaDAO _ventaDao = new();
        private readonly ProductoDAO _productoDao = new();

        // ---- Controles ----
        private readonly DateTimePicker dtpFecha = new();
        private readonly ComboBox cboCliente = new();
        private readonly ComboBox cboProducto = new();
        private readonly NumericUpDown nudCantidad = new();
        private readonly TextBox txtPrecio = new();
        private readonly Button btnAgregar = new();
        private readonly Button btnQuitar = new();
        private readonly Button btnConfirmar = new();
        private readonly Button btnNueva = new();
        private readonly DataGridView dgv = new();
        private readonly Label lblTotal = new();

        public FrmVentas()
        {
            Text = "Registro de Ventas (Maestro-Detalle)";
            Size = new Size(820, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);

            ConstruirInterfaz();
            Load += FrmVentas_Load;
        }

        // Construcción de la interfaz
        private void ConstruirInterfaz()
        {
            // --- CABECERA (Maestro) ---
            var gbCab = new GroupBox { Text = "Cabecera de venta", Left = 12, Top = 8, Width = 780, Height = 70 };
            gbCab.Controls.Add(new Label { Text = "Fecha:", Left = 12, Top = 30, AutoSize = true });
            dtpFecha.Left = 65; dtpFecha.Top = 26; dtpFecha.Width = 200;
            dtpFecha.Format = DateTimePickerFormat.Custom;
            dtpFecha.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpFecha.Enabled = false; // fecha y hora actuales por defecto
            gbCab.Controls.Add(dtpFecha);

            gbCab.Controls.Add(new Label { Text = "Cliente:", Left = 290, Top = 30, AutoSize = true });
            cboCliente.Left = 355; cboCliente.Top = 26; cboCliente.Width = 400;
            cboCliente.DropDownStyle = ComboBoxStyle.DropDown; // permite escribir o elegir
            gbCab.Controls.Add(cboCliente);
            Controls.Add(gbCab);

            // --- AGREGAR ARTÍCULO ---
            var gbItem = new GroupBox { Text = "Agregar artículo", Left = 12, Top = 84, Width = 780, Height = 70 };
            gbItem.Controls.Add(new Label { Text = "Producto:", Left = 12, Top = 30, AutoSize = true });
            cboProducto.Left = 85; cboProducto.Top = 26; cboProducto.Width = 270;
            cboProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProducto.SelectedIndexChanged += (s, e) => MostrarPrecioSeleccionado();
            gbItem.Controls.Add(cboProducto);

            gbItem.Controls.Add(new Label { Text = "Cant.:", Left = 370, Top = 30, AutoSize = true });
            nudCantidad.Left = 420; nudCantidad.Top = 26; nudCantidad.Width = 70;
            nudCantidad.Minimum = -1000;   // permitido a propósito: la validación la hace el código (Prueba 3)
            nudCantidad.Maximum = 100000;
            nudCantidad.Value = 1;
            gbItem.Controls.Add(nudCantidad);

            gbItem.Controls.Add(new Label { Text = "P. Unit.:", Left = 505, Top = 30, AutoSize = true });
            txtPrecio.Left = 570; txtPrecio.Top = 26; txtPrecio.Width = 70; txtPrecio.ReadOnly = true;
            gbItem.Controls.Add(txtPrecio);

            btnAgregar.Text = "Agregar"; btnAgregar.Left = 655; btnAgregar.Top = 24; btnAgregar.Width = 110;
            btnAgregar.Click += BtnAgregar_Click;
            gbItem.Controls.Add(btnAgregar);
            Controls.Add(gbItem);

            // --- DETALLE (en memoria) ---
            _bs.DataSource = _detalles;
            dgv.Left = 12; dgv.Top = 162; dgv.Width = 780; dgv.Height = 280;
            dgv.AutoGenerateColumns = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Columns.Add(Col(nameof(DetalleVenta.NombreProducto), "Producto", 40));
            dgv.Columns.Add(Col(nameof(DetalleVenta.Cantidad), "Cantidad", 15));
            dgv.Columns.Add(Col(nameof(DetalleVenta.PrecioUnitario), "Precio Unit.", 20, "N2"));
            dgv.Columns.Add(Col(nameof(DetalleVenta.Subtotal), "Subtotal", 25, "N2"));
            dgv.DataSource = _bs;
            Controls.Add(dgv);

            // --- PIE: botones + total ---
            btnQuitar.Text = "Quitar seleccionado"; btnQuitar.Left = 12; btnQuitar.Top = 455; btnQuitar.Width = 180;
            btnQuitar.Click += BtnQuitar_Click;
            Controls.Add(btnQuitar);

            lblTotal.Left = 420; lblTotal.Top = 450; lblTotal.Width = 372; lblTotal.Height = 40;
            lblTotal.TextAlign = ContentAlignment.MiddleRight;
            lblTotal.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            Controls.Add(lblTotal);

            btnNueva.Text = "Nueva venta"; btnNueva.Left = 440; btnNueva.Top = 505; btnNueva.Width = 160; btnNueva.Height = 36;
            btnNueva.Click += (s, e) => NuevaVenta();
            Controls.Add(btnNueva);

            btnConfirmar.Text = "Aceptar / Confirmar Venta";
            btnConfirmar.Left = 610; btnConfirmar.Top = 505; btnConfirmar.Width = 182; btnConfirmar.Height = 36;
            btnConfirmar.BackColor = Color.SeaGreen; btnConfirmar.ForeColor = Color.White;
            btnConfirmar.Click += BtnConfirmar_Click;
            Controls.Add(btnConfirmar);
        }

        private static DataGridViewTextBoxColumn Col(string prop, string header, float peso, string? formato = null)
        {
            var c = new DataGridViewTextBoxColumn
            {
                DataPropertyName = prop,
                HeaderText = header,
                FillWeight = peso
            };
            if (formato != null) c.DefaultCellStyle.Format = formato;
            return c;
        }

        // Carga inicial
        private void FrmVentas_Load(object? sender, EventArgs e)
        {
            try
            {
                cboProducto.DataSource = _productoDao.ListarProductos();
                cboCliente.DataSource = _productoDao.ListarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo conectar a PostgreSQL.\n\nRevisen la cadena de conexión en Data/Conexion.cs " +
                    "y que el servicio esté iniciado.\n\nDetalle: " + ex.Message,
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            NuevaVenta();
        }

        private void MostrarPrecioSeleccionado()
        {
            txtPrecio.Text = cboProducto.SelectedItem is Producto p ? p.Precio.ToString("N2") : "";
        }

        // Lógica Maestro-Detalle en memoria
        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            if (cboProducto.SelectedItem is not Producto prod)
            {
                MessageBox.Show("Seleccione un producto.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int cantidad = (int)nudCantidad.Value;
            if (cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor a 0. Corrija el valor e intente de nuevo.",
                    "Cantidad inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudCantidad.Focus();
                return;
            }

            // Si el producto ya está en la lista, acumulamos cantidad en lugar de duplicar la fila
            var existente = _detalles.FirstOrDefault(d => d.IdProducto == prod.Id);
            if (existente != null)
            {
                existente.Cantidad += cantidad;
                _bs.ResetBindings(false);         // refresca la grilla (Subtotal es calculado)
            }
            else
            {
                _detalles.Add(new DetalleVenta
                {
                    IdProducto = prod.Id,
                    NombreProducto = prod.Nombre,
                    Cantidad = cantidad,
                    PrecioUnitario = prod.Precio
                });
            }

            nudCantidad.Value = 1;
            ActualizarTotal();
        }

        private void BtnQuitar_Click(object? sender, EventArgs e)
        {
            if (_bs.Current is DetalleVenta item)
            {
                _detalles.Remove(item);
                ActualizarTotal();
            }
            else
            {
                MessageBox.Show("Seleccione primero una fila del detalle.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ActualizarTotal()
        {
            _ventaActual.Detalles = _detalles.ToList();
            lblTotal.Text = $"Bs. {_ventaActual.Total:N2}";
        }

        private void NuevaVenta()
        {
            _ventaActual = new Venta();      // Fecha = ahora, Cliente = "Consumidor Final"
            _detalles.Clear();
            dtpFecha.Value = _ventaActual.Fecha;
            if (cboCliente.Items.Count > 0) cboCliente.SelectedIndex = 0;
            nudCantidad.Value = 1;
            ActualizarTotal();
        }

        // Cierre de la operación
        private void BtnConfirmar_Click(object? sender, EventArgs e)
        {
            // Validación de borde (Prueba 3)
            if (_detalles.Count == 0)
            {
                MessageBox.Show("No se puede confirmar una venta vacía. Agregue al menos un artículo.",
                    "Venta sin artículos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_detalles.Any(d => d.Cantidad <= 0))
            {
                MessageBox.Show("Existen artículos con cantidad inválida. Corríjalos antes de confirmar.",
                    "Cantidad inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _ventaActual.Fecha = DateTime.Now;
            _ventaActual.Cliente = string.IsNullOrWhiteSpace(cboCliente.Text) ? "Consumidor Final" : cboCliente.Text.Trim();
            _ventaActual.Detalles = _detalles.ToList();

            try
            {
                decimal total = _ventaActual.Total;
                int id = _ventaDao.RegistrarVenta(_ventaActual);

                MessageBox.Show($"¡Venta #{id} registrada con éxito por un total de Bs. {total:N2}!",
                    "Venta confirmada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                NuevaVenta();
            }
            catch (Exception ex)
            {
                // Rollback ya hecho en el DAO: no queda nada a medias en la BD
                MessageBox.Show("No se pudo registrar la venta. No se guardó ningún dato.\n\nDetalle: " + ex.Message,
                    "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
