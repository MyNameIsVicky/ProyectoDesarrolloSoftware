using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoDesarrolloSoftware.Migrations
{
    /// <inheritdoc />
    public partial class AgregaExamenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpedienteMedicamentos_Tratamientos_TratamientoId",
                table: "ExpedienteMedicamentos");

            migrationBuilder.RenameColumn(
                name: "fechaSuspension",
                table: "ExpedientePadecimientos",
                newName: "FechaSuspension");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "ExpedientePadecimientos",
                newName: "Suspendido");

            migrationBuilder.RenameColumn(
                name: "TratamientoId",
                table: "ExpedienteMedicamentos",
                newName: "MedicamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_ExpedienteMedicamentos_TratamientoId",
                table: "ExpedienteMedicamentos",
                newName: "IX_ExpedienteMedicamentos_MedicamentoId");

            migrationBuilder.CreateTable(
                name: "Examenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    MedicoId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ArchivoRuta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TipoArchivo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Examenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Examenes_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Examenes_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Examenes_MedicoId",
                table: "Examenes",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Examenes_PacienteId",
                table: "Examenes",
                column: "PacienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpedienteMedicamentos_Medicamentos_MedicamentoId",
                table: "ExpedienteMedicamentos",
                column: "MedicamentoId",
                principalTable: "Medicamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpedienteMedicamentos_Medicamentos_MedicamentoId",
                table: "ExpedienteMedicamentos");

            migrationBuilder.DropTable(
                name: "Examenes");

            migrationBuilder.RenameColumn(
                name: "FechaSuspension",
                table: "ExpedientePadecimientos",
                newName: "fechaSuspension");

            migrationBuilder.RenameColumn(
                name: "Suspendido",
                table: "ExpedientePadecimientos",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "MedicamentoId",
                table: "ExpedienteMedicamentos",
                newName: "TratamientoId");

            migrationBuilder.RenameIndex(
                name: "IX_ExpedienteMedicamentos_MedicamentoId",
                table: "ExpedienteMedicamentos",
                newName: "IX_ExpedienteMedicamentos_TratamientoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpedienteMedicamentos_Tratamientos_TratamientoId",
                table: "ExpedienteMedicamentos",
                column: "TratamientoId",
                principalTable: "Tratamientos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
