using Microsoft.AspNetCore.Mvc;
using TalentHub.Admin.Controllers;
using TalentHub.Admin.Models;
using TalentHub.Admin.Services.Interfaces;

namespace TalentHub.Admin.Controllers
{
    public class EmpleadosController : BaseController
    {
        private readonly IEmpleadoService _empleadoService;

        public EmpleadosController(IEmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }

        public IActionResult Index()
        {
            var r = Proteger();
            if (r != null) return r;
            var empleados = _empleadoService.ObtenerEmpleados();
            return View(empleados);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Empleado empleado)
        {
            if (!ModelState.IsValid) return View(empleado);

            _empleadoService.CrearEmpleado(empleado);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var empleado = _empleadoService.ObtenerEmpleado(id);
            return View(empleado);
        }

        [HttpPost]
        public IActionResult Edit(Empleado empleado)
        {
            if (!ModelState.IsValid) return View(empleado);

            _empleadoService.ActualizarEmpleado(empleado);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _empleadoService.EliminarEmpleado(id);
            return RedirectToAction("Index");
        }
    }
}