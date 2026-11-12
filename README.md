# Veterinaria API – Semana 13 (capas + DTOs + AutoMapper)

## Tema
Clínica veterinaria: registra **dueños** y las **mascotas** que atiende, reemplazando fichas en papel.
- **Dueno** (Id, Nombre, Dni, Telefono, Email, FechaRegistro) 1 — N **Mascota** (Id, Nombre, Especie, Raza, Edad, Peso, Activo, DuenoId → FK).

### Reglas de negocio (lanzan `ReglaNegocioException` → 409)
1. No se puede registrar un dueño con un DNI ya existente.
2. No se puede eliminar un dueño que tiene mascotas.
3. Un dueño no puede tener más de 5 mascotas.
4. Un dueño no puede repetir el nombre de una de sus mascotas.

## Arquitectura
`Api → Negocio → Datos → Entidades`
- **Entidades**: `Models/` (scaffold, sin `[JsonIgnore]`) y `Dtos/`.
- **Datos**: `VeterinariaContext` y repositorios (interfaz + clase).
- **Negocio**: servicios, `MappingProfile`, `ReglaNegocioException`.
- **Api**: `DuenosController`, `MascotasController`, `Program.cs` (DI, Swagger UI).

Base de datos: [BaseDatos/Veterinaria.sql](BaseDatos/Veterinaria.sql) (con datos de ejemplo).
Cadena de conexión: crear Veterinaria.Api/appsettings.Development.json (no se sube al repo) con la clave ConnectionStrings:VeterinariaDb apuntando a tu SQL Server.

Ejecutar: `dotnet run --project Veterinaria.Api` → `/swagger`.

## Endpoints (11)
| Método | Ruta | Respuestas |
|---|---|---|
| GET | api/duenos?nombre= | 200 |
| GET | api/duenos/{id} | 200, 404 |
| GET | api/duenos/{id}/mascotas (anidada) | 200, 404 |
| POST | api/duenos | 201, 400, 409 |
| PUT | api/duenos/{id} | 204, 400, 404 |
| DELETE | api/duenos/{id} | 204, 404, 409 |
| GET | api/mascotas?especie=&activo=&pesoMin=&pesoMax= | 200 |
| GET | api/mascotas/{id} | 200, 404 |
| POST | api/mascotas | 201, 400, 404 (dueño no existe), 409 |
| PUT | api/mascotas/{id} | 204, 400, 404, 409 |
| DELETE | api/mascotas/{id} | 204, 404 |

## Sustentación de los mapeos
| Mapeo | Origen → Destino | ¿Por qué en ese sentido? | Configuración especial |
|---|---|---|---|
| Dueno → DuenoDto | Entidad → DTO de lectura | Es lo que la API devuelve; la entidad (con su colección `Mascotas`) no sale de la API, así se evita el ciclo JSON | ForMember: `CantidadMascotas` = `Mascotas.Count` |
| Mascota → MascotaDto | Entidad → DTO de lectura | Es lo que la API devuelve al cliente | ForMember: `DuenoNombre` toma `Dueno.Nombre` en vez del objeto Dueño |
| DuenoCrearDto → Dueno | DTO de entrada → Entidad | Llega del cliente y se convierte en entidad para guardarla; no se devuelve | Ninguna: nombres coinciden (el servicio asigna `FechaRegistro`) |
| DuenoActualizarDto → Dueno | DTO de entrada → Entidad existente | Se usa con `Map(origen, destino)` para modificar la entidad cargada de la BD | Ninguna: el DTO no trae `Id` ni `Dni`, por eso no se sobrescriben |
| MascotaCrearDto → Mascota | DTO de entrada → Entidad | Llega del cliente y se guarda | Ninguna: coinciden (el servicio fija `Activo = true`) |
| MascotaActualizarDto → Mascota | DTO de entrada → Entidad existente | `Map(origen, destino)` sobre la mascota existente | Ninguna: no trae `Id` ni `DuenoId`, no se puede cambiar de dueño |

No se usa `ReverseMap()`: los DTOs de lectura nunca se convierten de vuelta a entidad (los de entrada son distintos), así que no se justifica.

## Pendiente por tu parte (capturas)
Explorador de soluciones, MappingProfile completo, pruebas en Swagger/Postman de 200, 201, 204, 400, 404 y 409, y Observaciones/Conclusiones.
