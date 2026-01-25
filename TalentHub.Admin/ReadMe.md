# TalentHub – Sistema de Recomendación de Talento Interno

TalentHub es un sistema desarrollado en **ASP.NET Core MVC** que permite administrar información organizacional y exponer un **API REST (JSON)** para soportar un **core de recomendación de talento interno**, el cual es consumido por un aplicativo frontend desarrollado en **React**.

El proyecto aplica principios de **ingeniería de software**, **SOLID**, **patrones de diseño** y separación por capas, como parte del proceso de diseño de ingeniería (UDLA–ABET).

---

## 1. Descripción general del proyecto

El sistema se divide en dos componentes principales:

1. **TalentHub.Admin (ASP.NET Core MVC)**  
   Aplicación administrativa encargada de gestionar:
   - Áreas
   - Supervisores
   - Empleados
   - Vacantes internas
   - Evaluaciones de supervisores

2. **TalentHub Recomendaciones (React)**  
   Aplicación frontend que consume el API REST para visualizar el ranking de empleados recomendados para una vacante específica.

La información se almacena en una base de datos **SQL Server**, y el backend expone los datos necesarios en formato **JSON** para su consumo externo.

---

## 2. Arquitectura general

El proyecto sigue una arquitectura por capas:

- **Controllers**
- **Services**
- **Repositories**
- **Strategies**
- **Models**
- **API REST**
- **Frontend React**

Se aplican los siguientes principios y patrones:

### Principios SOLID
- **Single Responsibility Principle (SRP)**  
  Separación clara entre controladores, servicios, repositorios y estrategias.
- **Dependency Inversion Principle (DIP)**  
  Uso de interfaces e inyección de dependencias en `Program.cs`.

### Patrones de diseño
- **Repository Pattern**: acceso a datos desacoplado.
- **Strategy Pattern**: cálculo flexible del puntaje de recomendación.

---

## 3. Tecnologías utilizadas

### Backend
- .NET 8
- ASP.NET Core MVC
- C#
- SQL Server
- ADO.NET (`SqlConnection`, `SqlCommand`)
- Inyección de dependencias nativa
- API REST (JSON)

### Frontend
- React
- JavaScript (ES6+)
- Fetch API
- HTML + CSS

### Otros
- Git / GitHub
- Azure App Service (deploy backend)
- Vercel (deploy frontend React)

---

## 4. Modelo de datos

Base de datos: **TalentHubDb**

Tablas principales:

### Areas
- `Id` (PK)
- `Nombre`

### Supervisores
- `Id` (PK)
- `NombreCompleto`
- `AreaId` (FK → Areas)

### Empleados
- `Id` (PK)
- `NombreCompleto`
- `Cedula`
- `Correo`
- `FechaIngreso`
- `AreaId` (FK → Areas)
- `SupervisorId` (FK → Supervisores)

### Vacantes
- `Id` (PK)
- `Titulo`
- `Area`
- `Ubicacion`
- `Estado`
- `FechaPublicacion`

### EvaluacionesVacante
- `Id`
- `EmpleadoId`
- `SupervisorId`
- `ScoreSupervisor`
- `Comentarios`
- `FechaEvaluacion`

---

## 5. API REST

### Endpoint principal

```http
GET /api/vacantes/{vacanteId}/recomendaciones
Respuesta (JSON)
[
  {
    "empleadoId": 1,
    "nombreCompleto": "Juan Perez",
    "correo": "juan@empresa.com",
    "scoreSupervisor": 80
  }
]

El ranking se calcula dinámicamente utilizando múltiples estrategias de puntuación (Strategy Pattern).


## 6. Estructura del repositorio

TalentHub.Admin/
├── TalentHub.Admin              # Backend MVC + API
├── TalentHub.Admin.Test         # Pruebas unitarias
├── talenthub-recomendaciones    # Frontend React
├── DOCUMENTO.docx               # Documento de diseño de ingeniería
├── PruebaFuncional.docx         # Evidencia de prueba funcional
├── TalentHub.Admin.sln
└── README.md

## 7. Ejecución del proyecto en local

### 7.1 Requisitos

Visual Studio 2022 o superior
.NET SDK 8
SQL Server (LocalDB o instancia completa)
Node.js (v18 o superior)
Git

### 7.2 Backend (ASP.NET Core MVC)

1. Clonar el repositorio:

git clone https://github.com/Carlooosfif/TalentHub.Admin.git

2. Abrir la solución:

TalentHub.Admin.sln

3. Configurar la cadena de conexión en appsettings.json.

4. Crear la base de datos y tablas usando el script SQL del proyecto.

5. Ejecutar el proyecto desde Visual Studio.

El backend quedará disponible en:

https://localhost:{puerto}

### 7.3 Frontend (React)

1. Ir a la carpeta:
cd talenthub-recomendaciones

2. Instalar dependencias:
npm install

3. Configurar la URL del API en el archivo de servicios (fetch).

4.Ejecutar el proyecto:
npm start

La aplicación React quedará disponible en:
http://localhost:3000

## 8. Pruebas

Prueba unitaria:
Implementada sobre las estrategias de cálculo de score usando xUnit.

Prueba funcional:
Documento incluido con evidencia de ejecución del sistema completo.

