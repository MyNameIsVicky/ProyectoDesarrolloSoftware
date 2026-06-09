namespace ProyectoDesarrolloSoftware.Models
{
    // Clase intermedia entre Medico y Especialidad para representar la relación Muchos a Muchos
    public class MedicoEspecialdad
    {
        public int MedicoId { get; set; }

        public Medico Medico { get; set; } = null;

        public int EspecialidadId { get; set; }

        public Especialidad Especialidad { get; set; } = null;

    }
}
