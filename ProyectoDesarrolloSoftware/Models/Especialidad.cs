using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models
{
    public class Especialidad
    {
      
            [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(100)]
            public string Nombre { get; set; }

            // Relación Muchos a Muchos con Médicos
            public virtual IList<Medico> Medicos { get; set; } = new List<Medico>();
        
    }
}
