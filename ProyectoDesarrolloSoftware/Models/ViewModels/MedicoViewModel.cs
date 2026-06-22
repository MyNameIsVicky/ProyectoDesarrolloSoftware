using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models.ViewModels
{    
    // ViewModel para crear/editar un Médico con selección múltiple de especialidades
        public class MedicoViewModel
    {
        [ValidateNever]
        public Medico Medico { get; set; } = new();

        // IDs de especialidades seleccionadas en el form
        public List<int> EspecialidadesIds { get; set; } = new();

        [ValidateNever]
        public IEnumerable<SelectListItem> EspecialidadesList { get; set; } = new List<SelectListItem>();

    }
}
