using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models.Expedientes
{
    public class HistorialClinico
    {
        public int Id { get; set; }

        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;

        // El médico que realizó la asignación 
                public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        [Required]
        public string Nota { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }

    }
}
