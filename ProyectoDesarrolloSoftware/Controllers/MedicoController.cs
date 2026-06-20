using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ViewModel;

namespace ProyectoDesarrolloSoftware.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MedicoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // UserManager para usar usuarios creados con identity
        public MedicoController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
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
            return View(new MedicoVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicoVM medicoVM)
        {
            
            ModelState.Remove("Medico.MedicosEspecialidades");
            ModelState.Remove("Medico.UsuarioCedula");
            // Validaciones para usuario, email, cedula, numero colegiado y contraseña
            if (await _userManager.FindByNameAsync(medicoVM.Username) != null)
            {
                ModelState.AddModelError(nameof(medicoVM.Username), "El nombre de usuario ya está en uso.");
            }

            if (await _userManager.FindByEmailAsync(medicoVM.Email) != null)
            {
                ModelState.AddModelError(nameof(medicoVM.Email), "El correo electrónico ya está registrado.");
            }

            if (medicoVM.Medico != null)
            {
                if (!string.IsNullOrEmpty(medicoVM.Medico.CedulaFisica) && _context.Medicos.Any(m => m.CedulaFisica == medicoVM.Medico.CedulaFisica))
                {
                    ModelState.AddModelError("Medico.CedulaFisica", "La cédula ya está registrada.");
                }

                if (!string.IsNullOrEmpty(medicoVM.Medico.NumeroColegiado) && _context.Medicos.Any(m => m.NumeroColegiado == medicoVM.Medico.NumeroColegiado))
                {
                    ModelState.AddModelError("Medico.NumeroColegiado", "El número de colegiado ya está en uso.");
                }
            }

            if (string.IsNullOrEmpty(medicoVM.Password) || medicoVM.Password.Length < 6)
            {
                ModelState.AddModelError(nameof(medicoVM.Password), "La contraseña debe tener al menos 6 caracteres.");
            }

            // Verificar que se haya seleccionado al menos una especialidad
            if (medicoVM.EspecialidadesIds == null || !medicoVM.EspecialidadesIds.Any())
            {
                ModelState.AddModelError(nameof(medicoVM.EspecialidadesIds), "Seleccione al menos una especialidad.");
            }

            if (!ModelState.IsValid)
            {
                await CargarEspecialidades();
                return View(medicoVM);
            }

            var nuevoUsuario = new IdentityUser
            {
                UserName = medicoVM.Username,
                Email = medicoVM.Email,
                EmailConfirmed = true 
            };

            var result = await _userManager.CreateAsync(nuevoUsuario, medicoVM.Password);

            
            if (result.Succeeded)
            {
                // Se asigna el Rol si todo sale bien
                await _userManager.AddToRoleAsync(nuevoUsuario, Perfil.Medico.ToString());

                medicoVM.Medico.UsuarioCedula = nuevoUsuario.Id;

                medicoVM.Medico.MedicosEspecialidades = medicoVM.EspecialidadesIds
                    .Select(id => new MedicoEspecialidad { EspecialidadId = id }).ToList();

                if (String.IsNullOrEmpty(medicoVM.Medico.FotoUrl))
                {
                    medicoVM.Medico.FotoUrl = "https://cdn-icons-png.flaticon.com/512/149/149071.png";
                }

                _context.Add(medicoVM.Medico);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Médico y credenciales creados exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            // Se mapean los errores de Identity a ModelState para mostrarlos en la vista en español
            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicateUserName")
                {
                    ModelState.AddModelError(nameof(medicoVM.Username), "El nombre de usuario ya está en uso.");
                }
                else if (error.Code == "DuplicateEmail")
                {
                    ModelState.AddModelError(nameof(medicoVM.Email), "El correo electrónico ya está registrado.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            await CargarEspecialidades();
            return View(medicoVM);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicoVM medicoVM)
        {
            ModelState.Remove("Medico.MedicosEspecialidades");
            ModelState.Remove("Username");
            ModelState.Remove("Email");
            ModelState.Remove("Password");
            ModelState.Remove("Medico.UsuarioCedula");

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

            medicoDb.NombreCompleto = medicoVM.Medico.NombreCompleto;
            medicoDb.NumeroColegiado = medicoVM.Medico.NumeroColegiado;
            medicoDb.CedulaFisica = medicoVM.Medico.CedulaFisica;
            medicoDb.FotoUrl = medicoVM.Medico.FotoUrl;

            medicoDb.MedicosEspecialidades.Clear();

            if (medicoVM.EspecialidadesIds != null)
            {
                foreach (var id in medicoVM.EspecialidadesIds)
                {
                    medicoDb.MedicosEspecialidades.Add(new MedicoEspecialidad
                    {
                        MedicoId = medicoDb.Id,
                        EspecialidadId = id
                    });
                }
            }
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Médico actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Medico/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var medico = await _context.Medicos
                .Include(m => m.MedicosEspecialidades)
                .ThenInclude(me => me.Especialidad)
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

            // Se guarda el userId antes de eliminar el medico, ya que se necesita para eliminar el usuario de Identity después
            var userId = medico.UsuarioCedula;

            // Se borra el medico primero para evitar problemas de FK
            _context.Medicos.Remove(medico);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var deleteResult = await _userManager.DeleteAsync(user);
                    if (!deleteResult.Succeeded)
                    {
                        // Mensaje de error por si no se pudo eliminar el usuario de Identity
                        TempData["ErrorMessage"] = "El médico fue eliminado, pero no se pudo eliminar el usuario asociado.";
                    }
                }
            }

            TempData["SuccessMessage"] = "Médico y su usuario asociado eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarEspecialidades()
        {
            ViewBag.Especialidades = new SelectList(await _context.Especialidades.ToListAsync(), "Id", "Nombre");
        }
    }
}