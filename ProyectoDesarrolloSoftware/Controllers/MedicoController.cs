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
    [Authorize(Roles = "Administrador")]
    public class MedicoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MedicoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var medicos = await _context.Medicos
                .Include(m => m.MedicosEspecialidades)
                    .ThenInclude(me => me.Especialidad)
                .ToListAsync();

            return View(medicos);
        }

        public async Task<IActionResult> Create()
        {
            await CargarEspecialidades();
            return View(new MedicoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicoViewModel medicoVM)
        {
            ModelState.Remove("Medico.MedicosEspecialidades");

            if (await _userManager.FindByNameAsync(medicoVM.Medico.NombreCompleto) != null)
                ModelState.AddModelError("", "El usuario ya existe.");

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

            if (string.IsNullOrEmpty(medicoVM.Medico.FotoUrl))
                medicoVM.Medico.FotoUrl = "https://cdn-icons-png.flaticon.com/512/149/149071.png";

            _context.Medicos.Add(medicoVM.Medico);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

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

            medicoDb.NombreCompleto = medicoVM.Medico.NombreCompleto;
            medicoDb.NumeroColegiado = medicoVM.Medico.NumeroColegiado;
            medicoDb.CedulaFisica = medicoVM.Medico.CedulaFisica;
            medicoDb.FotoUrl = medicoVM.Medico.FotoUrl;

            medicoDb.MedicosEspecialidades.Clear();

            if (medicoVM.EspecialidadesIds != null)
            {
                medicoDb.MedicosEspecialidades = medicoVM.EspecialidadesIds
                    .Select(id => new MedicoEspecialidad
                    {
                        MedicoId = medicoDb.Id,
                        EspecialidadId = id
                    }).ToList();
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Médico actualizado.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var medico = await _context.Medicos
                .Include(m => m.MedicosEspecialidades)
                    .ThenInclude(x => x.Especialidad)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null) return NotFound();

            return View(medico);
        }

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