using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models.ViewModels;
using ProyectoDesarrolloSoftware.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ProyectoDesarrolloSoftware.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UsuarioController( UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var vms = new List<UsuarioViewModel>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var bloqueado = await _userManager.IsLockedOutAsync(u);

                vms.Add(new UsuarioViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName ?? "",
                    Correo = u.Email ?? "",
                    NombreCompleto = u.NombreCompleto,
                    Cedula = u.Cedula,
                    Perfil = u.Perfil,
                    Bloqueado = bloqueado,
                    MedicoId = u.MedicoId
                });
            }

            return View(vms);
        }

          [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(await BuildVM(new UsuarioViewModel()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioViewModel vm)
        {
            // Validaciones manuales para campos que dependen del perfil
            if (string.IsNullOrWhiteSpace(vm.Password))
                ModelState.AddModelError(nameof(vm.Password), "La contraseña es obligatoria.");

            if (vm.Perfil != Perfil.Medico)
            {
                if (string.IsNullOrWhiteSpace(vm.NombreCompleto)) { 
                ModelState.AddModelError(nameof(vm.NombreCompleto), "El nombre completo es obligatorio.");
            }
                if (string.IsNullOrWhiteSpace(vm.Cedula))
                {
                    ModelState.AddModelError(nameof(vm.Cedula), "La cédula es obligatoria.");
                }
            }

            if (!ModelState.IsValid)
                return View(await BuildVM(vm));

            // Para médico, NombreCompleto y Cedula vienen de la entidad Medico
            if (vm.Perfil == Perfil.Medico)
            {
                if (!vm.MedicoId.HasValue)
                {
                    ModelState.AddModelError(nameof(vm.MedicoId), "Debe seleccionar un médico tratante.");
                    return View(await BuildVM(vm));
                }

                var medicoData = await _context.Medicos.FindAsync(vm.MedicoId.Value);
                if (medicoData == null)
                {
                    ModelState.AddModelError(nameof(vm.MedicoId), "El médico seleccionado no existe.");
                    return View(await BuildVM(vm));
                }

                vm.NombreCompleto = medicoData.NombreCompleto;
                vm.Cedula = medicoData.CedulaFisica;
            }

            var user = new ApplicationUser
            {
                UserName = vm.UserName,
                Email = vm.Correo,
                NombreCompleto = vm.NombreCompleto,
                Cedula = vm.Cedula,
                EmailConfirmed = true,
                Perfil = vm.Perfil
            };

            var result = await _userManager.CreateAsync(user, vm.Password!);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);

                return View(await BuildVM(vm));
            }

            await _userManager.AddToRoleAsync(user, vm.Perfil.ToString());

            if (vm.Perfil == Perfil.Paciente)
            {
                _context.Pacientes.Add(new Paciente { UsuarioId = user.Id });
                await _context.SaveChangesAsync();
            }
            else if (vm.Perfil == Perfil.Medico)
            {
                user.MedicoId = vm.MedicoId!.Value;
                await _userManager.UpdateAsync(user);
            }

            TempData["SuccessMessage"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _userManager.GetRolesAsync(user);

            var vm = new UsuarioViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Correo = user.Email ?? "",
                NombreCompleto = user.NombreCompleto,
                Cedula = user.Cedula,
                Perfil = user.Perfil,
                MedicoId = user.MedicoId
            };

            return View(await BuildVM(vm));
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<IActionResult> EditConfirm(UsuarioViewModel vm)
        {
            var user = await _userManager.FindByIdAsync(vm.Id!);
            if (user == null)
            {
                return NotFound();
            }
            user.UserName = vm.UserName;
            user.Email = vm.Correo;
            user.NombreCompleto = vm.NombreCompleto;
            user.Cedula = vm.Cedula;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(await BuildVM(vm));
            }
            // Contraseña 
            if (!string.IsNullOrWhiteSpace(vm.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, vm.Password);
            }

            TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBloqueo(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (await _userManager.IsLockedOutAsync(user))
                await _userManager.SetLockoutEndDateAsync(user, null);
            else
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return View(new UsuarioViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                NombreCompleto = user.NombreCompleto,
                Correo = user.Email ?? "",
                Perfil = user.Perfil,
            });
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            // Si es paciente, eliminar también su registro de expediente
            if (user.Perfil == Perfil.Paciente)
            {
                var paciente = await _context.Pacientes
                    .FirstOrDefaultAsync(p => p.UsuarioId == id);

                if (paciente != null)
                {
                    _context.Pacientes.Remove(paciente);
                }
                await _context.SaveChangesAsync();
            }

            await _userManager.DeleteAsync(user);
            TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        private async Task<UsuarioViewModel> BuildVM(UsuarioViewModel vm)
        {
            vm.MedicosList = await _context.Medicos
                .Where(m => !_context.Users.Any(u => u.MedicoId == m.Id) || m.Id == vm.MedicoId)
                .OrderBy(m => m.NombreCompleto)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.NombreCompleto
                })
                .ToListAsync();

            return vm;
        }
    }
}