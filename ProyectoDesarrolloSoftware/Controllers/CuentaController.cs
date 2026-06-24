using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ViewModels;

namespace ProyectoDesarrolloSoftware.Controllers
{
    public class CuentaController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CuentaController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
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

        // GET: /Cuenta/Registrar  (autoregistro público para Paciente)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Registrar()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectSegunRol();

            return View("RegistroPaciente", new RegistroPacienteViewModel());
        }

        // POST: /Cuenta/Registrar
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistroPacienteViewModel vm)
        {
          
            // Verificar si el correo ya existe en otro usuario
            if (await _userManager.FindByEmailAsync(vm.Email) != null)
                ModelState.AddModelError(nameof(vm.Email), "Ya existe una cuenta registrada con ese correo.");

            // Verificar si la cédula ya existe en otro usuario
            var cedulaExiste = await _context.Users
                .AnyAsync(u => u.Cedula == vm.Cedula && u.Id != vm.Cedula);

            if (cedulaExiste)
            {
                ModelState.AddModelError(nameof(vm.Cedula), "Ya existe un usuario con esa cédula.");
            }

            if (!ModelState.IsValid)
                return View("RegistroPaciente", vm);

            var usuario = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                NombreCompleto = vm.NombreCompleto,
                Cedula = vm.Cedula,
                Perfil = Perfil.Paciente 
            };

            var resultado = await _userManager.CreateAsync(usuario, vm.Password);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                    ModelState.AddModelError("", error.Description);
                return View("RegistroPaciente", vm);
            }

            await _userManager.AddToRoleAsync(usuario, Perfil.Paciente.ToString());

            _context.Pacientes.Add(new Paciente { UsuarioId = usuario.Id });
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(usuario, isPersistent: false);

            TempData["SuccessMessage"] = "¡Registro exitoso! Bienvenido/a.";
            return RedirectToAction("Index", "Home");
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

        // GET: /Cuenta/AccesoDenegado
        // Vista para mostrar cuando un usuario intenta acceder a una pagina sin permisos
        [AllowAnonymous]
        public IActionResult AccesoDenegado(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("AccesoDenegado");
        }
    }
}