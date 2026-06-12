using Microsoft.EntityFrameworkCore;

namespace ProyectoDesarrolloSoftware.Models
{
    // Clase intermedia entre Medico y Especialidad para representar la relación Muchos a Muchos

    // Crear la clave compuesta manualmente, ya que no se detecta automáticamente 
    [PrimaryKey(nameof(MedicoId), nameof(EspecialidadId))]
    public class MedicoEspecialidad
    {
        public int MedicoId { get; set; }

        public Medico Medico { get; set; } = null;

        public int EspecialidadId { get; set; }

        public Especialidad Especialidad { get; set; } = null;
    }
}
