using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ViewModel;

namespace ProyectoDesarrolloSoftware.Controllers
{
    public class MedicoController : Controller
    {

        private readonly ApplicationDbContext _context;

        public MedicoController(ApplicationDbContext context)
        {
            _context = context;
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
            return View(new MedicoVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(MedicoVM medicoVM)
        {
            ModelState.Remove("Medico.Especialidades");
            if (!ModelState.IsValid) {
                await CargarEspecialidades();
                return View(medicoVM);
            }

            medicoVM.Medico.MedicosEspecialidades = medicoVM.EspecialidadesIds.Select(id => new MedicoEspecialidad { EspecialidadId = id }).ToList();

            if (String.IsNullOrEmpty(medicoVM.Medico.FotoUrl))
            {
                medicoVM.Medico.FotoUrl = "https://cdn-icons-png.flaticon.com/512/149/149071.png";
            }

            _context.Add(medicoVM.Medico);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Médico creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var medico = await _context.Medicos
                .Include(m => m.MedicosEspecialidades)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null)
            {
                return NotFound();
            }

            var medicoVM = new MedicoVM
            {
                Medico = medico,
                EspecialidadesIds = medico.MedicosEspecialidades.Select(me => me.EspecialidadId).ToList()
            };
            await CargarEspecialidades();
            return View(medicoVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MedicoVM medicoVM) 
        {
            ModelState.Remove("Medico.Especialidades");
            if (!ModelState.IsValid)
            {
                await CargarEspecialidades();
                return View(medicoVM);
            }

            var medicoDb = await _context.Medicos
                .Include(m => m.MedicosEspecialidades)
                .FirstOrDefaultAsync(m => m.Id == medicoVM.Medico.Id);

            if (medicoDb == null)
            {
                return NotFound("No se encontraron Medicos en la base de datos");
            }

            // Actualiza las propiedades básicas automáticamente
            _context.Entry(medicoDb).CurrentValues.SetValues(medicoVM.Medico);

            // Actualiza la relación Muchos a Muchos limpiando y reinsertando
            medicoDb.MedicosEspecialidades.Clear();
            foreach (var id in medicoVM.EspecialidadesIds)
            {
                medicoDb.MedicosEspecialidades.Add(new MedicoEspecialidad { MedicoId = medicoDb.Id, EspecialidadId = id });
            }
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Médico actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }


        // Método reutilizable para cargar el catálogo de especialidades en el ViewBag
        private async Task CargarEspecialidades()
        {
            ViewBag.Especialidades = new SelectList(await _context.Especialidades.ToListAsync(), "Id", "Nombre");
        }
    }
}
