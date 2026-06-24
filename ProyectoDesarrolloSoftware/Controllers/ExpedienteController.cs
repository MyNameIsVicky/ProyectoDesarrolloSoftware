using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.Expedientes;
using ProyectoDesarrolloSoftware.Models.ViewModels;

namespace ProyectoDesarrolloSoftware.Controllers
{
    // Permiso para que solo el médico pueda agregar/suspender cosas del expediente o escribir notas 
    [Authorize(Roles = "Medico, Administrador")]
    public class ExpedienteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".pdf" };

        public ExpedienteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        private async Task<int?> ObtenerMedicoIdActual()
        {
            var usuario = await _userManager.GetUserAsync(User);
            return usuario?.MedicoId;
        }

        // GET: Expediente/Ver/5
        public async Task<IActionResult> Ver(int pacienteId)
        {
            var paciente = await _context.Pacientes
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == pacienteId);

            if (paciente == null || paciente.Usuario == null) return NotFound();

            var vm = new ExpedienteViewModel
            {
                Paciente = paciente,
                NombrePaciente = paciente.Usuario.NombreCompleto,
                CedulaPaciente = paciente.Usuario.Cedula,

                Padecimientos = await _context.ExpedientePadecimientos
                    .Include(x => x.Padecimiento)
                    .Include(x => x.Medico)
                    .Where(x => x.PacienteId == pacienteId)
                    .OrderByDescending(x => x.FechaAsignacion)
                    .ToListAsync(),

                Tratamientos = await _context.ExpedienteTratamientos
                    .Include(x => x.Tratamiento)
                    .Include(x => x.Medico)
                    .Where(x => x.PacienteId == pacienteId)
                    .OrderByDescending(x => x.FechaAsignacion)
                    .ToListAsync(),

                Medicamentos = await _context.ExpedienteMedicamentos
                    .Include(x => x.Medicamento)
                    .Include(x => x.Medico)
                    .Where(x => x.PacienteId == pacienteId)
                    .OrderByDescending(x => x.FechaAsignacion)
                    .ToListAsync(),

                Examenes = await _context.Examenes
                    .Include(x => x.Medico)
                    .Where(x => x.PacienteId == pacienteId)
                    .OrderByDescending(x => x.FechaSubida)
                    .ToListAsync(),

                HistorialClinico = await _context.HistorialClinicos
                    .Include(x => x.Medico)
                    .Where(x => x.PacienteId == pacienteId)
                    .OrderByDescending(x => x.FechaRegistro)
                    .ToListAsync(),
                
                PadecimientosCatalogo = await _context.Padecimientos
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                    .ToListAsync(),

                TratamientosCatalogo = await _context.Tratamientos
                    .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Nombre })
                    .ToListAsync(),

                MedicamentosCatalogo = await _context.Medicamentos
                    .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.NombreMedicamento })
                    .ToListAsync(),
            };

            return View(vm);
        }

        // POST: Agregar padecimiento al expediente
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarPadecimiento(AsignarPadecimientoViewModel vm)
        {
            var medicoId = await ObtenerMedicoIdActual();
            if (medicoId == null) return Forbid(); // Forbid rechaza la peticion si el usuario no es un médico (Codigo 403)

            if (ModelState.IsValid)
            {
                _context.ExpedientePadecimientos.Add(new ExpedientePadecimiento
                {
                    PacienteId = vm.PacienteId,
                    PadecimientoId = vm.PadecimientoId,
                    MedicoId = medicoId.Value,
                    FechaAsignacion = DateTime.Now,
                    Suspendido = false
                });
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Padecimiento agregado.";
            }
            else
            {
                TempData["ErrorMessage"] = "Seleccione un padecimiento válido.";
            }

            return RedirectToAction(nameof(Ver), new { pacienteId = vm.PacienteId });
        }

        // POST: Suspender padecimiento del expediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuspenderPadecimiento(int id, int pacienteId)
        {
            var registro = await _context.ExpedientePadecimientos.FindAsync(id);
            if (registro == null) return NotFound();

            registro.Suspendido = true;
            registro.FechaSuspension = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Padecimiento suspendido.";
            return RedirectToAction(nameof(Ver), new { pacienteId });
        }

        // POST: Agregar tratamiento al expediente

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarTratamiento(AsignarTratamientoViewModel vm)
        {
            var medicoId = await ObtenerMedicoIdActual();
            if (medicoId == null) return Forbid();

            if (ModelState.IsValid)
            {
                _context.ExpedienteTratamientos.Add(new ExpedienteTratamiento
                {
                    PacienteId = vm.PacienteId,
                    TratamientoId = vm.TratamientoId,
                    MedicoId = medicoId.Value,
                    FechaAsignacion = DateTime.Now,
                    Suspendido = false
                });
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tratamiento agregado.";
            }
            else
            {
                TempData["ErrorMessage"] = "Seleccione un tratamiento válido.";
            }

            return RedirectToAction(nameof(Ver), new { pacienteId = vm.PacienteId });
        }

        // POST: Suspender tratamiento del expediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuspenderTratamiento(int id, int pacienteId)
        {
            var registro = await _context.ExpedienteTratamientos.FindAsync(id);
            if (registro == null) return NotFound();

            registro.Suspendido = true;
            registro.FechaSuspension = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tratamiento suspendido.";
            return RedirectToAction(nameof(Ver), new { pacienteId });
        }

        
        // POST: Agregar medicamento al expediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarMedicamento(AsignarMedicamentoViewModel vm)
        {
            var medicoId = await ObtenerMedicoIdActual();
            if (medicoId == null) return Forbid(); 

            if (ModelState.IsValid)
            {
                _context.ExpedienteMedicamentos.Add(new ExpedienteMedicamento
                {
                    PacienteId = vm.PacienteId,
                    MedicamentoId = vm.MedicamentoId,
                    MedicoId = medicoId.Value,
                    FechaAsignacion = DateTime.Now,
                    Suspendido = false
                });
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medicamento agregado.";
            }
            else
            {
                TempData["ErrorMessage"] = "Seleccione un medicamento válido.";
            }

            return RedirectToAction(nameof(Ver), new { pacienteId = vm.PacienteId });
        }

        // POST: Suspender medicamento del expediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuspenderMedicamento(int id, int pacienteId)
        {
            var registro = await _context.ExpedienteMedicamentos.FindAsync(id);
            if (registro == null) return NotFound();

            registro.Suspendido = true;
            registro.FechaSuspension = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medicamento suspendido.";
            return RedirectToAction(nameof(Ver), new { pacienteId });
        }

        // POST: Agregar examen al expediente
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarExamen(AgregarExamenViewModel vm)
        {
            var medicoId = await ObtenerMedicoIdActual();
            if (medicoId == null) return Forbid(); 

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Verifique la descripción y el archivo adjunto.";
                return RedirectToAction(nameof(Ver), new { pacienteId = vm.PacienteId });
            }

            var extension = Path.GetExtension(vm.Archivo.FileName).ToLowerInvariant();

            // Se verifica que la extensión del archivo sea valida (solo imagenes y PDF)
            if (!ExtensionesPermitidas.Contains(extension))
            {
                TempData["ErrorMessage"] = "Solo se permiten archivos de imagen (.jpg, .png) o PDF.";
                return RedirectToAction(nameof(Ver), new { pacienteId = vm.PacienteId });
            }
            // Crear carpeta si no existe
            // Se guarda en wwwroot/uploads/examenes
            var carpeta = Path.Combine(_env.WebRootPath, "uploads", "examenes");
            Directory.CreateDirectory(carpeta);

            // Genera un nombre unico usando guid y la extensión con la que se subió el archivo
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await vm.Archivo.CopyToAsync(stream);
            }

            _context.Examenes.Add(new ExamenMedico
            {
                PacienteId = vm.PacienteId,
                MedicoId = medicoId.Value,
                Descripcion = vm.Descripcion,
                ArchivoRuta = $"/uploads/examenes/{nombreArchivo}", // ruta que da acceso desde el navegador
                TipoArchivo = extension == ".pdf" ? "pdf" : "imagen",
                FechaSubida = DateTime.Now
            });

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Examen agregado al expediente.";
            return RedirectToAction(nameof(Ver), new { pacienteId = vm.PacienteId });
        }

        // POST: Agregar nota al historial clínico
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarNota(AgregarNotaHistorialViewModel vm)
        {
            var medicoId = await ObtenerMedicoIdActual();
            if (medicoId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "La nota no puede estar vacía.";
                return RedirectToAction(nameof(Ver), new { pacienteId = vm.PacienteId });
            }

            _context.HistorialClinicos.Add(new HistorialClinico
            {
                PacienteId = vm.PacienteId,
                MedicoId = medicoId.Value,
                Nota = vm.Nota,
                FechaRegistro = DateTime.Now
            });

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Nota agregada al historial clínico.";
            return RedirectToAction(nameof(Ver), new { pacienteId = vm.PacienteId });
        }
    }
}