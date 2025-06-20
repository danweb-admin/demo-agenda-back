using Microsoft.EntityFrameworkCore.Migrations;

namespace Solucao.Application.Migrations
{
    public partial class AddedNewColumnIsPF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPF",
                table: "ClientDigitalSignatures",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientDigitalSignatures_ClientId",
                table: "ClientDigitalSignatures",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientDigitalSignatures_Clients_ClientId",
                table: "ClientDigitalSignatures",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientDigitalSignatures_Clients_ClientId",
                table: "ClientDigitalSignatures");

            migrationBuilder.DropIndex(
                name: "IX_ClientDigitalSignatures_ClientId",
                table: "ClientDigitalSignatures");

            migrationBuilder.DropColumn(
                name: "IsPF",
                table: "ClientDigitalSignatures");
        }
    }
}
