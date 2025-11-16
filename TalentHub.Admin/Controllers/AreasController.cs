using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TalentHub.Admin.Data;
using TalentHub.Admin.Models;

namespace TalentHub.Admin.Controllers
{
    public class AreasController : Controller
    {
        // GET: Areas
        public IActionResult Index()
        {
            List<Area> areas = new();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT Id, Nombre FROM Areas";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        areas.Add(new Area
                        {
                            Id = (int)reader["Id"],
                            Nombre = reader["Nombre"].ToString()!
                        });
                    }
                }
            }

            return View(areas);
        }

        // GET: Areas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Areas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Area area)
        {
            if (!ModelState.IsValid)
            {
                return View(area);
            }

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "INSERT INTO Areas (Nombre) VALUES (@Nombre)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", area.Nombre);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Areas/Edit/5
        public IActionResult Edit(int id)
        {
            Area? area = null;

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT Id, Nombre FROM Areas WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            area = new Area
                            {
                                Id = (int)reader["Id"],
                                Nombre = reader["Nombre"].ToString()!
                            };
                        }
                    }
                }
            }

            if (area == null)
                return NotFound();

            return View(area);
        }

        // POST: Areas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Area area)
        {
            if (!ModelState.IsValid)
            {
                return View(area);
            }

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "UPDATE Areas SET Nombre = @Nombre WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", area.Id);
                    cmd.Parameters.AddWithValue("@Nombre", area.Nombre);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Areas/Delete/5
        public IActionResult Delete(int id)
        {
            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = "DELETE FROM Areas WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
