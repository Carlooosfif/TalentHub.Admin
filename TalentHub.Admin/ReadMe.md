# TalentHub Admin (MVC)

Proyecto de administración desarrollado en ASP.NET Core MVC que alimenta el core de recomendación de talento interno.  
El objetivo del admin es gestionar la información base que utilizará un sistema central (core) para sugerir candidatos internos a vacantes.

## 1. Descripción general

El sistema permite administrar:

- Áreas
- Supervisores
- Empleados
- Vacantes internas

Esta información se guarda en una base de datos SQL Server y se expone a través de una interfaz MVC con vistas Razor y estilos en SCSS/CSS.

El proyecto está pensado como la "capa de administración" que alimenta un core de negocio orientado a recomendar talento interno para vacantes.

## 2. Tecnologías utilizadas

- .NET 8 / ASP.NET Core MVC
- C#
- SQL Server
- ADO.NET con `SqlConnection` y `SqlCommand` (clase `SqlHelper`)
- HTML + Razor
- SCSS (compilado a CSS)
- Bootstrap (parcialmente)

## 3. Modelo de datos (tablas principales)

Base de datos: `TalentHubDb`

Tablas principales:

- `Areas`
  - `Id` (PK)
  - `Nombre`

- `Supervisores`
  - `Id` (PK)
  - `NombreCompleto`
  - `AreaId` (FK a `Areas.Id`)

- `Empleados`
  - `Id` (PK)
  - `NombreCompleto`
  - `Cedula`
  - `Correo`
  - `FechaIngreso`
  - `AreaId` (FK a `Areas.Id`)
  - `SupervisorId` (FK a `Supervisores.Id`)

- `Vacantes`
  - `Id` (PK)
  - `Titulo`
  - `Area`
  - `Ubicacion`
  - `Estado`
  - `FechaPublicacion`

Los scripts de creación de tablas se encuentran en el archivo SQL del proyecto (por ejemplo, `Database.sql` o similar).

## 4. Características importantes para la tarea

### 4.1. Validación de dato sensible en Back-End (cédula)

La cédula del empleado se considera un dato sensible para el core.  
Por este motivo:

- Se valida en el controlador `EmpleadosController`, en las acciones `Create` y `Edit`.
- La cédula debe:
  - Tener exactamente 10 dígitos.
  - Ser única en la tabla `Empleados`.

Si la cédula no cumple las reglas, se agrega un error al `ModelState` y el registro no se guarda en la base de datos.

Esto garantiza que, aunque el usuario deshabilite JavaScript o modifique el formulario en el navegador, el servidor no aceptará cédulas inválidas ni duplicadas.

### 4.2. Uso de dropdowns con referencia a otras tablas

Para evitar ingresar claves foráneas manualmente, el proyecto implementa dropdowns dependientes.  
Un ejemplo es el formulario de creación de empleados:

- El usuario selecciona un **Área** desde un dropdown poblado desde la tabla `Areas`.
- En base al área seleccionada, se llama a un endpoint que devuelve los **Supervisores** de esa área.
- El dropdown de **Supervisor** se llena dinámicamente con esos datos.

De esta manera:

- No se ingresa el `AreaId` ni el `SupervisorId` a mano.
- Se respeta la relación entre tablas y se guía al usuario a elegir valores válidos.

## 5. Estructura del proyecto

Estructura general:

- `Controllers/`
  - `HomeController.cs`
  - `AreasController.cs`
  - `SupervisoresController.cs`
  - `EmpleadosController.cs`
  - `VacantesController.cs`
- `Models/`
  - `Area.cs`
  - `Supervisor.cs`
  - `Empleado.cs`
  - `Vacante.cs`
  - `DashboardViewModel.cs` (modelo para el panel principal)
- `Data/`
  - `SqlHelper.cs` (clase estática para obtener conexiones a SQL)
- `Views/`
  - `Shared/_Layout.cshtml`
  - `Home/Index.cshtml` (panel principal con métricas e últimas vacantes)
  - Carpeta de vistas para cada entidad (Áreas, Supervisores, Empleados, Vacantes)
- `wwwroot/`
  - `css/main.css`
  - `scss/main.scss`
  - librerías estáticas (Bootstrap, etc.)

## 6. Panel principal (Home)

La pantalla de inicio (`Home/Index`) funciona como un dashboard:

- Muestra contadores:
  - Total de áreas
  - Total de supervisores
  - Total de empleados
  - Total de vacantes
- Muestra una tabla con las últimas vacantes registradas en la base de datos.

Los datos se obtienen desde SQL utilizando `SqlHelper` en el `HomeController`.

## 7. Cómo ejecutar el proyecto en local

### 7.1. Requisitos

- Visual Studio 2022 (o superior) con carga de trabajo de ASP.NET y desarrollo web.
- SQL Server (localdb o instancia en equipo).
- .NET SDK (versión compatible con el proyecto, por ejemplo .NET 8).

### 7.2. Pasos

1. Clonar el repositorio:

   ```bash
   git clone https://github.com/Carlooosfif/talenthub-admin-mvc.git
