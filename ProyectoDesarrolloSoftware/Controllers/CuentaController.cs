using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoDesarrolloSoftware.Models;

namespace ProyectoDesarrolloSoftware.Controllers
{
    public class CuentaController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public CuentaController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: /Cuenta/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Si ya esta logueado, redirigue a la vista correspondiente segun el rol
            if (User.Identity.IsAuthenticated)
            {
                return RedirectSegunRol();
            }
            return View();
        }

        // POST: /Cuenta/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
         {
           if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Por favor, complete todos los campos.";
                return View();
            }

          // Verificar si el usuario existe y está bloqueado antes de intentar el login
            var user = await _userManager.FindByNameAsync(username);
            if (user != null && await _userManager.IsLockedOutAsync(user))
              {
                  ViewBag.Error = "Su cuenta está bloqueada. Contacte a un administrador.";
                 return View();
             }
            // Intenta iniciar sesion con el usuario y contraseña
            // IsPersistent: false para que no recuerde la sesión después de cerrar el navegador (cookies)
            // lockoutOnFailure: false para que no bloquee la cuenta si se falla varias veces
            var resultado = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

             if (resultado.Succeeded)
               {
                 return RedirectSegunRol();
              }

               ViewBag.Error = "Usuario o contraseña incorrectos.";
               return View();
            }


        // POST: /Cuenta/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Cuenta");
        }

        // Metodo para redirigir al usuario a la vista que le corresponde segun el rol
        private IActionResult RedirectSegunRol()
        {
            if (User.IsInRole("Administrador"))
            {
                return RedirectToAction("VistaAdministracion", "Administracion");
            }
            else if (User.IsInRole("Medico"))
            {
                return RedirectToAction("VistaMedicos", "Medico");
            }
            else if (User.IsInRole("Paciente"))
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
