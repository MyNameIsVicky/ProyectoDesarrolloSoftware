using System.ComponentModel.DataAnnotations;


using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ModuloMedicina;
 
namespace ProyectoDesarrolloSoftware.Models.Expedientes
{
    public class ExamenLaboratorio
    {
        public int Id { get; set; }

        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;

        // El médico que registró el examen
        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        [Required]
        public string Nombre { get; set; } = string.Empty;       

        public string Descripcion { get; set; } = string.Empty;    

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        public string RutaArchivo { get; set; } = string.Empty;    // Ruta física del PDF en el servidor

        public string NombreArchivo { get; set; } = string.Empty;  
    }
}
