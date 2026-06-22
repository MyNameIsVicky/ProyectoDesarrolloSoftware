using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDesarrolloSoftware.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Datos personales — fuente única de verdad para todos los perfiles.
        // Médico y Paciente leen NombreCompleto y Cedula desde aquí.
        [Required]
        [MaxLength(200)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        public Perfil Perfil { get; set; }

        // Solo aplica cuando Perfil == Medico
        public int? MedicoId { get; set; }

        [ForeignKey(nameof(MedicoId))]
        public Medico? Medico { get; set; }

    }
}