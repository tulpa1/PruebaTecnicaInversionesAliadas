using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaInversionesAliadas.Models;
using PruebaTecnicaInversionesAliadas.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace PruebaTecnicaInversionesAliadas.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly crudbasico1Context _DBcontext;

        public HomeController(crudbasico1Context context)
        {
            _DBcontext = context;
        }

        public IActionResult Index()
        {
            List<Empleado> lista = _DBcontext.Empleados.Include(c => c.oCargo).ToList();

            return View(lista);
        }

        [HttpGet]
        public IActionResult Empleado_Detalle(int idEmpleado)
        {
            EmpleadoVM oEmpleadoVM = new EmpleadoVM()
            {
                oEmpleado = new Empleado(),
                oListaCargo = _DBcontext.Cargos.Select(cargo => new SelectListItem()
                {
                    Text = cargo.Descripcion,
                    Value = cargo.IdCargo.ToString()
                }).ToList()
            };

            if(idEmpleado != 0)
            {
                oEmpleadoVM.oEmpleado = _DBcontext.Empleados.Find(idEmpleado);
            }

            return View(oEmpleadoVM);

        }

        [HttpPost]
        public IActionResult Empleado_Detalle(EmpleadoVM empleadoVM)
        {
            if(empleadoVM.oEmpleado.IdEmpleado == 0)
            {
                _DBcontext.Empleados.Add(empleadoVM.oEmpleado);
            }
            else
            {
                _DBcontext.Empleados.Update(empleadoVM.oEmpleado);
            }

            _DBcontext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        
        public IActionResult Eliminar_Empleado(int idempleado)
        {

            if(idempleado != 0)
            { 
                Empleado? modelo = _DBcontext.Empleados.
                                    Where(x => x.IdEmpleado == idempleado).FirstOrDefault();
                
                if(modelo != null)
                {
                    _DBcontext.Empleados.Remove(modelo);
                    _DBcontext.SaveChanges();
                }
            }
           
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Acceso");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
