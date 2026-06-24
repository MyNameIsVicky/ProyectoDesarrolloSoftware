using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ViewModels;

namespace ProyectoDesarrolloSoftware.Controllers
{
    [Authorize(Roles = "Administrador, Medico")]
    public class MedicoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MedicoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Medico
        public async Task<IActionResult> Index()
        {
            var medicos = await _context.Medicos
                .Include(m => m.MedicosEspecialidades)
                    .ThenInclude(me => me.Especialidad)
                .ToListAsync();

            return View(medicos);
        }
        // GET: Medico/VistaMedicos
        [Authorize(Roles = "Medico")]
        public IActionResult VistaMedicos()
        {
            return View();
        }

        // GET: Medico/Create
        public async Task<IActionResult> Create()
        {
            await CargarEspecialidades();
            return View(new MedicoViewModel());
        }

        // POST: Medico/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicoViewModel medicoVM)
        {
            ModelState.Remove("Medico.MedicosEspecialidades");

            if (string.IsNullOrWhiteSpace(medicoVM.Medico.CedulaFisica))
                ModelState.AddModelError(nameof(medicoVM.Medico.CedulaFisica), "La cédula es obligatoria.");

            var cedulaExiste = await _context.Medicos
                .AnyAsync(m => m.CedulaFisica == medicoVM.Medico.CedulaFisica);

            if (cedulaExiste)
                ModelState.AddModelError(nameof(medicoVM.Medico.CedulaFisica), "Ya existe un médico con esa cédula.");

            if (medicoVM.EspecialidadesIds == null || !medicoVM.EspecialidadesIds.Any())
                ModelState.AddModelError(nameof(medicoVM.EspecialidadesIds), "Seleccione al menos una especialidad.");

            if (!ModelState.IsValid)
            {
                await CargarEspecialidades();
                return View(medicoVM);
            }

            medicoVM.Medico.MedicosEspecialidades = medicoVM.EspecialidadesIds
                .Select(id => new MedicoEspecialidad { EspecialidadId = id })
                .ToList();

            medicoVM.Medico.FotoUrl ??= "https://cdn-icons-png.flaticon.com/512/149/149071.png";

            _context.Medicos.Add(medicoVM.Medico);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Médico creado.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Medico/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var medico = await _context.Medicos
                .Include(m => m.MedicosEspecialidades)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null) return NotFound();

            var vm = new MedicoViewModel
            {
                Medico = medico,
                EspecialidadesIds = medico.MedicosEspecialidades.Select(x => x.EspecialidadId).ToList()
            };

            await CargarEspecialidades();
            return View(vm);
        }

        // POST: Medico/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicoViewModel medicoVM)
        {
            ModelState.Remove("Medico.MedicosEspecialidades");

            if (!ModelState.IsValid)
            {
                await CargarEspecialidades();
                return View(medicoVM);
            }

            var medicoDb = await _context.Medicos
                .Include(m => m.MedicosEspecialidades)
                .FirstOrDefaultAsync(m => m.Id == medicoVM.Medico.Id);

            if (medicoDb == null) return NotFound();

            // Validar cédula duplicada
            var cedulaExiste = await _context.Medicos
                .AnyAsync(m => m.CedulaFisica == medicoVM.Medico.CedulaFisica
                            && m.Id != medicoVM.Medico.Id);

            if (cedulaExiste)
            {
                ModelState.AddModelError(nameof(medicoVM.Medico.CedulaFisica), "Ya existe un médico con esa cédula.");
                await CargarEspecialidades();
                return View(medicoVM);
            }

            medicoDb.NombreCompleto = medicoVM.Medico.NombreCompleto;
            medicoDb.NumeroColegiado = medicoVM.Medico.NumeroColegiado;
            medicoDb.CedulaFisica = medicoVM.Medico.CedulaFisica;
            medicoDb.FotoUrl = medicoVM.Medico.FotoUrl;

            _context.MedicoEspecialidades.RemoveRange(medicoDb.MedicosEspecialidades);

            medicoDb.MedicosEspecialidades = medicoVM.EspecialidadesIds?
                .Select(id => new MedicoEspecialidad
                {
                    MedicoId = medicoDb.Id,
                    EspecialidadId = id
                }).ToList() ?? new List<MedicoEspecialidad>();

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Médico actualizado.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Medico/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var medico = await _context.Medicos
                .Include(m => m.MedicosEspecialidades)
                    .ThenInclude(x => x.Especialidad)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null) return NotFound();

            return View(medico);
        }

        // POST: Medico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null) return NotFound();

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.MedicoId == id);

            _context.Medicos.Remove(medico);
            await _context.SaveChangesAsync();

            if (user != null)
                await _userManager.DeleteAsync(user);

            TempData["SuccessMessage"] = "Médico eliminado.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarEspecialidades()
        {
            ViewBag.Especialidades = await _context.Especialidades
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Nombre
                })
                .ToListAsync();
        }
    }
}