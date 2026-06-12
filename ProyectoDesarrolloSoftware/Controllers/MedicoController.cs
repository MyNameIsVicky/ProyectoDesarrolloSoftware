using Microsoft.AspNetCore.Mvc;

namespace ProyectoDesarrolloSoftware.Controllers
{
    public class MedicoController : Controller
    {
        public IActionResult VistaMedicos()
        {
            return View();
        }
    }
}
