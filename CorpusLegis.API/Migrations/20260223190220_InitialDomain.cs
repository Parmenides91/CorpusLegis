using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CorpusLegis.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Civitates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FoundedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Civitates", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Rogationes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CivisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivitasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rogationes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rogationes_Cives_CivisId",
                        column: x => x.CivisId,
                        principalTable: "Cives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rogationes_Civitates_CivitasId",
                        column: x => x.CivitasId,
                        principalTable: "Civitates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Leges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivitasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginRogatioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PromulgatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DerogatedByLexId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leges_Civitates_CivitasId",
                        column: x => x.CivitasId,
                        principalTable: "Civitates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Leges_Leges_DerogatedByLexId",
                        column: x => x.DerogatedByLexId,
                        principalTable: "Leges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Leges_Rogationes_OriginRogatioId",
                        column: x => x.OriginRogatioId,
                        principalTable: "Rogationes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sententiae",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RogatioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sententiae", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sententiae_Cives_CivisId",
                        column: x => x.CivisId,
                        principalTable: "Cives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sententiae_Rogationes_RogatioId",
                        column: x => x.RogatioId,
                        principalTable: "Rogationes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Suffragia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RogatioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Votum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CastAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suffragia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suffragia_Cives_CivisId",
                        column: x => x.CivisId,
                        principalTable: "Cives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suffragia_Rogationes_RogatioId",
                        column: x => x.RogatioId,
                        principalTable: "Rogationes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CivisCivitas_CivitatesId",
                table: "CivisCivitas",
                column: "CivitatesId");

            migrationBuilder.CreateIndex(
                name: "IX_Leges_CivitasId",
                table: "Leges",
                column: "CivitasId");

            migrationBuilder.CreateIndex(
                name: "IX_Leges_DerogatedByLexId",
                table: "Leges",
                column: "DerogatedByLexId");

            migrationBuilder.CreateIndex(
                name: "IX_Leges_OriginRogatioId",
                table: "Leges",
                column: "OriginRogatioId");

            migrationBuilder.CreateIndex(
                name: "IX_Rogationes_CivisId",
                table: "Rogationes",
                column: "CivisId");

            migrationBuilder.CreateIndex(
                name: "IX_Rogationes_CivitasId",
                table: "Rogationes",
                column: "CivitasId");

            migrationBuilder.CreateIndex(
                name: "IX_Sententiae_CivisId",
                table: "Sententiae",
                column: "CivisId");

            migrationBuilder.CreateIndex(
                name: "IX_Sententiae_RogatioId",
                table: "Sententiae",
                column: "RogatioId");

            migrationBuilder.CreateIndex(
                name: "IX_Suffragia_CivisId",
                table: "Suffragia",
                column: "CivisId");

            migrationBuilder.CreateIndex(
                name: "IX_Suffragia_RogatioId_CivisId",
                table: "Suffragia",
                columns: new[] { "RogatioId", "CivisId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CivisCivitas");

            migrationBuilder.DropTable(
                name: "Leges");

            migrationBuilder.DropTable(
                name: "Sententiae");

            migrationBuilder.DropTable(
                name: "Suffragia");

            migrationBuilder.DropTable(
                name: "Rogationes");

            migrationBuilder.DropTable(
                name: "Cives");

            migrationBuilder.DropTable(
                name: "Civitates");
        }
    }
}
