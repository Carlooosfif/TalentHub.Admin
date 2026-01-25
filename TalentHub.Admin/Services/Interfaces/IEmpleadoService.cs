using TalentHub.Admin.Models;

namespace TalentHub.Admin.Services.Interfaces
{
    public interface IEmpleadoService
    {
        List<Empleado> ObtenerEmpleados();
        Empleado ObtenerEmpleado(int id);
        void CrearEmpleado(Empleado empleado);
        void ActualizarEmpleado(Empleado empleado);
        void EliminarEmpleado(int id);
    }
}
