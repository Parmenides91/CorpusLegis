using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CorpusLegis.API.Migrations
{
    /// <inheritdoc />
    public partial class Sententiae : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Sententiae",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByCivisId",
                table: "Sententiae",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Sententiae",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Sententiae",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Sententiae",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sententiae_ParentId",
                table: "Sententiae",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sententiae_Sententiae_ParentId",
                table: "Sententiae",
                column: "ParentId",
                principalTable: "Sententiae",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sententiae_Sententiae_ParentId",
                table: "Sententiae");

            migrationBuilder.DropIndex(
                name: "IX_Sententiae_ParentId",
                table: "Sententiae");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Sententiae");

            migrationBuilder.DropColumn(
                name: "DeletedByCivisId",
                table: "Sententiae");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Sententiae");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Sententiae");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Sententiae");
        }
    }
}
