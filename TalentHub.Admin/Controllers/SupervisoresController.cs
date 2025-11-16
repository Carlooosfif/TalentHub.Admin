using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using TalentHub.Admin.Data;
using TalentHub.Admin.Models;

namespace TalentHub.Admin.Controllers
{
    public class SupervisoresController : Controller
    {
        // GET: Supervisores
        public IActionResult Index()
        {
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
            ViewBag.Areas = GetAreasSelectList();
            return View();
        }

        // POST: Supervisores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supervisor supervisor)
        {
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
