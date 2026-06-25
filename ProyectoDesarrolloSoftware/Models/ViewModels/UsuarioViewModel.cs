using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoDesarrolloSoftware.Models;
using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Correo { get; set; } = string.Empty;

        [ValidateNever]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "La cédula debe contener exactamente 9 dígitos")]
        public string Cedula { get; set; } = string.Empty;
        public string? Password { get; set; }

        public Perfil Perfil { get; set; } = Perfil.Paciente;

        public bool Bloqueado { get; set; } = false;

        public int? MedicoId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> MedicosList { get; set; } = new List<SelectListItem>();
    }
}