using Microsoft.AspNetCore.Mvc.Rendering;
using PruebaTecnicaInversionesAliadas.Models;

namespace PruebaTecnicaInversionesAliadas.ViewModel
{
    public class EmpleadoVM
    {
        public Empleado oEmpleado { get; set; }
        public List<SelectListItem> oListaCargo { get; set; }
    }
}
