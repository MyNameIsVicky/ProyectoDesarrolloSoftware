using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models.Expedientes
{
    public class ExamenMedico
    {
        public int Id { get; set; }

        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;

        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        // Ruta del archivo a guardar 
        [Required]
        [MaxLength(500)]
        public string ArchivoRuta { get; set; } = string.Empty;

        [MaxLength(10)]
        public string TipoArchivo { get; set; } = string.Empty; // pdf o imagen

        public DateTime FechaSubida { get; set; }
    }
}
