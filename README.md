# Patitas · Sistema de gestión veterinaria

Aplicación web en **ASP.NET Core MVC (.NET 8)** para administrar los pacientes (mascotas) y los propietarios de una clínica veterinaria. Usa **Entity Framework Core 8** con **SQL Server**, vistas **Razor con Bootstrap 5**, subida de fotos y listas dependientes Especie → Raza cargadas por `fetch`.

## Requisitos

- .NET SDK 8
- SQL Server (LocalDB, Express o una instancia completa)
- Herramienta de EF Core: `dotnet tool install --global dotnet-ef`

## Puesta en marcha

1. **Cadena de conexión.** En `appsettings.json`, clave `ConnectionStrings:VeterinariaDB`. Por defecto apunta a LocalDB:

   ```
   Server=(localdb)\MSSQLLocalDB;Database=VeterinariaDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
   ```

2. **Crear la base de datos.** Elige una de las dos opciones:

   - **Migraciones de EF Core (Code First)**

     ```bash
     dotnet ef migrations add Inicial
     dotnet ef database update
     ```

   - **Script SQL:** ejecuta `script.sql` en SSMS o Azure Data Studio.

   Las dos opciones cargan los mismos catálogos iniciales: 3 especies (Canino, Felino, Ave) y 9 razas. La base arranca sin propietarios ni mascotas; se registran desde la aplicación (primero un propietario y luego sus mascotas).

3. **Ejecutar**

   ```bash
   dotnet run
   ```

   Abre `https://localhost:7080` (o `http://localhost:5080`).

## Funcionalidades

| Módulo | Qué hace |
|---|---|
| **Tablero** | Totales de mascotas, propietarios, especies y razas; distribución de pacientes por especie y últimos ingresos. |
| **Propietarios** | CRUD completo, búsqueda por nombre, apellido, correo o teléfono, conteo de mascotas por propietario y ficha con sus mascotas. No deja eliminar a un propietario que todavía tenga mascotas. |
| **Pacientes (mascotas)** | CRUD completo con foto. Listado en tarjetas con foto, nombre, dueño, especie y raza. Búsqueda libre y filtros por especie, raza y propietario, con orden por nombre, más recientes, edad o peso. |

### Listas dependientes (Especie → Raza)

Al elegir una especie, `wwwroot/js/app.js` pide las razas con `fetch` a:

```
GET /Mascotas/RazasDeEspecie/{idEspecie}   →   [{ "id": 5, "nombre": "Siamés" }, …]
```

y reconstruye el `<select>` de raza sin recargar la página. La misma cascada funciona en el formulario de registro y edición y en el panel de filtros del listado.

### Fotografías

- Se guardan en `wwwroot/images/` con un nombre único (GUID). En la base de datos solo queda la ruta (`/images/xxxx.jpg`).
- Formatos admitidos: JPG, PNG, WEBP o GIF, de hasta 5 MB. El servidor valida el formato y el tamaño.
- Vista previa inmediata al elegir la imagen o arrastrarla sobre el recuadro.
- Al editar se puede reemplazar o quitar la foto. El archivo anterior se borra del disco, y también al eliminar la mascota.

## Estructura

```
Controllers/
  TableroController.cs        Tablero e indicadores
  PropietariosController.cs   CRUD de propietarios
  MascotasController.cs       CRUD de mascotas, filtros, fotos y endpoint JSON de razas
Data/
  DataContext.cs              DbContext, relaciones y datos semilla
Models/                       Propietario, Especie, Raza, Mascota
ViewModels/                   TableroViewModel, MascotasListadoViewModel
Views/                        Vistas Razor (formularios compartidos como parciales)
wwwroot/css/app.css           Tema visual propio sobre Bootstrap 5
wwwroot/js/app.js             Cascada de razas, vista previa de foto y avisos (sin jQuery)
script.sql                    Creación de tablas y catálogos iniciales
```

## Decisiones de diseño

- **Sin Repository ni Unit of Work.** El `DataContext` se inyecta y se usa directamente en los controladores, como pide el enunciado.
- **Modelo normalizado.** `Mascotas` guarda solo `IdRaza`. La especie se obtiene navegando `Mascota → Raza → Especie` con `Include(...).ThenInclude(...)`, que EF traduce a `JOIN`. En `script.sql` se incluye la consulta SQL equivalente como referencia.
- **Corrección al modelo propuesto.** En la tabla del enunciado, la llave foránea de `Mascotas` aparece como `IdEspecie → Razas.Id`. Según el texto, la columna correcta es `IdRaza → Razas.Id`, y así se implementó.
- **Borrados restringidos.** Las llaves foráneas usan `Restrict`: no se puede borrar una especie que tenga razas, ni una raza o un propietario que tengan mascotas.
- **Tipos de columna.** Se respetan los tipos del modelo SQL propuesto: `VARCHAR`, `DATE` y `DECIMAL(5,2)`.
- **Cultura.** Se usa `es-CO` para mostrar fechas, pero con punto como separador decimal, para que el peso que envía `<input type="number">` (ej. `15.50`) se interprete igual en cualquier servidor.
- **Validación en dos capas.** Data annotations con mensajes en español y validación del lado del cliente (jQuery Validation Unobtrusive). En el servidor también se valida la foto y que la fecha de nacimiento no sea futura.