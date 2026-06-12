using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models.ModuloMedicina
{
    public class Medicamento
    {
       
            [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(150)]
            public string NombreMedicamento { get; set; }
        
    }
}
