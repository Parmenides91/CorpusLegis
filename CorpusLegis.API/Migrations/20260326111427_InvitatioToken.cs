using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CorpusLegis.API.Migrations
{
    /// <inheritdoc />
    public partial class InvitatioToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RespondedAt",
                table: "Invitationes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Invitationes",
                type: "nvarchar(43)",
                maxLength: 43,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "Invitationes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RespondedAt",
                table: "Invitationes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
