using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Migrations;
using ProyectoDesarrolloSoftware.Models;

namespace ProyectoDesarrolloSoftware.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EspecialidadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EspecialidadController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Especialidad
        public async Task<IActionResult> Index()
        {
            var especialidades = await _context.Especialidades.ToListAsync();
            return View(especialidades);
        }

        // GET: Especialidad/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Especialidad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Especialidad especialidad)
        {
            // Validación en el modelError para ver si ya hay una especialidad con ese nombre
            if (await _context.Especialidades.AnyAsync(e => e.Nombre == especialidad.Nombre))
            {
                ModelState.AddModelError("Nombre", "Ya existe una especialidad registrada con este nombre.");
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(especialidad);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Especialidad creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(especialidad);
        }

        // GET: Especialidad/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var especialidad = await _context.Especialidades.FindAsync(id);
            if (especialidad == null) return NotFound();

            return View(especialidad);
        }

        // POST: Especialidad/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Especialidad especialidad)
        {
            if (id != especialidad.Id) return NotFound();

            // Validación en el modelError para ver si ya hay una especialidad con ese nombre (excluyendo el registro actual)
            if (await _context.Especialidades.AnyAsync(e => e.Nombre == especialidad.Nombre && e.Id != id))
            {
                ModelState.AddModelError("Nombre", "Ya existe otra especialidad con este mismo nombre.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(especialidad);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Especialidad actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EspecialidadExists(especialidad.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(especialidad);
        }

        // GET: Especialidad/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var especialidad = await _context.Especialidades.FirstOrDefaultAsync(m => m.Id == id);
            if (especialidad == null) return NotFound();

            return View(especialidad);
        }

        // POST: Especialidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var especialidad = await _context.Especialidades.FindAsync(id);

            if (especialidad != null)
            {
                // Validación para evitar borrar si esta asociada a algun medico
                bool estaAsociada = await _context.Set<MedicoEspecialidad>().AnyAsync(me => me.EspecialidadId == id);

                if (estaAsociada)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar la especialidad porque está asignada a uno o más médicos.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Especialidades.Remove(especialidad);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Especialidad eliminada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EspecialidadExists(int id)
        {
            return _context.Especialidades.Any(e => e.Id == id);
        }

    }
}