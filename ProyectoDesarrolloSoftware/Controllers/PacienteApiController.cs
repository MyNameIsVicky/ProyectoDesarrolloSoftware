using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using ProyectoDesarrolloSoftware.Data;
using System.Security.Claims;

namespace ProyectoDesarrolloSoftware.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class PacienteApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PacienteApiController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private string GetPacienteId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("sub")?.Value;

            if (claim == null) throw new UnauthorizedAccessException();

            return claim; 
        }

     
        // GET api/paciente/padecimientos
     
        [HttpGet("padecimientos")]
        public async Task<IActionResult> GetPadecimientos()
        {
          
            var usuarioId = GetPacienteId();

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            if (paciente == null)
            {
                return NotFound(new { message = "No se encontró el expediente del paciente asociado a esta cuenta." });
            }

          
            var datos = await _context.ExpedientePadecimientos
                .Where(e => e.PacienteId == paciente.Id) 
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
                    e.Suspendido,
                    e.FechaSuspension
                })
                .OrderByDescending(e => e.FechaAsignacion)
                .ToListAsync();

            return Ok(datos);
        }

     
        // GET api/paciente/tratamientos
      
        [HttpGet("tratamientos")]
        public async Task<IActionResult> GetTratamientos()
        {
            var usuarioId = GetPacienteId();

           
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            var datos = await _context.ExpedienteTratamientos
                .Where(e => e.PacienteId == paciente.Id)
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

     
        // GET api/paciente/medicamentos
 
        [HttpGet("medicamentos")]
       
        public async Task<IActionResult> GetMedicamentos()
        {
        
            var usuarioId = GetPacienteId();

   
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            if (paciente == null)
            {
                return NotFound(new { message = "No se encontró el expediente del paciente asociado a esta cuenta." });
            }

     
            var datos = await _context.ExpedienteMedicamentos
                .Where(e => e.PacienteId == paciente.Id)
                .Include(e => e.Medico)
                .Include(e => e.Medicamento) 
                .Select(e => new
                {
                    e.Id,
                    e.PacienteId,
                    e.MedicoId,
                    medicoNombre = e.Medico.NombreCompleto,
                    e.FechaAsignacion,
                    e.Suspendido,
                    e.FechaSuspension,

             
                    medicamentoNombre = e.Medicamento != null ? e.Medicamento.NombreMedicamento : "Medicamento",
                   
                })
                .OrderByDescending(e => e.FechaAsignacion)
                .ToListAsync();

            return Ok(datos);
        }

        // GET api/paciente/historial
   
        [HttpGet("historial")]
        public async Task<IActionResult> GetHistorial()
        {
            var usuarioId = GetPacienteId();

          
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            var datos = await _context.HistorialClinicos
                .Where(h => h.PacienteId == paciente.Id)
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

      
        // GET api/paciente/historial/{id}
   
        [HttpGet("historial/{id}")]
        public async Task<IActionResult> GetDetalleNota(int id)
        {
            var usuarioId = GetPacienteId();

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            var nota = await _context.HistorialClinicos
                .Where(h => h.Id == id && h.PacienteId == paciente.Id)
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

      
        // GET api/paciente/examenes
        // Retorna el listado de exámenes de laboratorio del paciente
       
        [HttpGet("examenes")]
        public async Task<IActionResult> GetExamenes()
        {
            try
            {
                var usuarioId = GetPacienteId();

                var paciente = await _context.Pacientes
                    .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

                
                if (paciente == null)
                    return NotFound("El usuario actual no está registrado como un paciente válido.");

                var datos = await _context.Examenes
                    .Where(e => e.PacienteId == paciente.Id)
                    .Include(e => e.Medico)
                    .Select(e => new
                    {
                        e.Id,
                        e.PacienteId,
                        e.MedicoId,
                      
                        medicoNombre = e.Medico != null ? e.Medico.NombreCompleto : "Médico no asignado",
                        e.Descripcion,
                        e.FechaSubida,
                       
                        tipoArchivo = e.TipoArchivo ?? "pdf"
                    })
                    .OrderByDescending(e => e.FechaSubida)
                    .ToListAsync();

                return Ok(datos);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"[Error Exámenes]: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }


        // GET api/paciente/examenes/{id}/archivo
        // Descarga el PDF del examen de laboratorio
        [HttpGet("examenes/{id}/archivo")]
        public async Task<IActionResult> DescargarExamen(int id)
        {
            var usuarioId = GetPacienteId();

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            if (paciente == null) return NotFound();

            var examen = await _context.Examenes
                .FirstOrDefaultAsync(e => e.Id == id && e.PacienteId == paciente.Id);

            if (examen == null) return NotFound();

           
            var rutaRelativaLimpia = examen.ArchivoRuta.TrimStart('/');
            var rutaFisicaCompleta = Path.Combine(_env.WebRootPath, rutaRelativaLimpia);

            if (!System.IO.File.Exists(rutaFisicaCompleta))
                return NotFound("El archivo no se encontró físicamente en el servidor.");

      
            var extension = Path.GetExtension(rutaFisicaCompleta).ToLowerInvariant();
            string contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            var bytes = await System.IO.File.ReadAllBytesAsync(rutaFisicaCompleta);
            var nombreDescarga = Path.GetFileName(rutaFisicaCompleta);

           
            return File(bytes, contentType, nombreDescarga);
        }
    }
}
