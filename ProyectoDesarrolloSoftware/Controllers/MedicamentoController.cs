using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models.ModuloMedicina;

namespace ProyectoDesarrolloSoftware.Controllers
{
    [Authorize(Roles = "Administrador, Medico")]
    public class MedicamentoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicamentoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Medicamento
        public async Task<IActionResult> Index()
        {
            var medicamentos = await _context.Medicamentos.ToListAsync();
            return View(medicamentos);
        }

        // GET: Medicamento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medicamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombreMedicamento")] Medicamento medicamento)
        {
            // Validación para ver si ya hay un medicamento con ese nombre
            if (await _context.Medicamentos.AnyAsync(m => m.NombreMedicamento == medicamento.NombreMedicamento))
            {
                ModelState.AddModelError("NombreMedicamento", "Ya existe un medicamento registrado con este nombre.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(medicamento);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medicamento creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(medicamento);
        }

        // GET: Medicamento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null) return NotFound();

            return View(medicamento);
        }


        // POST: Medicamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreMedicamento")] Medicamento medicamento)
        {
            if (id != medicamento.Id) return NotFound();

            // Validación para ver si ya hay otro medicamento con ese mismo nombre (excluyendo el actual)
            if (await _context.Medicamentos.AnyAsync(m => m.NombreMedicamento == medicamento.NombreMedicamento && m.Id != id))
            {
                ModelState.AddModelError("NombreMedicamento", "Ya existe otro medicamento con este mismo nombre.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicamento);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Medicamento actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicamentoExists(medicamento.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(medicamento);
        }

        // GET: Medicamento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var medicamento = await _context.Medicamentos.FirstOrDefaultAsync(m => m.Id == id);
            if (medicamento == null) return NotFound();

            return View(medicamento);
        }

        // POST: Medicamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicamento = await _context.Medicamentos.FindAsync(id);

            if (medicamento != null)
            {
                _context.Medicamentos.Remove(medicamento);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medicamento eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MedicamentoExists(int id)
        {
            return _context.Medicamentos.Any(e => e.Id == id);
        }
    }
}
