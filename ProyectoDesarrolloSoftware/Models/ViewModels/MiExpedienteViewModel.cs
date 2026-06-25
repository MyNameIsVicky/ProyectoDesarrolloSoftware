using ProyectoDesarrolloSoftware.Models.Expedientes;

namespace ProyectoDesarrolloSoftware.Models.ViewModels
{
    public class MiExpedienteViewModel
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public List<ExpedientePadecimiento> Padecimientos { get; set; } = new();
        public List<ExpedienteTratamiento> Tratamientos { get; set; } = new();
        public List<ExpedienteMedicamento> Medicamentos { get; set; } = new();
        public List<ExamenMedico> Examenes { get; set; } = new();
        public List<HistorialClinico> HistorialClinico { get; set; } = new();

    }
}
