using Microsoft.Data.SqlClient;
using TalentHub.Admin.Data;
using TalentHub.Admin.Models;
using TalentHub.Admin.Repositories.Interfaces;

namespace TalentHub.Admin.Repositories
{
    public class VacanteRepository : IVacanteRepository
    {
        public Vacante? ObtenerPorId(int id)
        {
            using var conn = SqlHelper.GetConnection();
            conn.Open();

            string sql = @"
            SELECT Id, Titulo, Area, Ubicacion, Estado, FechaPublicacion
            FROM Vacantes
            WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Vacante
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