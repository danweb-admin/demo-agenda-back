using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Solucao.Application.Migrations
{
    public partial class AddedNewColumnColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Equipaments",
                type: "varchar(7)",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Equipaments");
        }
    }
}
