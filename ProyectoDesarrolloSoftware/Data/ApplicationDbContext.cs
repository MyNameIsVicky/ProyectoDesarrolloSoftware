using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ModuloMedicina;
using ProyectoDesarrolloSoftware.Models.Expedientes;

namespace ProyectoDesarrolloSoftware.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }


        // Catálogo de administrativos
        public DbSet<Especialidad> Especialidades { get; set; }

        public DbSet<Medico> Medicos { get; set; }

        public DbSet<MedicoEspecialidad> MedicoEspecialidades { get; set; }

        public DbSet<Paciente> Pacientes { get; set; }

        // Clinicos

        public DbSet<Padecimiento> Padecimientos { get; set; }

        public DbSet<Tratamiento> Tratamientos { get; set; }

        public DbSet<Medicamento> Medicamentos { get; set; }

        // Expedientes

        public DbSet<ExpedientePadecimiento> ExpedientePadecimientos { get; set; }

        public DbSet<ExpedienteTratamiento> ExpedienteTratamientos { get; set; }

        public DbSet<ExpedienteMedicamento> ExpedienteMedicamentos { get; set; }

        public DbSet<HistorialClinico> HistorialClinicos{ get; set; }

        public DbSet<ExamenLaboratorio> ExamenesLaboratorio { get; set; }

    }
}
