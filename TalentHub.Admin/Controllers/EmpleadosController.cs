using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TalentHub.Admin.Data;
using TalentHub.Admin.Models;

namespace TalentHub.Admin.Controllers
{
    public class EmpleadosController : Controller
    {
        // LISTA DE EMPLEADOS
        public IActionResult Index()
        {
            List<Empleado> empleados = new();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT e.Id, e.NombreCompleto, e.Cedula, e.Correo, e.FechaIngreso,
                           a.Id AS AreaId, a.Nombre AS AreaNombre,
                           s.Id AS SupervisorId, s.NombreCompleto AS SupervisorNombre
                    FROM Empleados e
                    INNER JOIN Areas a ON e.AreaId = a.Id
                    INNER JOIN Supervisores s ON e.SupervisorId = s.Id";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        empleados.Add(new Empleado
                        {
                            Id = (int)reader["Id"],
                            NombreCompleto = reader["NombreCompleto"].ToString()!,
                            Cedula = reader["Cedula"].ToString()!,
                            Correo = reader["Correo"].ToString()!,
                            FechaIngreso = Convert.ToDateTime(reader["FechaIngreso"]),
                            AreaId = (int)reader["AreaId"],
                            Area = new Area { Id = (int)reader["AreaId"], Nombre = reader["AreaNombre"].ToString()! },
                            SupervisorId = (int)reader["SupervisorId"],
                            Supervisor = new Supervisor { Id = (int)reader["SupervisorId"], NombreCompleto = reader["SupervisorNombre"].ToString()! }
                        });
                    }
                }
            }

            return View(empleados);
        }

        // FORMULARIO CREATE (GET)
        public IActionResult Create()
        {
            ViewBag.Areas = GetAreas();
            ViewBag.Supervisores = new List<Supervisor>(); // vacío hasta que elijan área

            return View();
        }

        // CREATE (POST)
        [HttpPost]
        public IActionResult Create(Empleado empleado)
        {
            // VALIDACIÓN DE CÉDULA (BACKEND)
            if (empleado.Cedula == null || empleado.Cedula.Length != 10)
            {
                ModelState.AddModelError("Cedula", "La cédula debe tener 10 dígitos.");
            }

            if (CedulaExiste(empleado.Cedula))
            {
                ModelState.AddModelError("Cedula", "La cédula ya existe.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Areas = GetAreas();
                ViewBag.Supervisores = GetSupervisores(empleado.AreaId);
                return View(empleado);
            }

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    INSERT INTO Empleados (NombreCompleto, Cedula, Correo, FechaIngreso, AreaId, SupervisorId)
                    VALUES (@Nombre, @Cedula, @Correo, @Fecha, @Area, @Supervisor)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", empleado.NombreCompleto);
                    cmd.Parameters.AddWithValue("@Cedula", empleado.Cedula);
                    cmd.Parameters.AddWithValue("@Correo", empleado.Correo);
                    cmd.Parameters.AddWithValue("@Fecha", empleado.FechaIngreso);
                    cmd.Parameters.AddWithValue("@Area", empleado.AreaId);
                    cmd.Parameters.AddWithValue("@Supervisor", empleado.SupervisorId);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        // EDIT (GET)
        public IActionResult Edit(int id)
        {
            Empleado empleado = GetEmpleado(id);

            ViewBag.Areas = GetAreas();
            ViewBag.Supervisores = GetSupervisores(empleado.AreaId);

            return View(empleado);
        }

        // EDIT (POST)
        [HttpPost]
        public IActionResult Edit(Empleado empleado)
        {
            if (empleado.Cedula == null || empleado.Cedula.Length != 10)
            {
                ModelState.AddModelError("Cedula", "La cédula debe tener 10 dígitos.");
            }

            if (CedulaExiste(empleado.Cedula, empleado.Id))
            {
                ModelState.AddModelError("Cedula", "La cédula ya está registrada.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Areas = GetAreas();
                ViewBag.Supervisores = GetSupervisores(empleado.AreaId);
                return View(empleado);
            }

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    UPDATE Empleados
                    SET NombreCompleto = @Nombre,
                        Cedula = @Cedula,
                        Correo = @Correo,
                        FechaIngreso = @Fecha,
                        AreaId = @Area,
                        SupervisorId = @Supervisor
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", empleado.Id);
                    cmd.Parameters.AddWithValue("@Nombre", empleado.NombreCompleto);
                    cmd.Parameters.AddWithValue("@Cedula", empleado.Cedula);
                    cmd.Parameters.AddWithValue("@Correo", empleado.Correo);
                    cmd.Parameters.AddWithValue("@Fecha", empleado.FechaIngreso);
                    cmd.Parameters.AddWithValue("@Area", empleado.AreaId);
                    cmd.Parameters.AddWithValue("@Supervisor", empleado.SupervisorId);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "DELETE FROM Empleados WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        // MÉTODOS AUXILIARES
        private Empleado GetEmpleado(int id)
        {
            Empleado empleado = new();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT * FROM Empleados WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            empleado.Id = id;
                            empleado.NombreCompleto = reader["NombreCompleto"].ToString()!;
                            empleado.Cedula = reader["Cedula"].ToString()!;
                            empleado.Correo = reader["Correo"].ToString()!;
                            empleado.FechaIngreso = Convert.ToDateTime(reader["FechaIngreso"]);
                            empleado.AreaId = (int)reader["AreaId"];
                            empleado.SupervisorId = (int)reader["SupervisorId"];
                        }
                    }
                }
            }

            return empleado;
        }

        private bool CedulaExiste(string cedula, int? id = null)
        {
            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT COUNT(*) FROM Empleados WHERE Cedula = @Cedula";

                if (id != null)
                    sql += " AND Id <> @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Cedula", cedula);

                    if (id != null)
                        cmd.Parameters.AddWithValue("@Id", id);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private List<Area> GetAreas()
        {
            List<Area> lista = new();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM Areas";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Area
                        {
                            Id = (int)reader["Id"],
                            Nombre = reader["Nombre"].ToString()!
                        });
                    }
                }
            }

            return lista;
        }

        private List<Supervisor> GetSupervisores(int areaId)
        {
            List<Supervisor> lista = new();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM Supervisores WHERE AreaId = @Area";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Area", areaId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Supervisor
                            {
                                Id = (int)reader["Id"],
                                NombreCompleto = reader["NombreCompleto"].ToString()!,
                                AreaId = areaId
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}
