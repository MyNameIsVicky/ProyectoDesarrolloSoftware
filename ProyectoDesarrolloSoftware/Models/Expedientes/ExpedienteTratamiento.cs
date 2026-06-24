using ProyectoDesarrolloSoftware.Models.ModuloMedicina;

namespace ProyectoDesarrolloSoftware.Models.Expedientes
{
    public class ExpedienteTratamiento
    {
        public int Id { get; set; }

        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;

        public int TratamientoId { get; set; }
        public Tratamiento Tratamiento { get; set; } = null!;

        // El médico que realizó la asignación 
        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        public DateTime FechaAsignacion { get; set; }
        public bool Suspendido { get; set; } = false;  // false = activo, true = dado de alta
        public DateTime? FechaSuspension { get; set; } // Fecha en la que se dio de alta

    }
}
