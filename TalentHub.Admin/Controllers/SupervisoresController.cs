using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using TalentHub.Admin.Data;
using TalentHub.Admin.Models;

namespace TalentHub.Admin.Controllers
{
    public class SupervisoresController : BaseController
    {
        // ================== HELPERS DE ROL / SESIÓN ==================

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Rol") == "Admin";
        }

        private bool IsSupervisor()
        {
            return HttpContext.Session.GetString("Rol") == "Supervisor";
        }

        private int? GetSupervisorIdSession()
        {
            return HttpContext.Session.GetInt32("SupervisorId");
        }

        // ================== CRUD SUPERVISORES (SOLO ADMIN) ==================

        // GET: Supervisores
        public IActionResult Index()
        {
            var r = Proteger();
            if (r != null) return r;

            // Rol incorrecto => NO se va a Login (eso causa "logout fantasma")
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            List<Supervisor> supervisores = new();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT s.Id, s.NombreCompleto, s.AreaId, a.Nombre AS AreaNombre
                    FROM Supervisores s
                    INNER JOIN Areas a ON s.AreaId = a.Id";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        supervisores.Add(new Supervisor
                        {
                            Id = (int)reader["Id"],
                            NombreCompleto = reader["NombreCompleto"].ToString()!,
                            AreaId = (int)reader["AreaId"],
                            Area = new Area
                            {
                                Id = (int)reader["AreaId"],
                                Nombre = reader["AreaNombre"].ToString()!
                            }
                        });
                    }
                }
            }

            return View(supervisores);
        }

        // GET: Supervisores/Create
        public IActionResult Create()
        {
            var r = Proteger();
            if (r != null) return r;

            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            ViewBag.Areas = GetAreasSelectList();
            return View();
        }

        // POST: Supervisores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supervisor supervisor)
        {
            var r = Proteger();
            if (r != null) return r;

            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Areas = GetAreasSelectList();
                return View(supervisor);
            }

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    INSERT INTO Supervisores (NombreCompleto, AreaId)
                    VALUES (@Nombre, @AreaId)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", supervisor.NombreCompleto);
                    cmd.Parameters.AddWithValue("@AreaId", supervisor.AreaId);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Supervisores/Edit/5
        public IActionResult Edit(int id)
        {
            var r = Proteger();
            if (r != null) return r;

            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            Supervisor? supervisor = null;

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT Id, NombreCompleto, AreaId FROM Supervisores WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            supervisor = new Supervisor
                            {
                                Id = (int)reader["Id"],
                                NombreCompleto = reader["NombreCompleto"].ToString()!,
                                AreaId = (int)reader["AreaId"]
                            };
                        }
                    }
                }
            }

            if (supervisor == null)
                return NotFound();

            ViewBag.Areas = GetAreasSelectList();
            return View(supervisor);
        }

        // POST: Supervisores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Supervisor supervisor)
        {
            var r = Proteger();
            if (r != null) return r;

            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Areas = GetAreasSelectList();
                return View(supervisor);
            }

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    UPDATE Supervisores
                    SET NombreCompleto = @Nombre,
                        AreaId = @AreaId
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", supervisor.Id);
                    cmd.Parameters.AddWithValue("@Nombre", supervisor.NombreCompleto);
                    cmd.Parameters.AddWithValue("@AreaId", supervisor.AreaId);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Supervisores/Delete/5
        public IActionResult Delete(int id)
        {
            var r = Proteger();
            if (r != null) return r;

            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "DELETE FROM Supervisores WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // EVALUACIONES: cada supervisor evalúa a su propio equipo
        // =========================================================

        // Listado de empleados de un supervisor + su score (si existe)
        // URL: /Supervisores/Evaluar/5   (5 = Id del supervisor)
        public IActionResult Evaluar(int id)
        {
            var r = Proteger();
            if (r != null) return r;

            // Solo Admin o Supervisor logueado
            if (!IsAdmin() && !IsSupervisor())
                return RedirectToAction("Index", "Home");

            var supervisor = GetSupervisorById(id);
            if (supervisor == null) return NotFound();

            // Si es supervisor, solo puede ver su propio equipo
            if (IsSupervisor())
            {
                var supSession = GetSupervisorIdSession();
                if (!supSession.HasValue || supSession.Value != id)
                    return Forbid(); // aquí sí corresponde
            }

            var lista = GetEmpleadosConEvaluacionPorSupervisor(id);

            ViewBag.Supervisor = supervisor;
            return View(lista);
        }

        // GET: formulario para evaluar/editar a un empleado de ese supervisor
        // URL: /Supervisores/EditEvaluacion?supervisorId=5&empleadoId=3
        public IActionResult EditEvaluacion(int supervisorId, int empleadoId)
        {
            var r = Proteger();
            if (r != null) return r;

            if (!IsAdmin() && !IsSupervisor())
                return RedirectToAction("Index", "Home");

            var supervisor = GetSupervisorById(supervisorId);
            if (supervisor == null) return NotFound();

            // Validar que el supervisor logueado solo evalúe su equipo
            if (IsSupervisor())
            {
                var supSession = GetSupervisorIdSession();
                if (!supSession.HasValue || supSession.Value != supervisorId)
                    return Forbid(); // aquí sí corresponde
            }

            var empleado = GetEmpleadoById(empleadoId);
            if (empleado == null || empleado.SupervisorId != supervisorId)
                return NotFound(); // no puedes evaluar empleados que no son tuyos

            var evaluacion = GetEvaluacion(supervisorId, empleadoId)
                             ?? new EvaluacionVacante
                             {
                                 SupervisorId = supervisorId,
                                 EmpleadoId = empleadoId
                             };

            ViewBag.Supervisor = supervisor;
            ViewBag.Empleado = empleado;

            return View(evaluacion);
        }

        // POST: guardar evaluación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEvaluacion(EvaluacionVacante model)
        {
            var r = Proteger();
            if (r != null) return r;

            if (!IsAdmin() && !IsSupervisor())
                return RedirectToAction("Index", "Home");

            // Validar que el supervisor logueado corresponde
            if (IsSupervisor())
            {
                var supSession = GetSupervisorIdSession();
                if (!supSession.HasValue || supSession.Value != model.SupervisorId)
                    return Forbid(); // aquí sí corresponde
            }

            // Validar que el empleado realmente pertenece a ese supervisor
            var empleado = GetEmpleadoById(model.EmpleadoId);
            if (empleado == null || empleado.SupervisorId != model.SupervisorId)
            {
                ModelState.AddModelError(string.Empty, "El empleado no pertenece a este supervisor.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Supervisor = GetSupervisorById(model.SupervisorId);
                ViewBag.Empleado = empleado;
                return View(model);
            }

            UpsertEvaluacion(model);

            return RedirectToAction("Evaluar", new { id = model.SupervisorId });
        }

        // ================== MÉTODOS PRIVADOS ==================

        private Supervisor? GetSupervisorById(int id)
        {
            Supervisor? supervisor = null;

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT Id, NombreCompleto, AreaId FROM Supervisores WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            supervisor = new Supervisor
                            {
                                Id = (int)reader["Id"],
                                NombreCompleto = reader["NombreCompleto"].ToString()!,
                                AreaId = (int)reader["AreaId"]
                            };
                        }
                    }
                }
            }

            return supervisor;
        }

        private Empleado? GetEmpleadoById(int id)
        {
            Empleado? emp = null;

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT Id, NombreCompleto, Cedula, Correo, FechaIngreso, AreaId, SupervisorId
                    FROM Empleados
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            emp = new Empleado
                            {
                                Id = (int)reader["Id"],
                                NombreCompleto = reader["NombreCompleto"].ToString()!,
                                Cedula = reader["Cedula"].ToString()!,
                                Correo = reader["Correo"].ToString()!,
                                FechaIngreso = Convert.ToDateTime(reader["FechaIngreso"]),
                                AreaId = (int)reader["AreaId"],
                                SupervisorId = (int)reader["SupervisorId"]
                            };
                        }
                    }
                }
            }

            return emp;
        }

        private EvaluacionVacante? GetEvaluacion(int supervisorId, int empleadoId)
        {
            EvaluacionVacante? ev = null;

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT Id, EmpleadoId, SupervisorId, ScoreSupervisor, Comentarios, FechaEvaluacion
                    FROM EvaluacionesVacante
                    WHERE SupervisorId = @SupervisorId AND EmpleadoId = @EmpleadoId";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SupervisorId", supervisorId);
                    cmd.Parameters.AddWithValue("@EmpleadoId", empleadoId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ev = new EvaluacionVacante
                            {
                                Id = (int)reader["Id"],
                                EmpleadoId = (int)reader["EmpleadoId"],
                                SupervisorId = (int)reader["SupervisorId"],
                                ScoreSupervisor = (int)reader["ScoreSupervisor"],
                                Comentarios = reader["Comentarios"] as string,
                                FechaEvaluacion = Convert.ToDateTime(reader["FechaEvaluacion"])
                            };
                        }
                    }
                }
            }

            return ev;
        }

        private void UpsertEvaluacion(EvaluacionVacante model)
        {
            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string checkSql = @"
                    SELECT COUNT(*) 
                    FROM EvaluacionesVacante
                    WHERE SupervisorId = @SupervisorId AND EmpleadoId = @EmpleadoId";

                int count;
                using (var cmd = new SqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@SupervisorId", model.SupervisorId);
                    cmd.Parameters.AddWithValue("@EmpleadoId", model.EmpleadoId);
                    count = (int)cmd.ExecuteScalar();
                }

                if (count == 0)
                {
                    string insertSql = @"
                        INSERT INTO EvaluacionesVacante
                            (EmpleadoId, SupervisorId, ScoreSupervisor, Comentarios)
                        VALUES
                            (@EmpleadoId, @SupervisorId, @ScoreSupervisor, @Comentarios);";

                    using (var cmd = new SqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmpleadoId", model.EmpleadoId);
                        cmd.Parameters.AddWithValue("@SupervisorId", model.SupervisorId);
                        cmd.Parameters.AddWithValue("@ScoreSupervisor", model.ScoreSupervisor);
                        cmd.Parameters.AddWithValue("@Comentarios", (object?)model.Comentarios ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    string updateSql = @"
                        UPDATE EvaluacionesVacante
                        SET ScoreSupervisor = @ScoreSupervisor,
                            Comentarios = @Comentarios,
                            FechaEvaluacion = GETDATE()
                        WHERE SupervisorId = @SupervisorId AND EmpleadoId = @EmpleadoId;";

                    using (var cmd = new SqlCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmpleadoId", model.EmpleadoId);
                        cmd.Parameters.AddWithValue("@SupervisorId", model.SupervisorId);
                        cmd.Parameters.AddWithValue("@ScoreSupervisor", model.ScoreSupervisor);
                        cmd.Parameters.AddWithValue("@Comentarios", (object?)model.Comentarios ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private List<RecomendacionEmpleadoViewModel> GetEmpleadosConEvaluacionPorSupervisor(int supervisorId)
        {
            var lista = new List<RecomendacionEmpleadoViewModel>();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        e.Id AS EmpleadoId,
                        e.NombreCompleto,
                        e.Cedula,
                        e.Correo,
                        ev.ScoreSupervisor
                    FROM Empleados e
                    LEFT JOIN EvaluacionesVacante ev
                        ON ev.EmpleadoId = e.Id
                        AND ev.SupervisorId = @SupervisorId
                    WHERE e.SupervisorId = @SupervisorId
                    ORDER BY ISNULL(ev.ScoreSupervisor, -1) DESC, e.NombreCompleto;";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SupervisorId", supervisorId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object scoreObj = reader["ScoreSupervisor"];
                            int? score = scoreObj == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(scoreObj);

                            lista.Add(new RecomendacionEmpleadoViewModel
                            {
                                EmpleadoId = (int)reader["EmpleadoId"],
                                NombreCompleto = reader["NombreCompleto"].ToString()!,
                                Cedula = reader["Cedula"].ToString()!,
                                Correo = reader["Correo"].ToString()!,
                                ScoreSupervisor = score
                            });
                        }
                    }
                }
            }

            return lista;
        }

        // 🔹 Método auxiliar para llenar el dropdown de áreas
        private List<SelectListItem> GetAreasSelectList()
        {
            var items = new List<SelectListItem>();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT Id, Nombre FROM Areas";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Value = reader["Id"].ToString(),
                            Text = reader["Nombre"].ToString()
                        });
                    }
                }
            }

            return items;
        }

        // 🔹 Endpoint para obtener supervisores por área (para el dropdown dependiente)
        [HttpGet]
        public IActionResult GetByArea(int areaId)
        {
            var r = Proteger();
            if (r != null) return r;

            // Si no es admin, no es que "no esté logueado", es falta de permisos
            if (!IsAdmin())
                return Forbid();

            List<Supervisor> supervisores = new();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT Id, NombreCompleto FROM Supervisores WHERE AreaId = @AreaId";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AreaId", areaId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            supervisores.Add(new Supervisor
                            {
                                Id = (int)reader["Id"],
                                NombreCompleto = reader["NombreCompleto"].ToString()!
                            });
                        }
                    }
                }
            }

            // Devuelve JSON para consumirlo con JavaScript en la vista de Empleados
            return Json(supervisores);
        }
    }
}
