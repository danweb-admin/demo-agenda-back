using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Solucao.Application.Migrations
{
    public partial class AddedUserIdToPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "People",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_People_Users_UserId",
                table: "People",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Users_UserId",
                table: "People");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "People",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_People_Users_UserId",
                table: "People",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
