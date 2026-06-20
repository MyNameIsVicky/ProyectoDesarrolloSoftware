using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ViewModels;

namespace ProyectoDesarrolloSoftware.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UsuarioController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var vms = new List<UsuarioViewModel>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var bloqueado = await _userManager.IsLockedOutAsync(u);

                // Obtener datos del paciente vinculado si existe
                var paciente = _context.Pacientes.FirstOrDefault(p => p.UsuarioId == u.Id);

                vms.Add(new UsuarioViewModel
                {
                    Id = u.Id,
                    Correo = u.Email ?? "",
                    NombreCompleto = paciente?.NombreCompleto ?? u.UserName ?? "",
                    Cedula = paciente?.Cedula ?? "",
                    Perfil = roles.FirstOrDefault() ?? "Paciente",
                    Bloqueado = bloqueado
                });
            }

            return View(vms);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildVM(new UsuarioViewModel());
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await BuildVM(vm);
                return View(vm);
            }

            var user = new IdentityUser
            {
                UserName = vm.Correo,
                Email = vm.Correo,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, vm.Password!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                vm = await BuildVM(vm);
                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, vm.Perfil);

            // Si el perfil es Paciente, crear registro en la tabla Pacientes
            if (vm.Perfil == "Paciente")
            {
                _context.Pacientes.Add(new Paciente
                {
                    Cedula = vm.Cedula,
                    NombreCompleto = vm.NombreCompleto,
                    Correo = vm.Correo,
                    UsuarioId = user.Id
                });
                await _context.SaveChangesAsync();
            }

            // Si el perfil es Médico, vincular el usuario al médico seleccionado
            if (vm.Perfil == "Medico" && vm.MedicoId.HasValue)
            {
                var medico = await _context.Medicos.FindAsync(vm.MedicoId.Value);
                if (medico != null)
                {
                    medico.UsuarioCedula = user.Id;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var paciente =  _context.Pacientes.FirstOrDefault(p => p.UsuarioId == id);

            var vm = new UsuarioViewModel
            {
                Id = user.Id,
                Correo = user.Email ?? "",
                NombreCompleto = paciente?.NombreCompleto ?? "",
                Cedula = paciente?.Cedula ?? "",
                Perfil = roles.FirstOrDefault() ?? "Paciente"
            };

            vm = await BuildVM(vm);
            return View(vm);
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<IActionResult> EditConfirm(UsuarioViewModel vm)
        {
            var user = await _userManager.FindByIdAsync(vm.Id!);
            if (user == null) return NotFound();

            user.Email = vm.Correo;
            user.UserName = vm.Correo;
            await _userManager.UpdateAsync(user);

            // Actualizar paciente si aplica
            var paciente = _context.Pacientes.FirstOrDefault(p => p.UsuarioId == vm.Id);
            if (paciente != null)
            {
                paciente.NombreCompleto = vm.NombreCompleto;
                paciente.Cedula = vm.Cedula;
                paciente.Correo = vm.Correo;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBloqueo(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var esBloqueado = await _userManager.IsLockedOutAsync(user);

            if (esBloqueado)
            {
                // Desbloquear: quitar el lockout
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            else
            {
                // Bloquear: lockout por 100 años (indefinido)
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            var vm = new UsuarioViewModel { Id = user.Id, Correo = user.Email ?? "", Perfil = roles.FirstOrDefault() ?? "" };
            return View(vm);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }


        private async Task<UsuarioViewModel> BuildVM(UsuarioViewModel vm)
        {
            vm.MedicosList = _context.Medicos.OrderBy(m => m.NombreCompleto).Select(m => new SelectListItem 
            { Text = m.NombreCompleto, Value = m.Id.ToString() }).ToList();
            return vm;
        }
    }
}
