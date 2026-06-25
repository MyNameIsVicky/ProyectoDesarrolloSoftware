using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ViewModels;

namespace ProyectoDesarrolloSoftware.Controllers
{
     [Authorize]
    [Authorize]
    public class PacienteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PacienteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrador,Medico")]
        public async Task<IActionResult> Index(string? busqueda)
        {
            var pacientes = await _context.Pacientes
                .Include(p => p.Usuario)
                .ToListAsync();

            // Última fecha de atención
            // Se usa la fecha más reciente de cualquier acción en el expediente del paciente: padecimientos, tratamientos, medicamentos, exámenes o notas clínicas.
            var ultimaPadecimiento = await _context.ExpedientePadecimientos
                .GroupBy(x => x.PacienteId)
                .Select(g => new { PacienteId = g.Key, Fecha = g.Max(x => x.FechaAsignacion) })
                .ToDictionaryAsync(x => x.PacienteId, x => x.Fecha);

            var ultimoTratamiento = await _context.ExpedienteTratamientos
                .GroupBy(x => x.PacienteId)
                .Select(g => new { PacienteId = g.Key, Fecha = g.Max(x => x.FechaAsignacion) })
                .ToDictionaryAsync(x => x.PacienteId, x => x.Fecha);

            var ultimoMedicamento = await _context.ExpedienteMedicamentos
                .GroupBy(x => x.PacienteId)
                .Select(g => new { PacienteId = g.Key, Fecha = g.Max(x => x.FechaAsignacion) })
                .ToDictionaryAsync(x => x.PacienteId, x => x.Fecha);

            var ultimoExamen = await _context.Examenes
                .GroupBy(x => x.PacienteId)
                .Select(g => new { PacienteId = g.Key, Fecha = g.Max(x => x.FechaSubida) })
                .ToDictionaryAsync(x => x.PacienteId, x => x.Fecha);

            var ultimaNota = await _context.HistorialClinicos
                .GroupBy(x => x.PacienteId)
                .Select(g => new { PacienteId = g.Key, Fecha = g.Max(x => x.FechaRegistro) })
                .ToDictionaryAsync(x => x.PacienteId, x => x.Fecha);

            var lista = pacientes.Select(p =>
            {
                var fechas = new[]
                {
                    ultimaPadecimiento.GetValueOrDefault(p.Id),
                    ultimoTratamiento.GetValueOrDefault(p.Id),
                    ultimoMedicamento.GetValueOrDefault(p.Id),
                    ultimoExamen.GetValueOrDefault(p.Id),
                    ultimaNota.GetValueOrDefault(p.Id)
                };
                var maxFecha = fechas.Max();

                return new PacienteListItemViewModel
                {
                    Id = p.Id,
                    NombreCompleto = p.Usuario?.NombreCompleto ?? "",
                    Cedula = p.Usuario?.Cedula ?? "",
                    UltimaAtencion = maxFecha == default ? null : maxFecha
                };
            })
            .OrderByDescending(x => x.UltimaAtencion ?? DateTime.MinValue)
            .ToList();

            ViewBag.Busqueda = busqueda;
            return View(lista);
        }

        // GET: Paciente/Create
        [Authorize(Roles = "Administrador,Medico")]
        public IActionResult Create()
        {
            return View(new UsuarioViewModel());
        }

        // POST: Paciente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "Administrador,Medico")]
        public async Task<IActionResult> Create(UsuarioViewModel vm)
        {

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(vm.Password))
                ModelState.AddModelError(nameof(vm.Password), "La contraseña es obligatoria.");


            if (string.IsNullOrWhiteSpace(vm.Correo))
                ModelState.AddModelError(nameof(vm.Correo), "El correo es obligatorio.");

            if (string.IsNullOrWhiteSpace(vm.UserName))
                ModelState.AddModelError(nameof(vm.UserName), "El nombre de usuario es obligatorio.");

            // Duplicados
            var cedulaExiste = await _context.Users
                .AnyAsync(u => u.Cedula == vm.Cedula);

            if (cedulaExiste)
                ModelState.AddModelError(nameof(vm.Cedula), "Ya existe un usuario con esa cédula.");

            var emailExiste = await _context.Users
                .AnyAsync(u => u.Email == vm.Correo);

            if (emailExiste)
                ModelState.AddModelError(nameof(vm.Correo), "Ya existe un usuario con ese correo.");

            var usernameExiste = await _context.Users
                .AnyAsync(u => u.UserName == vm.UserName);

            if (usernameExiste)
                ModelState.AddModelError(nameof(vm.UserName), "Ya existe un usuario con ese nombre de usuario.");

            if (!ModelState.IsValid)
                return View(vm);

            var usuario = new ApplicationUser
            {
                UserName = vm.UserName,
                Email = vm.Correo,
                NombreCompleto = vm.NombreCompleto,
                Cedula = vm.Cedula,
                Perfil = Perfil.Paciente
            };

            var resultado = await _userManager.CreateAsync(usuario, vm.Password!);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(vm);
            }

            await _userManager.AddToRoleAsync(usuario, Perfil.Paciente.ToString());

            _context.Pacientes.Add(new Paciente { UsuarioId = usuario.Id });
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Paciente registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Paciente: ver su propio expediente (solo lectura)
        [Authorize(Roles = "Paciente")]
        public async Task<IActionResult> MiExpediente()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return NotFound();

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.UsuarioId == usuario.Id);
            if (paciente == null) return NotFound();

            var vm = new MiExpedienteViewModel
            {
                NombreCompleto = usuario.NombreCompleto,
                Cedula = usuario.Cedula,
                Correo = usuario.Email ?? "",
                UserName = usuario.UserName ?? "",

                Padecimientos = await _context.ExpedientePadecimientos
                    .Where(x => x.PacienteId == paciente.Id)
                    .Include(x => x.Padecimiento)
                    .Include(x => x.Medico)
                    .OrderByDescending(x => x.FechaAsignacion)
                    .ToListAsync(),

                Tratamientos = await _context.ExpedienteTratamientos
                    .Where(x => x.PacienteId == paciente.Id)
                    .Include(x => x.Tratamiento)
                    .Include(x => x.Medico)
                    .OrderByDescending(x => x.FechaAsignacion)
                    .ToListAsync(),

                Medicamentos = await _context.ExpedienteMedicamentos
                    .Where(x => x.PacienteId == paciente.Id)
                    .Include(x => x.Medicamento)
                    .Include(x => x.Medico)
                    .OrderByDescending(x => x.FechaAsignacion)
                    .ToListAsync(),

                Examenes = await _context.Examenes
                    .Where(x => x.PacienteId == paciente.Id)
                    .Include(x => x.Medico)
                    .OrderByDescending(x => x.FechaSubida)
                    .ToListAsync(),

                HistorialClinico = await _context.HistorialClinicos
                    .Where(x => x.PacienteId == paciente.Id)
                    .Include(x => x.Medico)
                    .OrderByDescending(x => x.FechaRegistro)
                    .ToListAsync()
            };

            return View(vm);
        }

    }
}