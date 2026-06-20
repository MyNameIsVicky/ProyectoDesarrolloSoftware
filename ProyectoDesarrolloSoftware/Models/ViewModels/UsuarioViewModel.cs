using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoDesarrolloSoftware.Models.ViewModels
{
    // ViewModel para crear un usuario con perfil Médico o Paciente

    public class UsuarioViewModel
    {
        public string? Id { get; set; }

        [ValidateNever]
        public string Cedula { get; set; } = string.Empty;

        [ValidateNever]
        public string NombreCompleto { get; set; } = string.Empty;

        [ValidateNever]
        public string Correo { get; set; } = string.Empty;

        public string? Password { get; set; }

        // "Admin" o "Medico" o "Paciente"
        public string Perfil { get; set; } = "Paciente";

        // Solo requerido cuando Perfil == "Medico", el signo de interrogación indica que es opcional
        public int? MedicoId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> MedicosList { get; set; } = new List<SelectListItem>();

        public bool Bloqueado { get; set; } = false;
    }
}
