using ProyectoDesarrolloSoftware.Models.ModuloMedicina;
using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models.Expedientes
{
    public class ExpedientePadecimiento
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int PacienteId { get; set; }

        [Required]
        public int PadecimientoId { get; set; }

        public Padecimiento Padecimiento { get; set; } = null;

        // El médico que realizó la asignación del padecimiento al paciente
        [Required]
        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null;

        public DateTime FechaAsignacion { get; set; }
        
        public bool Activo { get; set; } = false; // Indica si el padecimiento está activo o ha sido dado de alta

        public DateTime? fechaSuspension { get; set; } // Fecha en la que se dio de alta el padecimiento, si es que se dio de alta

    }
}
