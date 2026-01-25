using TalentHub.Admin.Models;
using TalentHub.Admin.Services.Interfaces;
using TalentHub.Admin.Repositories.Interfaces;

namespace TalentHub.Admin.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _repository;

        public EmpleadoService(IEmpleadoRepository repository)
        {
            _repository = repository;
        }

        public List<Empleado> ObtenerEmpleados()
            => _repository.ObtenerTodos();

        public Empleado ObtenerEmpleado(int id)
            => _repository.ObtenerPorId(id);

        public void CrearEmpleado(Empleado empleado)
            => _repository.Crear(empleado);

        public void ActualizarEmpleado(Empleado empleado)
            => _repository.Actualizar(empleado);

        public void EliminarEmpleado(int id)
            => _repository.Eliminar(id);
    }
}
