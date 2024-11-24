using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Solucao.Application.Migrations
{
    public partial class AddedNewTableEquipmentRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentRelantionships",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentRelantionships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentRelationshipEquipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EquipmentRelationshipId = table.Column<Guid>(nullable: false),
                    EquipmentId = table.Column<Guid>(nullable: false),
                    EquipmentRelantionshipId = table.Column<Guid>(nullable: true),
                    EquipamentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentRelationshipEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentRelationshipEquipment_Equipaments_EquipamentId",
                        column: x => x.EquipamentId,
                        principalTable: "Equipaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentRelationshipEquipment_EquipmentRelantionships_EquipmentRelantionshipId",
                        column: x => x.EquipmentRelantionshipId,
                        principalTable: "EquipmentRelantionships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRelationshipEquipment_EquipamentId",
                table: "EquipmentRelationshipEquipment",
                column: "EquipamentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRelationshipEquipment_EquipmentRelantionshipId",
                table: "EquipmentRelationshipEquipment",
                column: "EquipmentRelantionshipId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentRelationshipEquipment");

            migrationBuilder.DropTable(
                name: "EquipmentRelantionships");
        }
    }
}
