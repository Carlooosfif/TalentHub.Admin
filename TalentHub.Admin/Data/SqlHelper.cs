using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TalentHub.Admin.Data
{
    public static class SqlHelper
    {
        private static string? _connectionString;

        // Se llama una vez desde Program.cs
        public static void Initialize(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Se usa en los controladores para obtener una conexión lista
        public static SqlConnection GetConnection()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión no está configurada.");
            }

            return new SqlConnection(_connectionString);
        }
    }
}
