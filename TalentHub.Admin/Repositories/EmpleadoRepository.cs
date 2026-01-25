using Microsoft.Data.SqlClient;
using System.Data;
using TalentHub.Admin.Data;
using TalentHub.Admin.Models;
using TalentHub.Admin.Repositories.Interfaces;

namespace TalentHub.Admin.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        public List<Empleado> ObtenerTodos()
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

                using var cmd = new SqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

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
                        Area = new Area
                        {
                            Id = (int)reader["AreaId"],
                            Nombre = reader["AreaNombre"].ToString()!
                        },
                        SupervisorId = (int)reader["SupervisorId"],
                        Supervisor = new Supervisor
                        {
                            Id = (int)reader["SupervisorId"],
                            NombreCompleto = reader["SupervisorNombre"].ToString()!
                        }
                    });
                }
            }

            return empleados;
        }

        public Empleado ObtenerPorId(int id)
        {
            Empleado empleado = new();

            using var conn = SqlHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Empleados WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
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

            return empleado;
        }

        public void Crear(Empleado empleado)
        {
            using var conn = SqlHelper.GetConnection();
            conn.Open();

            string sql = @"
            INSERT INTO Empleados (NombreCompleto, Cedula, Correo, FechaIngreso, AreaId, SupervisorId)
            VALUES (@Nombre, @Cedula, @Correo, @Fecha, @Area, @Supervisor)";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Nombre", empleado.NombreCompleto);
            cmd.Parameters.AddWithValue("@Cedula", empleado.Cedula);
            cmd.Parameters.AddWithValue("@Correo", empleado.Correo);
            cmd.Parameters.AddWithValue("@Fecha", empleado.FechaIngreso);
            cmd.Parameters.AddWithValue("@Area", empleado.AreaId);
            cmd.Parameters.AddWithValue("@Supervisor", empleado.SupervisorId);

            cmd.ExecuteNonQuery();
        }

        public void Actualizar(Empleado empleado)
        {
            using var conn = SqlHelper.GetConnection();
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

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", empleado.Id);
            cmd.Parameters.AddWithValue("@Nombre", empleado.NombreCompleto);
            cmd.Parameters.AddWithValue("@Cedula", empleado.Cedula);
            cmd.Parameters.AddWithValue("@Correo", empleado.Correo);
            cmd.Parameters.AddWithValue("@Fecha", empleado.FechaIngreso);
            cmd.Parameters.AddWithValue("@Area", empleado.AreaId);
            cmd.Parameters.AddWithValue("@Supervisor", empleado.SupervisorId);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var conn = SqlHelper.GetConnection();
            conn.Open();

            using var cmd = new SqlCommand("DELETE FROM Empleados WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public bool CedulaExiste(string cedula, int? id = null)
        {
            using var conn = SqlHelper.GetConnection();
            conn.Open();

            string sql = "SELECT COUNT(*) FROM Empleados WHERE Cedula = @Cedula";
            if (id != null) sql += " AND Id <> @Id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Cedula", cedula);
            if (id != null) cmd.Parameters.AddWithValue("@Id", id);

            return (int)cmd.ExecuteScalar() > 0;
        }
    }
}
