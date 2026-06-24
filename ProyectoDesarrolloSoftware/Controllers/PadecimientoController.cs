using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models.ModuloMedicina;

namespace ProyectoDesarrolloSoftware.Controllers
{
    [Authorize(Roles = "Administrador, Medico")]
    public class PadecimientoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PadecimientoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Padecimiento
        public async Task<IActionResult> Index()
        {
            var padecimientos = await _context.Padecimientos.ToListAsync();
            return View(padecimientos);
        }

        // GET: Padecimiento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Padecimiento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] Padecimiento padecimiento)
        {
            // Validación para ver si ya hay un padecimiento con ese nombre
            if (await _context.Padecimientos.AnyAsync(p => p.Nombre == padecimiento.Nombre))
            {
                ModelState.AddModelError("Nombre", "Ya existe un padecimiento registrado con este nombre.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(padecimiento);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Padecimiento creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(padecimiento);
        }

        // GET: Padecimiento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var padecimiento = await _context.Padecimientos.FindAsync(id);
            if (padecimiento == null) return NotFound();

            return View(padecimiento);
        }

        // POST: Padecimiento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] Padecimiento padecimiento)
        {
            if (id != padecimiento.Id) return NotFound();

            // Validación para ver si ya hay otro padecimiento con ese mismo nombre (excluyendo el registro actual)
            if (await _context.Padecimientos.AnyAsync(p => p.Nombre == padecimiento.Nombre && p.Id != id))
            {
                ModelState.AddModelError("Nombre", "Ya existe otro padecimiento con este mismo nombre.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(padecimiento);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Padecimiento actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PadecimientoExists(padecimiento.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(padecimiento);
        }

        // GET: Padecimiento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var padecimiento = await _context.Padecimientos.FirstOrDefaultAsync(m => m.Id == id);
            if (padecimiento == null) return NotFound();

            return View(padecimiento);
        }

        // POST: Padecimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var padecimiento = await _context.Padecimientos.FindAsync(id);

            if (padecimiento != null)
            {
                _context.Padecimientos.Remove(padecimiento);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Padecimiento eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PadecimientoExists(int id)
        {
            return _context.Padecimientos.Any(e => e.Id == id);
        }
    }
}
