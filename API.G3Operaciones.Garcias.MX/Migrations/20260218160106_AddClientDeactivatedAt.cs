using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.G3Operaciones.Garcias.MX.Migrations
{
    /// <inheritdoc />
    public partial class AddClientDeactivatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAt",
                table: "Clients",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeactivatedAt",
                table: "Clients");
        }
    }
}
