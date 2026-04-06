using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CorpusLegis.API.Migrations
{
    /// <inheritdoc />
    public partial class CivitasVisibility001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CivisCivitas");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Civitates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Visibility",
                table: "Civitates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CivitasSodales",
                columns: table => new
                {
                    CivitasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CivitasSodales", x => new { x.CivitasId, x.CivisId });
                    table.ForeignKey(
                        name: "FK_CivitasSodales_Cives_CivisId",
                        column: x => x.CivisId,
                        principalTable: "Cives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CivitasSodales_Civitates_CivitasId",
                        column: x => x.CivitasId,
                        principalTable: "Civitates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invitationes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivitasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InviterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InviteeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitationes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitationes_Cives_InviteeId",
                        column: x => x.InviteeId,
                        principalTable: "Cives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitationes_Cives_InviterId",
                        column: x => x.InviterId,
                        principalTable: "Cives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitationes_Civitates_CivitasId",
                        column: x => x.CivitasId,
                        principalTable: "Civitates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CivitasSodales_CivisId",
                table: "CivitasSodales",
                column: "CivisId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitationes_CivitasId",
                table: "Invitationes",
                column: "CivitasId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitationes_InviteeId",
                table: "Invitationes",
                column: "InviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitationes_InviterId",
                table: "Invitationes",
                column: "InviterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CivitasSodales");

            migrationBuilder.DropTable(
                name: "Invitationes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Civitates");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Civitates");

            migrationBuilder.CreateTable(
                name: "CivisCivitas",
                columns: table => new
                {
                    CivesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivitatesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CivisCivitas", x => new { x.CivesId, x.CivitatesId });
                    table.ForeignKey(
                        name: "FK_CivisCivitas_Cives_CivesId",
                        column: x => x.CivesId,
                        principalTable: "Cives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CivisCivitas_Civitates_CivitatesId",
                        column: x => x.CivitatesId,
                        principalTable: "Civitates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CivisCivitas_CivitatesId",
                table: "CivisCivitas",
                column: "CivitatesId");
        }
    }
}
