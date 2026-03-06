using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CorpusLegis.API.Migrations
{
    /// <inheritdoc />
    public partial class QuorumAndMajorityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RequiredMajority",
                table: "Rogationes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RequiredQuorum",
                table: "Rogationes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredMajority",
                table: "Rogationes");

            migrationBuilder.DropColumn(
                name: "RequiredQuorum",
                table: "Rogationes");
        }
    }
}
