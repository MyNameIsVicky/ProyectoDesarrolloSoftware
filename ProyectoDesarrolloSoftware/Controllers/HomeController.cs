using Microsoft.AspNetCore.Mvc;
using ProyectoDesarrolloSoftware.Models;
using System.Diagnostics;

namespace ProyectoDesarrolloSoftware.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Si no hay sesión activa, ir directo al login
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }
    }

}
