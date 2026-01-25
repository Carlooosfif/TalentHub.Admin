using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using TalentHub.Admin.Data;
using TalentHub.Admin.Models;
using TalentHub.Admin.Helpers;

namespace TalentHub.Admin.Controllers
{
    public class AccountController : Controller
    {
        // ================== LOGIN ==================

        [HttpGet]
        public IActionResult Login()
        {
            // 🔒 Si ya hay sesión, NO debe volver al login
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Rol")))
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT Id, Email, PasswordHash, PasswordSalt, Rol, SupervisorId
                    FROM Usuarios
                    WHERE Email = @Email";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", model.Email);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string hashDb = reader["PasswordHash"].ToString()!;
                            string saltDb = reader["PasswordSalt"].ToString()!;
                            string rol = reader["Rol"].ToString()!;
                            int usuarioId = (int)reader["Id"];

                            int? supervisorId =
                                reader["SupervisorId"] != DBNull.Value
                                ? (int?)reader["SupervisorId"]
                                : null;

                            bool ok = PasswordHelper.VerifyPassword(
                                model.Password, hashDb, saltDb);

                            if (ok)
                            {
                                // 🔥 MUY IMPORTANTE: limpiar sesión previa
                                HttpContext.Session.Clear();

                                HttpContext.Session.SetInt32("UsuarioId", usuarioId);
                                HttpContext.Session.SetString("Rol", rol);

                                if (supervisorId.HasValue)
                                    HttpContext.Session.SetInt32(
                                        "SupervisorId", supervisorId.Value);

                                // 🔀 Redirección por rol
                                if (rol == "Admin")
                                    return RedirectToAction("Index", "Vacantes");

                                if (rol == "Supervisor" && supervisorId.HasValue)
                                    return RedirectToAction(
                                        "Evaluar", "Supervisores",
                                        new { id = supervisorId.Value });

                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Correo o contraseña inválidos.");
            return View(model);
        }

        // ================== REGISTRO ==================

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Supervisores = GetSupervisoresSelectList();
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Supervisores = GetSupervisoresSelectList();
                return View(model);
            }

            if (model.Rol == "Supervisor" && !model.SupervisorId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(model.SupervisorId),
                    "Debe seleccionar un supervisor asociado.");

                ViewBag.Supervisores = GetSupervisoresSelectList();
                return View(model);
            }

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string checkSql =
                    "SELECT COUNT(*) FROM Usuarios WHERE Email = @Email";

                using (var checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Email", model.Email);

                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        ModelState.AddModelError(
                            nameof(model.Email),
                            "Ya existe un usuario con este correo.");

                        ViewBag.Supervisores = GetSupervisoresSelectList();
                        return View(model);
                    }
                }

                var (hash, salt) =
                    PasswordHelper.HashPassword(model.Password);

                string insertSql = @"
                    INSERT INTO Usuarios
                        (Email, PasswordHash, PasswordSalt, Rol, SupervisorId)
                    VALUES
                        (@Email, @PasswordHash, @PasswordSalt, @Rol, @SupervisorId);";

                using (var cmd = new SqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", hash);
                    cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                    cmd.Parameters.AddWithValue("@Rol", model.Rol);
                    cmd.Parameters.AddWithValue(
                        "@SupervisorId",
                        (object?)model.SupervisorId ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Login");
        }

        // ================== LOGOUT ==================

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            // Refuerzo de seguridad
            Response.Headers["Cache-Control"] = "no-store";

            return RedirectToAction("Login");
        }

        // ================== MÉTODOS PRIVADOS ==================

        private List<SelectListItem> GetSupervisoresSelectList()
        {
            var lista = new List<SelectListItem>();

            using (var conn = SqlHelper.GetConnection())
            {
                conn.Open();

                string sql =
                    "SELECT Id, NombreCompleto FROM Supervisores ORDER BY NombreCompleto";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new SelectListItem
                        {
                            Value = reader["Id"].ToString(),
                            Text = reader["NombreCompleto"].ToString()
                        });
                    }
                }
            }

            return lista;
        }
    }
}
