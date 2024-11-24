using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Solucao.Application.Migrations
{
    public partial class ChangedTableModelsAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModelAttributes_Models_modelId",
                table: "ModelAttributes");

            migrationBuilder.RenameColumn(
                name: "modelId",
                table: "ModelAttributes",
                newName: "ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_ModelAttributes_modelId",
                table: "ModelAttributes",
                newName: "IX_ModelAttributes_ModelId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModelId",
                table: "ModelAttributes",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_ModelAttributes_Models_ModelId",
                table: "ModelAttributes",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModelAttributes_Models_ModelId",
                table: "ModelAttributes");

            migrationBuilder.RenameColumn(
                name: "ModelId",
                table: "ModelAttributes",
                newName: "modelId");

            migrationBuilder.RenameIndex(
                name: "IX_ModelAttributes_ModelId",
                table: "ModelAttributes",
                newName: "IX_ModelAttributes_modelId");

            migrationBuilder.AlterColumn<Guid>(
                name: "modelId",
                table: "ModelAttributes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelAttributes_Models_modelId",
                table: "ModelAttributes",
                column: "modelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
