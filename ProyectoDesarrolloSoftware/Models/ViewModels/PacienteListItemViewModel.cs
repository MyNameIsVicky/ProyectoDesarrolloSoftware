namespace ProyectoDesarrolloSoftware.Models.ViewModels
{
    public class PacienteListItemViewModel
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public DateTime? UltimaAtencion { get; set; }
    }
}
