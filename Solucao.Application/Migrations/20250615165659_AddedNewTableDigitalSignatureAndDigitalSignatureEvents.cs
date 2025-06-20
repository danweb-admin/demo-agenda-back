using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Solucao.Application.Migrations
{
    public partial class AddedNewTableDigitalSignatureAndDigitalSignatureEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DigitalSignatureEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdProcesso = table.Column<Guid>(nullable: false),
                    Evento = table.Column<string>(maxLength: 200, nullable: false),
                    IdConta = table.Column<Guid>(nullable: false),
                    IdWebhook = table.Column<Guid>(nullable: false),
                    DataHoraAtual = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalSignatureEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DigitalSignatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IdProcesso = table.Column<Guid>(nullable: false),
                    IdPasta = table.Column<Guid>(nullable: false),
                    IdResponsavel = table.Column<Guid>(nullable: false),
                    CalendarId = table.Column<Guid>(nullable: false),
                    NomeProcesso = table.Column<string>(maxLength: 150, nullable: false),
                    Status = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalSignatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DigitalSignatures_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DigitalSignatures_CalendarId",
                table: "DigitalSignatures",
                column: "CalendarId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DigitalSignatureEvents");

            migrationBuilder.DropTable(
                name: "DigitalSignatures");
        }
    }
}
