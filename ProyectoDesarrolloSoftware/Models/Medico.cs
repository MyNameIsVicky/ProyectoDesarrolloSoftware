using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDesarrolloSoftware.Models
{
    public class Medico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreCompleto { get; set; }

        [Required]
        [StringLength(50)]
        public string NumeroColegiado { get; set; }

        public string FotoUrl { get; set; } // Guarda la ruta de la imagen 

       
        [Required]
        public string UsuarioCedula { get; set; }
        [ForeignKey("UsuarioCedula")]
        // public virtual Usuario Usuario { get; set; }

      
        public virtual IList<Especialidad> Especialidades { get; set; } = new List<Especialidad>();
    }
}
