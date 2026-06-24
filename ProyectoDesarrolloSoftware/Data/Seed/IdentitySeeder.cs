using Microsoft.AspNetCore.Identity;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ModuloMedicina;

namespace ProyectoDesarrolloSoftware.Data.Seed
{
    public class IdentitySeeder
    {
        public static async Task seedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            // Roles 
            foreach (var role in Enum.GetNames(typeof(Perfil)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Administrador 
            if (await userManager.FindByNameAsync("admin") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@proyecto.com",
                    NombreCompleto = "Administrador Principal",
                    Cedula = "123654897",
                    EmailConfirmed = true,
                    Perfil = Perfil.Administrador
                };

                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, Perfil.Administrador.ToString());
            }

            // Especialidades 
            if (!context.Especialidades.Any())
            {
                context.Especialidades.AddRange(
                    new Especialidad { Nombre = "Medicina General" },
                    new Especialidad { Nombre = "Cardiología" },
                    new Especialidad { Nombre = "Pediatría" },
                    new Especialidad { Nombre = "Dermatología" },
                    new Especialidad { Nombre = "Neurología" }
                );
                await context.SaveChangesAsync();
            }

            // Médico tratante 
            if (!context.Medicos.Any(m => m.CedulaFisica == "119410090") &&
                await userManager.FindByNameAsync("mvfallas") == null)
            {
                var medico = new Medico
                {
                    NombreCompleto = "María Victoria Fallas",
                    CedulaFisica = "119410090",
                    NumeroColegiado = "MED-015",
                    FotoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQDHL8Bcf3t0PW_T_k-dgU7JmMEmms2XbiEpq73BtGfYA&s=10",
                    MedicosEspecialidades = new List<MedicoEspecialidad>
                    {
                        new MedicoEspecialidad { EspecialidadId = 1 },
                        new MedicoEspecialidad { EspecialidadId = 2 }
                    }
                };

                context.Medicos.Add(medico);
                await context.SaveChangesAsync();

                var usuarioMedico = new ApplicationUser
                {
                    UserName = "mvfallas",
                    Email = "medico@medicalsystem.com",
                    NombreCompleto = medico.NombreCompleto,
                    Cedula = medico.CedulaFisica,
                    EmailConfirmed = true,
                    Perfil = Perfil.Medico,
                    MedicoId = medico.Id
                };

                await userManager.CreateAsync(usuarioMedico, "Medico123!");
                await userManager.AddToRoleAsync(usuarioMedico, Perfil.Medico.ToString());
            }

            // Paciente 
            // Datos personales van en ApplicationUser; Paciente solo agrupa el expediente
            if (await userManager.FindByNameAsync("jpaciente") == null)
            {
                var usuarioPaciente = new ApplicationUser
                {
                    UserName = "jpaciente",
                    Email = "paciente@proyecto.com",
                    NombreCompleto = "John Paciente",
                    Cedula = "123456789",
                    EmailConfirmed = true,
                    Perfil = Perfil.Paciente
                };

                await userManager.CreateAsync(usuarioPaciente, "Paciente123!");
                await userManager.AddToRoleAsync(usuarioPaciente, Perfil.Paciente.ToString());

                context.Pacientes.Add(new Paciente { UsuarioId = usuarioPaciente.Id });
                await context.SaveChangesAsync();
            }

            // Padecimientos 
            if (!context.Padecimientos.Any())
            {
                context.Padecimientos.AddRange(
                    new Padecimiento { Nombre = "Hipertensión", Descripcion = "Presión arterial elevada de forma crónica." },
                    new Padecimiento { Nombre = "Diabetes Tipo 2", Descripcion = "Trastorno metabólico con niveles altos de glucosa." },
                    new Padecimiento { Nombre = "Asma", Descripcion = "Enfermedad inflamatoria crónica de las vías respiratorias." }
                );
                await context.SaveChangesAsync();
            }

            // Tratamientos 
            if (!context.Tratamientos.Any())
            {
                context.Tratamientos.AddRange(
                    new Tratamiento { Nombre = "Terapia Física", Descripcion = "Rehabilitación mediante ejercicios dirigidos." },
                    new Tratamiento { Nombre = "Dieta Balanceada", Descripcion = "Plan nutricional personalizado." },
                    new Tratamiento { Nombre = "Control de Glucosa", Descripcion = "Monitoreo diario de niveles de azúcar." }
                );
                await context.SaveChangesAsync();
            }

            // Medicamentos 
            if (!context.Medicamentos.Any())
            {
                context.Medicamentos.AddRange(
                    new Medicamento { NombreMedicamento = "Azatioprina" },
                    new Medicamento { NombreMedicamento = "Ibersartan" },
                    new Medicamento { NombreMedicamento = "Atenolol" },
                    new Medicamento { NombreMedicamento = "Lovastatina" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}