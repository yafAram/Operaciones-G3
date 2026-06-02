using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.G3Operaciones.Garcias.MX.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperatorAssignmentPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorAssignmentPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperatorAssignmentTarget",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorAssignmentPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChainId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorAssignmentTarget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperatorAssignmentTarget_OperatorAssignmentPlans_OperatorAssignmentPlanId",
                        column: x => x.OperatorAssignmentPlanId,
                        principalTable: "OperatorAssignmentPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperatorAssignmentTarget_OperatorAssignmentPlanId",
                table: "OperatorAssignmentTarget",
                column: "OperatorAssignmentPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperatorAssignmentTarget");

            migrationBuilder.DropTable(
                name: "OperatorAssignmentPlans");
        }
    }
}
