using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models
{
    public class Tratamiento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        public string Descripcion { get; set; }
    }
}
