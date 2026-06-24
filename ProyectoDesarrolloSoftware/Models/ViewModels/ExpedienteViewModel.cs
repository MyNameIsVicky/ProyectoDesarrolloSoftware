using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoDesarrolloSoftware.Models.Expedientes;
using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models.ViewModels
{
    public class ExpedienteViewModel
    {
        public Paciente Paciente { get; set; } = null!;
        public string NombrePaciente { get; set; } = string.Empty;
        public string CedulaPaciente { get; set; } = string.Empty;

        public List<ExpedientePadecimiento> Padecimientos { get; set; } = new();
        public List<ExpedienteTratamiento> Tratamientos { get; set; } = new();
        public List<ExpedienteMedicamento> Medicamentos { get; set; } = new();
        public List<ExamenMedico> Examenes { get; set; } = new();
        public List<HistorialClinico> HistorialClinico { get; set; } = new(); // ya viene ordenado desc desde el controller

        // Para los select a la hora de agregar un nuevo padecimiento, tratamiento o medicamento al expediente
        public IEnumerable<SelectListItem> PadecimientosCatalogo { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TratamientosCatalogo { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> MedicamentosCatalogo { get; set; } = new List<SelectListItem>();
    }
    // VM a la hora de agregar con sus respectivas validaciones
    public class AsignarPadecimientoViewModel
    {
        [Required]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "Seleccione un padecimiento")]
        public int PadecimientoId { get; set; }
    }

    public class AsignarTratamientoViewModel
    {
        [Required]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "Seleccione un tratamiento")]
        public int TratamientoId { get; set; }
    }

    public class AsignarMedicamentoViewModel
    {
        [Required]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "Seleccione un medicamento")]
        public int MedicamentoId { get; set; }
    }

    public class AgregarExamenViewModel
    {
        [Required]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe adjuntar un archivo (imagen o PDF)")]
        public IFormFile Archivo { get; set; } = null!;
    }

    public class AgregarNotaHistorialViewModel
    {
        [Required]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "La nota no puede estar vacía")]
        public string Nota { get; set; } = string.Empty;
    }
}