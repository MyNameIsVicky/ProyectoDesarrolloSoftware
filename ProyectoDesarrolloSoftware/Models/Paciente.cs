using ProyectoDesarrolloSoftware.Models.Expedientes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDesarrolloSoftware.Models
{
    public class Paciente
    {
        public int Id { get; set; }

        // FK obligatoria — un paciente siempre tiene cuenta de usuario
        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        [ForeignKey(nameof(UsuarioId))]
        public ApplicationUser? Usuario { get; set; }

        // Relaciones de expediente
        public ICollection<ExpedientePadecimiento> Padecimientos { get; set; } = new List<ExpedientePadecimiento>();
        public ICollection<ExpedienteTratamiento> Tratamientos { get; set; } = new List<ExpedienteTratamiento>();
        public ICollection<ExpedienteMedicamento> Medicamentos { get; set; } = new List<ExpedienteMedicamento>();
        public ICollection<HistorialClinico> HistorialClinico { get; set; } = new List<HistorialClinico>();

    }
}
