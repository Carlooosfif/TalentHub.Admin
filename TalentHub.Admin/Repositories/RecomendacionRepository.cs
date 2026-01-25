using Microsoft.Data.SqlClient;
using TalentHub.Admin.Data;
using TalentHub.Admin.Models;

namespace TalentHub.Admin.Repositories.Interfaces
{
    public class RecomendacionRepository : IRecomendacionRepository
    {
        public List<RecomendacionEmpleadoViewModel> ObtenerCandidatosPorArea(string area)
        {
            var lista = new List<RecomendacionEmpleadoViewModel>();

            using var conn = SqlHelper.GetConnection();
            conn.Open();

            string sql = @"
            SELECT 
                e.Id AS EmpleadoId,
                e.NombreCompleto,
                e.Cedula,
                e.Correo,
                e.FechaIngreso,
                ev.ScoreSupervisor
            FROM Empleados e
            INNER JOIN Areas a ON e.AreaId = a.Id
            LEFT JOIN EvaluacionesVacante ev ON ev.EmpleadoId = e.Id
            WHERE a.Nombre = @Area
            ORDER BY 
                ISNULL(ev.ScoreSupervisor, -1) DESC,
                e.FechaIngreso ASC,
                e.NombreCompleto";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Area", area);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new RecomendacionEmpleadoViewModel
                {
                    EmpleadoId = (int)reader["EmpleadoId"],
                    NombreCompleto = reader["NombreCompleto"].ToString()!,
                    Cedula = reader["Cedula"].ToString()!,
                    Correo = reader["Correo"].ToString()!,
                    ScoreSupervisor = reader["ScoreSupervisor"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(reader["ScoreSupervisor"])
                });
            }

            return lista;
        }
    }
}