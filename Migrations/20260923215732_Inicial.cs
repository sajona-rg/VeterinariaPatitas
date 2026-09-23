using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VeterinariaMVC.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Especies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Propietarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Telefono = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Direccion = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propietarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Razas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IdEspecie = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Razas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Razas_Especies_IdEspecie",
                        column: x => x.IdEspecie,
                        principalTable: "Especies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mascotas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IdPropietario = table.Column<int>(type: "int", nullable: false),
                    IdRaza = table.Column<int>(type: "int", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "date", nullable: true),
                    Peso = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RutaFoto = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mascotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mascotas_Propietarios_IdPropietario",
                        column: x => x.IdPropietario,
                        principalTable: "Propietarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mascotas_Razas_IdRaza",
                        column: x => x.IdRaza,
                        principalTable: "Razas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Especies",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Canino" },
                    { 2, "Felino" },
                    { 3, "Ave" }
                });

            migrationBuilder.InsertData(
                table: "Razas",
                columns: new[] { "Id", "IdEspecie", "Nombre" },
                values: new object[,]
                {
                    { 1, 1, "Labrador Retriever" },
                    { 2, 1, "Pastor Alemán" },
                    { 3, 1, "Bulldog Francés" },
                    { 4, 1, "Criollo" },
                    { 5, 2, "Siamés" },
                    { 6, 2, "Persa" },
                    { 7, 2, "Angora" },
                    { 8, 3, "Periquito" },
                    { 9, 3, "Canario" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_IdPropietario",
                table: "Mascotas",
                column: "IdPropietario");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_IdRaza",
                table: "Mascotas",
                column: "IdRaza");

            migrationBuilder.CreateIndex(
                name: "IX_Razas_IdEspecie",
                table: "Razas",
                column: "IdEspecie");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mascotas");

            migrationBuilder.DropTable(
                name: "Propietarios");

            migrationBuilder.DropTable(
                name: "Razas");

            migrationBuilder.DropTable(
                name: "Especies");
        }
    }
}
