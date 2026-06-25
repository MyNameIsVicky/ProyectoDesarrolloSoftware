using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDesarrolloSoftware.Models
{
    public class Medico
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150)]  
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El número de colegiado es obligatorio")]
        [StringLength(50)]
        public string NumeroColegiado { get; set; }

        public string FotoUrl { get; set; } // Guarda la ruta de la imagen 


        [Required(ErrorMessage = "La cédula es obligatoria")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "La cédula debe contener exactamente 9 dígitos")]
        public string CedulaFisica { get; set; } = string.Empty;

        public virtual IList<MedicoEspecialidad> MedicosEspecialidades { get; set; } = new List<MedicoEspecialidad>();
    }
}

