using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using System.Security.Claims;

namespace ProyectoDesarrolloSoftware.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PacienteApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PacienteApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Extrae el PacienteId del token JWT para asegurar que cada paciente
        // solo vea su propia información
        private int GetPacienteId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim == null) throw new UnauthorizedAccessException();
            return int.Parse(claim);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET api/paciente/padecimientos
        // Retorna los padecimientos asignados al paciente autenticado
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("padecimientos")]
        public async Task<IActionResult> GetPadecimientos()
        {
            var pacienteId = GetPacienteId();

            var datos = await _context.ExpedientePadecimientos
                .Where(e => e.PacienteId == pacienteId)
                .Include(e => e.Padecimiento)
                .Include(e => e.Medico)
                .Select(e => new
                {
                    e.Id,
                    e.PacienteId,
                    e.PadecimientoId,
                    padecimientoNombre = e.Padecimiento.Nombre,
                    padecimientoDescripcion = e.Padecimiento.Descripcion,
                    e.MedicoId,
                    medicoNombre = e.Medico.NombreCompleto,
                    e.FechaAsignacion,
                    e.Activo,
                    e.fechaSuspension
                })
                .OrderByDescending(e => e.FechaAsignacion)
                .ToListAsync();

            return Ok(datos);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET api/paciente/tratamientos
        // Retorna los tratamientos asignados al paciente autenticado
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("tratamientos")]
        public async Task<IActionResult> GetTratamientos()
        {
            var pacienteId = GetPacienteId();

            var datos = await _context.ExpedienteTratamientos
                .Where(e => e.PacienteId == pacienteId)
                .Include(e => e.Tratamiento)
                .Include(e => e.Medico)
                .Select(e => new
                {
                    e.Id,
                    e.PacienteId,
                    e.TratamientoId,
                    tratamientoNombre = e.Tratamiento.Nombre,
                    tratamientoDescripcion = e.Tratamiento.Descripcion,
                    e.MedicoId,
                    medicoNombre = e.Medico.NombreCompleto,
                    e.FechaAsignacion,
                    e.Suspendido,
                    e.FechaSuspension
                })
                .OrderByDescending(e => e.FechaAsignacion)
                .ToListAsync();

            return Ok(datos);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET api/paciente/medicamentos
        // Retorna los medicamentos asignados al paciente autenticado
        // NOTA: El modelo ExpedienteMedicamento usa TratamientoId en lugar de
        // MedicamentoId (parece un error de copia en el modelo original).
        // Ajustar si se corrige el modelo.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("medicamentos")]
        public async Task<IActionResult> GetMedicamentos()
        {
            var pacienteId = GetPacienteId();

            var datos = await _context.ExpedienteMedicamentos
                .Where(e => e.PacienteId == pacienteId)
                .Include(e => e.Tratamiento)
                .Include(e => e.Medico)
                .Select(e => new
                {
                    e.Id,
                    e.PacienteId,
                    e.TratamientoId,
                    tratamientoNombre = e.Tratamiento.Nombre,
                    e.MedicoId,
                    medicoNombre = e.Medico.NombreCompleto,
                    e.FechaAsignacion,
                    e.Suspendido,
                    e.FechaSuspension
                })
                .OrderByDescending(e => e.FechaAsignacion)
                .ToListAsync();

            return Ok(datos);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET api/paciente/historial
        // Retorna el listado de notas del historial clínico
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("historial")]
        public async Task<IActionResult> GetHistorial()
        {
            var pacienteId = GetPacienteId();

            var datos = await _context.HistorialClinicos
                .Where(h => h.PacienteId == pacienteId)
                .Include(h => h.Medico)
                .Select(h => new
                {
                    h.Id,
                    h.PacienteId,
                    h.MedicoId,
                    medicoNombre = h.Medico.NombreCompleto,
                    h.Nota,
                    h.FechaRegistro
                })
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();

            return Ok(datos);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET api/paciente/historial/{id}
        // Retorna el detalle de una nota clínica específica
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("historial/{id}")]
        public async Task<IActionResult> GetDetalleNota(int id)
        {
            var pacienteId = GetPacienteId();

            var nota = await _context.HistorialClinicos
                .Where(h => h.Id == id && h.PacienteId == pacienteId)
                .Include(h => h.Medico)
                .Select(h => new
                {
                    h.Id,
                    h.PacienteId,
                    h.MedicoId,
                    medicoNombre = h.Medico.NombreCompleto,
                    h.Nota,
                    h.FechaRegistro
                })
                .FirstOrDefaultAsync();

            if (nota == null) return NotFound();
            return Ok(nota);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET api/paciente/examenes
        // Retorna el listado de exámenes de laboratorio del paciente
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("examenes")]
        public async Task<IActionResult> GetExamenes()
        {
            var pacienteId = GetPacienteId();

            var datos = await _context.ExamenesLaboratorio
                .Where(e => e.PacienteId == pacienteId)
                .Include(e => e.Medico)
                .Select(e => new
                {
                    e.Id,
                    e.PacienteId,
                    e.MedicoId,
                    medicoNombre = e.Medico.NombreCompleto,
                    e.Nombre,
                    e.Descripcion,
                    e.FechaRegistro,
                    e.NombreArchivo
                })
                .OrderByDescending(e => e.FechaRegistro)
                .ToListAsync();

            return Ok(datos);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET api/paciente/examenes/{id}/archivo
        // Descarga el PDF del examen de laboratorio
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("examenes/{id}/archivo")]
        public async Task<IActionResult> DescargarExamen(int id)
        {
            var pacienteId = GetPacienteId();

            var examen = await _context.ExamenesLaboratorio
                .FirstOrDefaultAsync(e => e.Id == id && e.PacienteId == pacienteId);

            if (examen == null) return NotFound();

            if (!System.IO.File.Exists(examen.RutaArchivo))
                return NotFound("El archivo no se encontró en el servidor.");

            var bytes = await System.IO.File.ReadAllBytesAsync(examen.RutaArchivo);
            var nombreDescarga = string.IsNullOrEmpty(examen.NombreArchivo)
                ? Path.GetFileName(examen.RutaArchivo)
                : examen.NombreArchivo;

            return File(bytes, "application/pdf", nombreDescarga);
        }
    }
}