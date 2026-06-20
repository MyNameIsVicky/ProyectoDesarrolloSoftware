using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models.ViewModels
{    
    // ViewModel para crear/editar un Médico con selección múltiple de especialidades
        public class MedicoViewModel
    {
         //Campos para la creación de usuario y autenticación
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo no válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public Medico Medico { get; set; } = new Medico();

        // Aquí se guardarán los IDs de las especialidades seleccionadas
        [MinLength(1, ErrorMessage = "Seleccione al menos una especialidad.")]
        public List<int> EspecialidadesIds { get; set; } = new List<int>();
    }
}
