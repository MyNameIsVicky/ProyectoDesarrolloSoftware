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

        // Médico y Administrador: ver listado con última fecha de atención
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