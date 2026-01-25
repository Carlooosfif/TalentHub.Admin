using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using TalentHub.Admin.Data;
using TalentHub.Admin.Models;

namespace TalentHub.Admin.Controllers
{
    public class VacantesController : BaseController
    {
        public IActionResult Index()
        {
            var r = Proteger();
            if (r != null) return r;
            List<Vacante> vacantes = new();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT Id, Titulo, Area, Ubicacion, Estado, FechaPublicacion
                    FROM Vacantes
                    ORDER BY FechaPublicacion DESC";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vacantes.Add(new Vacante
                        {
                            Id = (int)reader["Id"],
                            Titulo = reader["Titulo"].ToString()!,
                            Area = reader["Area"].ToString()!,
                            Ubicacion = reader["Ubicacion"].ToString()!,
                            Estado = reader["Estado"].ToString()!,
                            FechaPublicacion = Convert.ToDateTime(reader["FechaPublicacion"])
                        });
                    }
                }
            }

            return View(vacantes);
        }

        public IActionResult Create()
        {
            ViewBag.Areas = GetAreasSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Vacante model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Areas = GetAreasSelectList();
                return View(model);
            }

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    INSERT INTO Vacantes (Titulo, Area, Ubicacion, Estado, FechaPublicacion)
                    VALUES (@Titulo, @Area, @Ubicacion, @Estado, @Fecha)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Titulo", model.Titulo);
                    cmd.Parameters.AddWithValue("@Area", model.Area);
                    cmd.Parameters.AddWithValue("@Ubicacion", model.Ubicacion);
                    cmd.Parameters.AddWithValue("@Estado", model.Estado);
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var vacante = GetVacante(id);
            if (vacante == null) return NotFound();

            return View(vacante);
        }

        public IActionResult Edit(int id)
        {
            var vacante = GetVacante(id);
            if (vacante == null) return NotFound();

            ViewBag.Areas = GetAreasSelectList();
            return View(vacante);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Vacante model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Areas = GetAreasSelectList();
                return View(model);
            }

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    UPDATE Vacantes
                    SET Titulo = @Titulo,
                        Area = @Area,
                        Ubicacion = @Ubicacion,
                        Estado = @Estado
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Titulo", model.Titulo);
                    cmd.Parameters.AddWithValue("@Area", model.Area);
                    cmd.Parameters.AddWithValue("@Ubicacion", model.Ubicacion);
                    cmd.Parameters.AddWithValue("@Estado", model.Estado);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var vacante = GetVacante(id);
            if (vacante == null) return NotFound();

            return View(vacante);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "DELETE FROM Vacantes WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // MÉTODOS AUXILIARES
        // =========================

        private List<SelectListItem> GetAreasSelectList()
        {
            var lista = new List<SelectListItem>();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT Id, Nombre FROM Areas ORDER BY Nombre";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new SelectListItem
                        {
                            Value = reader["Nombre"].ToString(),  // guardamos el NOMBRE
                            Text = reader["Nombre"].ToString()
                        });
                    }
                }
            }

            return lista;
        }


        private Vacante? GetVacante(int id)
        {
            Vacante? vacante = null;

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT Id, Titulo, Area, Ubicacion, Estado, FechaPublicacion
                    FROM Vacantes
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vacante = new Vacante
                            {
                                Id = (int)reader["Id"],
                                Titulo = reader["Titulo"].ToString()!,
                                Area = reader["Area"].ToString()!,
                                Ubicacion = reader["Ubicacion"].ToString()!,
                                Estado = reader["Estado"].ToString()!,
                                FechaPublicacion = Convert.ToDateTime(reader["FechaPublicacion"])
                            };
                        }
                    }
                }
            }

            return vacante;
        }
    }
}
