using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoDesarrolloSoftware.Models.Expedientes;

namespace ProyectoDesarrolloSoftware.Models.ViewModels
{
    // ViewModel para el expediente completo de un paciente.

    public class ExpedienteViewModel
    {
        // ValidateNever es para evitar validaciones innecesarias en las propiedades que no se envían desde el formulario, sino que se cargan desde la base de datos
        [ValidateNever]
        public Paciente Paciente { get; set; } = null!;

        [ValidateNever]
        public IEnumerable<ExpedientePadecimiento> Padecimientos { get; set; } = new List<ExpedientePadecimiento>();

        [ValidateNever]
        public IEnumerable<ExpedienteTratamiento> Tratamientos { get; set; } = new List<ExpedienteTratamiento>();

        [ValidateNever]
        public IEnumerable<ExpedienteMedicamento> Medicamentos { get; set; } = new List<ExpedienteMedicamento>();

        [ValidateNever]
        public IEnumerable<ExamenMedico> Examenes { get; set; } = new List<ExamenMedico>();


        // Historial ordenado del más reciente al menos reciente
        [ValidateNever]
        public IEnumerable<HistorialClinico> Historial { get; set; } = new List<HistorialClinico>();

        // Listas para los selects de asignación
        [ValidateNever]
        public IEnumerable<SelectListItem> PadecimientosList { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> TratamientosList { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> MedicamentosList { get; set; } = new List<SelectListItem>();

        // Para la nueva nota de historial
        public string? NuevaNotaHtml { get; set; }

        // Para la nueva asignación de examen
        [ValidateNever]
        public IFormFile? ArchivoExamen { get; set; }
        public string? DescripcionExamen { get; set; }
    }
}
