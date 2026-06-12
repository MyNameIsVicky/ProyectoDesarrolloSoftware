using ProyectoDesarrolloSoftware.Models.Expedientes;
using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models
{
    public class Paciente
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        // FK al usuario de Identity 
        public string? UsuarioId { get; set; }

        // Relaciones con expedientes
        public ICollection<ExpedientePadecimiento> Padecimientos { get; set; } = new List<ExpedientePadecimiento>();
        public ICollection<ExpedienteTratamiento> Tratamientos { get; set; } = new List<ExpedienteTratamiento>();
        public ICollection<ExpedienteMedicamento> Medicamentos { get; set; } = new List<ExpedienteMedicamento>();
             public ICollection<HistorialClinico> HistorialClinico { get; set; } = new List<HistorialClinico>();
    }
}
