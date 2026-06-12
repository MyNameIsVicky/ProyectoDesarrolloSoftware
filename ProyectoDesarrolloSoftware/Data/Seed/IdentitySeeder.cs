using Microsoft.AspNetCore.Identity;
using ProyectoDesarrolloSoftware.Models;
using ProyectoDesarrolloSoftware.Models.ModuloMedicina;

namespace ProyectoDesarrolloSoftware.Data.Seed
{
    public class IdentitySeeder
    {

        public static async Task seedAsync(ServiceProvider sevices)
        {
            var roleManager = sevices.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = sevices.GetRequiredService<UserManager<IdentityUser>>();
            var context = sevices.GetRequiredService<ApplicationDbContext>();


            // Roles se toman desde el enum Perfil
            var roles = Enum.GetNames(typeof(Perfil));
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Usuario Administrador
            var AdminUser = await userManager.FindByEmailAsync("admin@proyecto.com");
            if (AdminUser == null)
            {
                AdminUser = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@proyecto.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(AdminUser, "admin123");
                await userManager.AddToRoleAsync(AdminUser, Perfil.Administrador.ToString());
            }


            // Usuario Médico de prueba

            var MedicoUser = await userManager.FindByEmailAsync("medico@proyecto.com");
            if (MedicoUser == null)
            {
                MedicoUser = new IdentityUser
                {
                    UserName = "medico",
                    Email = "medico@proyecto.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(MedicoUser, "medico123");
                await userManager.AddToRoleAsync(MedicoUser, Perfil.Medico.ToString());

                // Crear el registro Medico vinculado al usuario
                if (context.Medicos.Any(m => m.UsuarioCedula == MedicoUser.Id) == false)
                {
                    var medico = new Medico
                    {
                        NombreCompleto = "Dr. Juan Pérez",
                        NumeroColegiado = "MED-001",
                        UsuarioCedula = MedicoUser.Id
                    };
                    context.Medicos.Add(medico);
                    await context.SaveChangesAsync();
                }
            }

            // Usuario Paciente de prueba
            var pacienteUser = await userManager.FindByEmailAsync("paciente@medicalsystem.com");

            if (pacienteUser == null)
            {
                pacienteUser = new IdentityUser
                {
                    UserName = "paciente",
                    Email = "paciente@medicalsystem.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(pacienteUser, "paciente123");
                await userManager.AddToRoleAsync(pacienteUser, Perfil.Paciente.ToString());

                // Crear el registro Paciente vinculado al usuario
                if (!context.Pacientes.Any(p => p.UsuarioId == pacienteUser.Id))
                {
                    context.Pacientes.Add(new Paciente
                    {
                        Cedula = "1-0000-0000",
                        NombreCompleto = "María García López",
                        Correo = "paciente@medicalsystem.com",
                        UsuarioId = pacienteUser.Id
                    });

                    await context.SaveChangesAsync();
                }
            }

            // Especialidades de ejemplo
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

            // Padecimientos de ejemplo
            if (!context.Padecimientos.Any())
            {
                context.Padecimientos.AddRange(
                    new Padecimiento { Nombre = "Hipertensión", Descripcion = "Presión arterial elevada de forma crónica."},
                    new Padecimiento { Nombre = "Diabetes Tipo 2", Descripcion = "Trastorno metabólico con niveles altos de glucosa."},
                    new Padecimiento{ Nombre = "Asma", Descripcion = "Enfermedad inflamatoria crónica de las vías respiratorias."}
                );

                await context.SaveChangesAsync();
            }

            // Tratamientos de ejemplo
            if (!context.Tratamientos.Any())
            {
                context.Tratamientos.AddRange(
                    new Tratamiento { Nombre = "Terapia Física", Descripcion = "Rehabilitación mediante ejercicios dirigidos."},
                    new Tratamiento { Nombre = "Dieta Balanceada", Descripcion = "Plan nutricional personalizado." },
                    new Tratamiento{ Nombre = "Control de Glucosa", Descripcion = "Monitoreo diario de niveles de azúcar."}
                );

                await context.SaveChangesAsync();
            }

            // Medicamentos de ejemplo
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