using ProyectoDesarrolloSoftware.Models.ModuloMedicina;
using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models.Expedientes
{
    public class ExpedientePadecimiento
    {
        public int Id { get; set; }

        [Required]
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;

        [Required]
        public int PadecimientoId { get; set; }
        public Padecimiento Padecimiento { get; set; } = null!;

        // El médico que realizó la asignación del padecimiento al paciente
        [Required]
        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        public DateTime FechaAsignacion { get; set; }

        public bool Suspendido { get; set; } = false; // false = activo, true = dado de alta

        public DateTime? FechaSuspension { get; set; } // Fecha en que se dio de alta
    }
}