using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaInversionesAliadas.Models;
using PruebaTecnicaInversionesAliadas.ViewModel;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace PruebaTecnicaInversionesAliadas.Controllers
{
    public class AccesoController : Controller
    {
        private readonly crudbasico1Context _DBcontext;

        public AccesoController(crudbasico1Context dbcontext)
        {
            _DBcontext = dbcontext;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(LoginVM modelo)
        {
            if (modelo.Clave != modelo.ConfirmarClave)
            {
                ViewData["mensaje"] = "Las contraseñas no coinciden!";
                return View();
            }

            Usuario usuario = new Usuario()
            {
                NombreCompleto = modelo.NombreCompleto,
                Correo = modelo.Correo,
                Clave = modelo.Clave
            };

            await _DBcontext.Usuarios.AddAsync(usuario);
            await _DBcontext.SaveChangesAsync();

            if (usuario.IdUsuario != 0)
            {
                return RedirectToAction("Login", "Acceso");
            }

            ViewData["mensaje"] = "Error al intentar registrar el usuario!";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM modelo)
        {
            Usuario? usuario_encontrado = await _DBcontext.Usuarios
                                          .Where(
                                            u => u.Correo == modelo.Correo &&
                                            u.Clave == modelo.Clave
                                          ).FirstOrDefaultAsync();

            if (usuario_encontrado == null)
            {
                ViewData["mensaje"] = "Credenciales incorrectas!";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.NombreCompleto)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
                );


            return RedirectToAction("Index", "Home");
        }
    }


}
