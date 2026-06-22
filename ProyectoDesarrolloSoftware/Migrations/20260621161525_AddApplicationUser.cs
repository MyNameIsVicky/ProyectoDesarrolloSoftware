using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoDesarrolloSoftware.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioCedula",
                table: "Medicos");

            migrationBuilder.AddColumn<int>(
                name: "MedicoId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Perfil",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MedicoId",
                table: "AspNetUsers",
                column: "MedicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Medicos_MedicoId",
                table: "AspNetUsers",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Medicos_MedicoId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MedicoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MedicoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Perfil",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCedula",
                table: "Medicos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
