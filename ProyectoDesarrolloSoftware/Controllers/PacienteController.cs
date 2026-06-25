using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ViewModels;

namespace ProyectoDesarrolloSoftware.Controllers
{
    // NOTA: este controller asume que en tu ApplicationDbContext los DbSet se llaman:
    // Pacientes, ExpedientePadecimientos, ExpedienteTratamientos, ExpedienteMedicamentos,
    // Examenes y HistorialesClinicos. Ajusta los nombres si en tu contexto son distintos.
    [Authorize(Roles = "Administrador,Medico")]
    public class PacienteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PacienteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Paciente
        
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
        public IActionResult Create()
        {
            return View(new PacienteViewModel());
        }

        // POST: Paciente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteViewModel vm)
        {
            ModelState.Remove("Paciente.UsuarioId");

            // Validaciones basicas

            if (await _userManager.FindByEmailAsync(vm.Email) != null)
                ModelState.AddModelError(nameof(vm.Email), "Ya existe un usuario con ese correo.");

            if (string.IsNullOrWhiteSpace(vm.Password))
                ModelState.AddModelError(nameof(vm.Password), "La contraseña es obligatoria.");

            if (!ModelState.IsValid)
                return View(vm);

            var usuario = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
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

            await _userManager.AddToRoleAsync(usuario, "Paciente");

            _context.Pacientes.Add(new Paciente { UsuarioId = usuario.Id });
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Paciente registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Paciente/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var paciente = await _context.Pacientes
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paciente == null || paciente.Usuario == null) return NotFound();

            var vm = new PacienteViewModel
            {
                Paciente = paciente,
                UserName = paciente.Usuario.UserName ?? string.Empty,
                NombreCompleto = paciente.Usuario.NombreCompleto,
                Cedula = paciente.Usuario.Cedula,
                Email = paciente.Usuario.Email ?? string.Empty
            };

            return View(vm);
        }

        // POST: Paciente/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PacienteViewModel vm)
        {
            ModelState.Remove(nameof(vm.Password));
            

            if (!ModelState.IsValid)
                return View(vm);

            var paciente = await _context.Pacientes
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == vm.Paciente.Id);

            if (paciente == null || paciente.Usuario == null) return NotFound();

            paciente.Usuario.NombreCompleto = vm.NombreCompleto;
            paciente.Usuario.Cedula = vm.Cedula;
            paciente.Usuario.UserName = vm.UserName;
           

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Paciente actualizado.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Paciente/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var paciente = await _context.Pacientes
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paciente == null) return NotFound();
            return View(paciente);
        }

        // POST: Paciente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null) return NotFound();

            var usuario = await _userManager.FindByIdAsync(paciente.UsuarioId);

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();

            if (usuario != null)
                await _userManager.DeleteAsync(usuario);

            TempData["SuccessMessage"] = "Paciente eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}