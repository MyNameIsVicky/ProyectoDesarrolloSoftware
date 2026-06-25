using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoDesarrolloSoftware.Migrations
{
    /// <inheritdoc />
    public partial class dbfinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamenLaboratorio_Medicos_MedicoId",
                table: "ExamenLaboratorio");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamenLaboratorio_Pacientes_PacienteId",
                table: "ExamenLaboratorio");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamenMedico_Medicos_MedicoId",
                table: "ExamenMedico");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamenMedico_Pacientes_PacienteId",
                table: "ExamenMedico");

            migrationBuilder.RenameTable(
                name: "ExamenMedico",
                newName: "Examenes");

            migrationBuilder.RenameTable(
                name: "ExamenLaboratorio",
                newName: "ExamenesLaboratorio");

            migrationBuilder.AlterColumn<string>(
                name: "CedulaFisica",
                table: "Medicos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PacienteId",
                table: "Examenes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MedicoId",
                table: "Examenes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Examenes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "ArchivoRuta",
                table: "Examenes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Examenes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaSubida",
                table: "Examenes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TipoArchivo",
                table: "Examenes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "PacienteId",
                table: "ExamenesLaboratorio",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MedicoId",
                table: "ExamenesLaboratorio",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ExamenesLaboratorio",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "ExamenesLaboratorio",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "ExamenesLaboratorio",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "ExamenesLaboratorio",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreArchivo",
                table: "ExamenesLaboratorio",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RutaArchivo",
                table: "ExamenesLaboratorio",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Examenes",
                table: "Examenes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExamenesLaboratorio",
                table: "ExamenesLaboratorio",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Examenes_MedicoId",
                table: "Examenes",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Examenes_PacienteId",
                table: "Examenes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamenesLaboratorio_MedicoId",
                table: "ExamenesLaboratorio",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamenesLaboratorio_PacienteId",
                table: "ExamenesLaboratorio",
                column: "PacienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Examenes_Medicos_MedicoId",
                table: "Examenes",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Examenes_Pacientes_PacienteId",
                table: "Examenes",
                column: "PacienteId",
                principalTable: "Pacientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamenesLaboratorio_Medicos_MedicoId",
                table: "ExamenesLaboratorio",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamenesLaboratorio_Pacientes_PacienteId",
                table: "ExamenesLaboratorio",
                column: "PacienteId",
                principalTable: "Pacientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Examenes_Medicos_MedicoId",
                table: "Examenes");

            migrationBuilder.DropForeignKey(
                name: "FK_Examenes_Pacientes_PacienteId",
                table: "Examenes");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamenesLaboratorio_Medicos_MedicoId",
                table: "ExamenesLaboratorio");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamenesLaboratorio_Pacientes_PacienteId",
                table: "ExamenesLaboratorio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExamenesLaboratorio",
                table: "ExamenesLaboratorio");

            migrationBuilder.DropIndex(
                name: "IX_ExamenesLaboratorio_MedicoId",
                table: "ExamenesLaboratorio");

            migrationBuilder.DropIndex(
                name: "IX_ExamenesLaboratorio_PacienteId",
                table: "ExamenesLaboratorio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Examenes",
                table: "Examenes");

            migrationBuilder.DropIndex(
                name: "IX_Examenes_MedicoId",
                table: "Examenes");

            migrationBuilder.DropIndex(
                name: "IX_Examenes_PacienteId",
                table: "Examenes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ExamenesLaboratorio");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "ExamenesLaboratorio");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "ExamenesLaboratorio");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "ExamenesLaboratorio");

            migrationBuilder.DropColumn(
                name: "NombreArchivo",
                table: "ExamenesLaboratorio");

            migrationBuilder.DropColumn(
                name: "RutaArchivo",
                table: "ExamenesLaboratorio");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Examenes");

            migrationBuilder.DropColumn(
                name: "ArchivoRuta",
                table: "Examenes");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Examenes");

            migrationBuilder.DropColumn(
                name: "FechaSubida",
                table: "Examenes");

            migrationBuilder.DropColumn(
                name: "TipoArchivo",
                table: "Examenes");

            migrationBuilder.RenameTable(
                name: "ExamenesLaboratorio",
                newName: "ExamenLaboratorio");

            migrationBuilder.RenameTable(
                name: "Examenes",
                newName: "ExamenMedico");

            migrationBuilder.AlterColumn<string>(
                name: "CedulaFisica",
                table: "Medicos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "PacienteId",
                table: "ExamenLaboratorio",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MedicoId",
                table: "ExamenLaboratorio",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PacienteId",
                table: "ExamenMedico",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MedicoId",
                table: "ExamenMedico",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamenLaboratorio_Medicos_MedicoId",
                table: "ExamenLaboratorio",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamenLaboratorio_Pacientes_PacienteId",
                table: "ExamenLaboratorio",
                column: "PacienteId",
                principalTable: "Pacientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamenMedico_Medicos_MedicoId",
                table: "ExamenMedico",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamenMedico_Pacientes_PacienteId",
                table: "ExamenMedico",
                column: "PacienteId",
                principalTable: "Pacientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
