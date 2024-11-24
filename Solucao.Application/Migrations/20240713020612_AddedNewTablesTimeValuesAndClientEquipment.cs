using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Solucao.Application.Migrations
{
    public partial class AddedNewTablesTimeValuesAndClientEquipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentRelationshipEquipment_EquipmentRelantionships_EquipmentRelantionshipId",
                table: "EquipmentRelationshipEquipment");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentRelationshipEquipment_EquipmentRelantionshipId",
                table: "EquipmentRelationshipEquipment");

            migrationBuilder.DropColumn(
                name: "EquipmentRelantionshipId",
                table: "EquipmentRelationshipEquipment");

            migrationBuilder.CreateTable(
                name: "ClientEquipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    EquipmentRelationshipId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientEquipment_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientEquipment_EquipmentRelantionships_EquipmentRelationshipId",
                        column: x => x.EquipmentRelationshipId,
                        principalTable: "EquipmentRelantionships",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimeValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Time = table.Column<string>(type: "char(5)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClientEquipmentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeValues_ClientEquipment_ClientEquipmentId",
                        column: x => x.ClientEquipmentId,
                        principalTable: "ClientEquipment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRelationshipEquipment_EquipmentRelationshipId",
                table: "EquipmentRelationshipEquipment",
                column: "EquipmentRelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEquipment_ClientId",
                table: "ClientEquipment",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEquipment_EquipmentRelationshipId",
                table: "ClientEquipment",
                column: "EquipmentRelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeValues_ClientEquipmentId",
                table: "TimeValues",
                column: "ClientEquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentRelationshipEquipment_EquipmentRelantionships_EquipmentRelationshipId",
                table: "EquipmentRelationshipEquipment",
                column: "EquipmentRelationshipId",
                principalTable: "EquipmentRelantionships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentRelationshipEquipment_EquipmentRelantionships_EquipmentRelationshipId",
                table: "EquipmentRelationshipEquipment");

            migrationBuilder.DropTable(
                name: "TimeValues");

            migrationBuilder.DropTable(
                name: "ClientEquipment");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentRelationshipEquipment_EquipmentRelationshipId",
                table: "EquipmentRelationshipEquipment");

            migrationBuilder.AddColumn<Guid>(
                name: "EquipmentRelantionshipId",
                table: "EquipmentRelationshipEquipment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRelationshipEquipment_EquipmentRelantionshipId",
                table: "EquipmentRelationshipEquipment",
                column: "EquipmentRelantionshipId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentRelationshipEquipment_EquipmentRelantionships_EquipmentRelantionshipId",
                table: "EquipmentRelationshipEquipment",
                column: "EquipmentRelantionshipId",
                principalTable: "EquipmentRelantionships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
