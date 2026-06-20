
namespace ProyectoDesarrolloSoftware.Models.ViewModel
{
    public class MedicoVM
    {
        public Medico Medico { get; set; } = new Medico();

        // Aqui se guardaran los IDs de las especialidades seleccionadas en la vista
        public List<int> EspecialidadesIds { get; set; } = new List<int>();
    }
}