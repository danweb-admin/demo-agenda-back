using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Solucao.Application.Migrations
{
    public partial class addednewcolumnsfiletocalendar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "IdProcesso",
                table: "DigitalSignatures",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "FileNameDocx",
                table: "Calendars",
                type: "varchar(150)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileNamePdf",
                table: "Calendars",
                type: "varchar(150)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileNameDocx",
                table: "Calendars");

            migrationBuilder.DropColumn(
                name: "FileNamePdf",
                table: "Calendars");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdProcesso",
                table: "DigitalSignatures",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
