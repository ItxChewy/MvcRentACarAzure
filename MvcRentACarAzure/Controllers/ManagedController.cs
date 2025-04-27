using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcRentACarAzure.Services;
using NugetRentACar.Models;

namespace MvcRentACarAzure.Controllers
{
    public class ManagedController : Controller
    {
        private IServiceRentACar service;
        public ManagedController(IServiceRentACar service)
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            string token = await this.service.GetTokenAsync(model.Email, model.Password);
            Usuario usuario = await this.service.LoginAsync(model.Email, model.Password);

            if (token != null)
            {
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity =
                    new ClaimsIdentity(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name, ClaimTypes.Role
                        );
                Claim claimUserName =
                new Claim(ClaimTypes.Name, usuario.Nombre);
                identity.AddClaim(claimUserName);
                Claim claimRole =
                    new Claim(ClaimTypes.Role, usuario.IdRolUsuario.ToString());
                identity.AddClaim(claimRole);
                Claim claimId =
                    new Claim("id", usuario.IdUsuario.ToString());
                identity.AddClaim(claimId);

                identity.AddClaim(new Claim("TOKEN", token));
                ClaimsPrincipal userPrincipal =
                new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });
                string controller = TempData["controller"]?.ToString();
                string action = TempData["action"]?.ToString();

                if (string.IsNullOrEmpty(controller) || usuario.IdRolUsuario == 1)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (TempData["id"] != null)
                {
                    string id =
                        TempData["id"].ToString();
                    return RedirectToAction(action, controller
                        , new { id = id });
                }
                return RedirectToAction(action, controller );
            }
            else
            {
                ViewData["MENSAJE"] = "Nombre y/o contraseña erroneas";
                TempData["ErrorMessage"] = "Nombre y/o contraseña incorrecta/s";
                return View();
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            TempData.Clear();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Register()
        {
            ViewData["roles"] = await this.service.GetRolesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register
            (string nombre, string email, string password, int idrol, string telefono
            , string? apellidos, string? dni
             , DateTime? fechanacimiento, string? direccion, string? passpecial
            , string? nombreempresa)
        {
            if (fechanacimiento.HasValue)
            {
                DateTime today = DateTime.Today;

                int age = today.Year - fechanacimiento.Value.Year;
                if (fechanacimiento.Value.Date > today.AddYears(-age)) age--;

                if (age < 18)
                {
                    TempData["ErrorMessage"] = "Error.Usted debe ser de mayor de edad para acceder a la web.";
                    ViewData["roles"] = await this.service.GetRolesAsync();
                    return View();
                }
            }

            bool isRegistered = await this.service.RegisterUsuarioAsync(nombre, email, password, idrol, telefono, apellidos, dni, fechanacimiento, direccion, passpecial, nombreempresa);

            if (isRegistered)
            {
                return RedirectToAction("Login");
            }
            else
            {
                TempData["ErrorMessage"] = "Error al registrar usuario.Intentelo de nuevo y verifique los campos.";
                ViewData["roles"] = await this.service.GetRolesAsync();
                return View();
            }
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
