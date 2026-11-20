# Actividad 7 – Guía paso a paso (VS Code + PostgreSQL + Git/GitHub)

## 0. Requisitos (Windows, porque el formulario es WinForms)
- .NET SDK 8 → `dotnet --version`
- Visual Studio Code + extensión **C# Dev Kit** (o "C#" de Microsoft)
- PostgreSQL local (con pgAdmin o psql)
- Git → `git --version`

## 1. Base de datos (PostgreSQL)
1. Abran **pgAdmin** → clic derecho en *Databases* → *Create → Database* → nombre `minisuper`.
   (o en psql: `CREATE DATABASE minisuper;`)
2. Clic derecho sobre `minisuper` → **Query Tool** → abran `sql/01_tablas.sql` → ejecuten (F5).
3. Comprueben: `SELECT * FROM productos;` debe mostrar 6 productos de ejemplo.
   > Si su proyecto YA tiene tabla `productos`/`clientes`, no ejecuten esas partes y ajusten
   > los nombres de columnas en `Data/ProductoDAO.cs`.

## 2. Proyecto C# en VS Code
**Opción A – Si parten de este proyecto:** abran la carpeta `TiendaProyecto` en VS Code (*File → Open Folder*).

**Opción B – Si ya tienen su proyecto de tienda (lo normal en el equipo):** copien estos archivos a él:

| Archivo | Destino en su proyecto |
|---|---|
| `Models/DetalleVenta.cs`, `Models/Venta.cs`, `Models/Producto.cs` | carpeta `Models/` |
| `Data/Conexion.cs`, `Data/VentaDAO.cs`, `Data/ProductoDAO.cs` | carpeta `Data/` |
| `Forms/FrmVentas.cs` | carpeta `Forms/` |
| `sql/*.sql` | carpeta `sql/` |

Y ajusten: el `namespace` si el suyo es distinto, y en `Program.cs` abran `new FrmVentas()` (o llamen al formulario desde su menú principal con `new FrmVentas().Show();`).

**Instalar Npgsql** (terminal de VS Code: *Terminal → New Terminal*, en la carpeta del `.csproj`):
```
dotnet add package Npgsql
```

**Contraseña de PostgreSQL:** editen `Data/Conexion.cs` y cambien `tu_password`.
(Para no subir la clave a GitHub, pueden definir la variable de entorno `TIENDA_CONN`.)

**Ejecutar:**
```
dotnet build
dotnet run
```

## 3. Pruebas (hagan capturas legibles)
**Prueba 1 – Camino feliz:** cliente → agregar 3 productos distintos (ej. 2 Arroz, 5 Aceite, 1 Azúcar) →
verifiquen subtotales y total (2×8.50 + 5×14.00 + 1×7.00 = **Bs. 94.00**) → *Confirmar* → **captura de la interfaz + mensaje**
(idealmente una captura con la grilla llena y otra con el mensaje, o una sola con ambos visibles).

**Prueba 2 – PostgreSQL:** en pgAdmin Query Tool ejecuten `sql/02_verificacion.sql`
→ captura mostrando `ventas` y `detalle_ventas` (y la consulta `coincide = true`).

**Prueba 3 – Validaciones:** (a) *Confirmar* con la lista vacía; (b) poner cantidad 0 o negativa y *Agregar*
→ capturen el mensaje amigable. La aplicación no debe cerrarse.

**Opcional (demuestra el Rollback):** en `detalle_ventas` el `CHECK (cantidad > 0)` hace fallar el INSERT; si
temporalmente comentan la validación en la UI y el DAO, verán que no queda ninguna fila "huérfana" en `ventas`.

## 4. Git / GitHub
Si el repositorio del equipo YA existe y está clonado, ejecuten desde la raíz del proyecto:
```bash
git pull origin main
git status
git add .
git commit -m "feat(ventas): implementa registro maestro-detalle transaccional con PostgreSQL"
git tag -a v0.2-maestro-detalle -m "Versión con módulo de registro de ventas funcional"
git push origin main
git push origin v0.2-maestro-detalle
```
Si NO existe repositorio todavía (primer subida):
1. En github.com → **New repository** (sin README) → copien la URL.
2. En la carpeta del proyecto:
```bash
git init
git branch -M main
git remote add origin https://github.com/USUARIO/REPO.git
git add .
git commit -m "feat(ventas): implementa registro maestro-detalle transaccional con PostgreSQL"
git tag -a v0.2-maestro-detalle -m "Versión con módulo de registro de ventas funcional"
git push -u origin main
git push origin v0.2-maestro-detalle
```
Notas:
- Agreguen a los compañeros en *Settings → Collaborators*.
- Si `git pull` muestra conflictos, resuélvanlos en VS Code (botones *Accept Current/Incoming*), luego `git add .` y `git commit`.
- Primera vez con Git: `git config --global user.name "Su Nombre"` y `git config --global user.email "correo@ejemplo.com"`.
- **Capturas 4:** pestaña *Commits* del repo y *Releases/Tags* (`https://github.com/USUARIO/REPO/tags`).
- No suban la contraseña real de PostgreSQL (usen `tu_password` en el commit o la variable `TIENDA_CONN`).

## 5. Informe PDF (ver `INFORME_PLANTILLA.md`)
Peguen las 4 capturas en la plantilla, expórtenla a PDF (Word → Guardar como PDF, o Google Docs → Descargar PDF)
y **cada integrante lo sube a su Moodle**.
