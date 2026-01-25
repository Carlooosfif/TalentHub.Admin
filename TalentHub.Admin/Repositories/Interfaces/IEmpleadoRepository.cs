using TalentHub.Admin.Models;

namespace TalentHub.Admin.Repositories.Interfaces
{
    public interface IEmpleadoRepository
    {
        List<Empleado> ObtenerTodos();
        Empleado ObtenerPorId(int id);
        void Crear(Empleado empleado);
        void Actualizar(Empleado empleado);
        void Eliminar(int id);
        bool CedulaExiste(string cedula, int? id = null);
    }
}