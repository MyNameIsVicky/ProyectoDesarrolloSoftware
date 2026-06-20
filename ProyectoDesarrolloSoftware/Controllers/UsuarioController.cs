using ProyectoDesarrolloSoftware.Data;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ViewModels;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsuarioController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
                var perfil = roles.FirstOrDefault() ?? "Sin perfil";

                string nombre = u.Email ?? u.UserName ?? "";
                string cedula = "—";

                if (perfil == "Paciente")
                {
                    var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == u.Id);
                    nombre = paciente?.NombreCompleto ?? nombre;
                    cedula = paciente?.Cedula ?? "—";
                }
                else if (perfil == "Medico")
                {
                    var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.UsuarioCedula == u.Id);
                    nombre = medico?.NombreCompleto ?? nombre;
                    cedula = medico?.CedulaFisica ?? "—";
                }

                vms.Add(new UsuarioViewModel
                {
                    Id = u.Id,
                    Correo = u.Email ?? "",
                    NombreCompleto = nombre,
                    Cedula = cedula,
                    Perfil = perfil,
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
            if (!ModelState.IsValid) { vm = await BuildVM(vm); return View(vm); }

            var user = new IdentityUser { UserName = vm.Correo, Email = vm.Correo, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, vm.Password!);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                vm = await BuildVM(vm);
                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, vm.Perfil);

            if (vm.Perfil == "Paciente")
            {
                _context.Pacientes.Add(new Paciente { Cedula = vm.Cedula, NombreCompleto = vm.NombreCompleto, Correo = vm.Correo, UsuarioId = user.Id });
                await _context.SaveChangesAsync();
            }
            else if (vm.Perfil == "Medico" && vm.MedicoId.HasValue)
            {
                var medico = await _context.Medicos.FindAsync(vm.MedicoId.Value);
                if (medico != null) { medico.UsuarioCedula = user.Id; await _context.SaveChangesAsync(); }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var perfil = roles.FirstOrDefault() ?? "Sin perfil";

            string nombre = "";
            string cedula = "";

            if (perfil == "Paciente")
            {
                var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == id);
                nombre = paciente?.NombreCompleto ?? "";
                cedula = paciente?.Cedula ?? "";
            }
            else if (perfil == "Medico")
            {
                var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.UsuarioCedula == id);
                nombre = medico?.NombreCompleto ?? "";
                cedula = medico?.NumeroColegiado ?? "";
            }
          

            var vm = new UsuarioViewModel
            {
                Id = user.Id,
                Correo = user.Email ?? "",
                NombreCompleto = nombre,
                Cedula = cedula,
                Perfil = perfil
            };

            return View(await BuildVM(vm));
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

            // Actualizar rol si cambió
            var rolesActuales = await _userManager.GetRolesAsync(user);
            if (!rolesActuales.Contains(vm.Perfil))
            {
                await _userManager.RemoveFromRolesAsync(user, rolesActuales);
                await _userManager.AddToRoleAsync(user, vm.Perfil);
            }

            if (vm.Perfil == "Paciente")
            {
                var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.UsuarioId == vm.Id);
                if (paciente != null)
                {
                    paciente.NombreCompleto = vm.NombreCompleto;
                    paciente.Cedula = vm.Cedula;
                    paciente.Correo = vm.Correo;
                    await _context.SaveChangesAsync();
                }
            }
            else if (vm.Perfil == "Medico")
            {
                var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.UsuarioCedula == vm.Id);
                if (medico != null)
                {
                    medico.NombreCompleto = vm.NombreCompleto;
                    medico.NumeroColegiado = vm.Cedula;
                    await _context.SaveChangesAsync();
                }
            }
            // Admin no tiene tabla propia, solo se actualiza correo arriba

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
            return View(new UsuarioViewModel { Id = user.Id, Correo = user.Email ?? "", Perfil = roles.FirstOrDefault() ?? "" });
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
            vm.MedicosList = await _context.Medicos
                .OrderBy(m => m.NombreCompleto)
                .Select(m => new SelectListItem { Text = m.NombreCompleto, Value = m.Id.ToString() })
                .ToListAsync();
            return vm;
        }
    }
}
