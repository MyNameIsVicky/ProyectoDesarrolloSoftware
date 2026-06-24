using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models.ModuloMedicina;

namespace ProyectoDesarrolloSoftware.Controllers
{
    [Authorize(Roles = "Administrador, Medico")]
    public class TratamientoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TratamientoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tratamiento
        public async Task<IActionResult> Index()
        {
            var tratamientos = await _context.Tratamientos.ToListAsync();
            return View(tratamientos);
        }

        // GET: Tratamiento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tratamiento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] Tratamiento tratamiento)
        {
            // Validación para ver si ya hay un tratamiento con ese nombre
            if (await _context.Tratamientos.AnyAsync(t => t.Nombre == tratamiento.Nombre))
            {
                ModelState.AddModelError("Nombre", "Ya existe un tratamiento registrado con este nombre.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(tratamiento);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tratamiento creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(tratamiento);
        }

        // GET: Tratamiento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tratamiento = await _context.Tratamientos.FindAsync(id);
            if (tratamiento == null) return NotFound();

            return View(tratamiento);
        }

        // POST: Tratamiento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] Tratamiento tratamiento)
        {
            if (id != tratamiento.Id) return NotFound();

            // Validación para ver si ya hay otro tratamiento con ese mismo nombre (excluyendo el actual)
            if (await _context.Tratamientos.AnyAsync(t => t.Nombre == tratamiento.Nombre && t.Id != id))
            {
                ModelState.AddModelError("Nombre", "Ya existe otro tratamiento con este mismo nombre.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tratamiento);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Tratamiento actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TratamientoExists(tratamiento.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tratamiento);
        }

        // GET: Tratamiento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tratamiento = await _context.Tratamientos.FirstOrDefaultAsync(m => m.Id == id);
            if (tratamiento == null) return NotFound();

            return View(tratamiento);
        }

        // POST: Tratamiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tratamiento = await _context.Tratamientos.FindAsync(id);

            if (tratamiento != null)
            {
                _context.Tratamientos.Remove(tratamiento);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tratamiento eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TratamientoExists(int id)
        {
            return _context.Tratamientos.Any(e => e.Id == id);
        }
    }
}
