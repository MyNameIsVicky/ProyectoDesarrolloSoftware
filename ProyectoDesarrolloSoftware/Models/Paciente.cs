using System.ComponentModel.DataAnnotations;

namespace ProyectoDesarrolloSoftware.Models
{
    //comitt prueba isaac
    public class Paciente
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string lastName{ get; set; }
        [Required]
        public String nationalId { get; set; }
        [Required]
        public int age { get; set; }
        [Required]
        public string sex { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        private string password { get; set; }

     
    }
}
