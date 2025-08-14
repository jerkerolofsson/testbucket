using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class LlmUsageCostSplitInAndOutPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsdPerMillionTokens",
                table: "ChatUsages",
                newName: "UsdPerMillionOutputTokens");

            migrationBuilder.AddColumn<double>(
                name: "UsdPerMillionInputTokens",
                table: "ChatUsages",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsdPerMillionInputTokens",
                table: "ChatUsages");

            migrationBuilder.RenameColumn(
                name: "UsdPerMillionOutputTokens",
                table: "ChatUsages",
                newName: "UsdPerMillionTokens");
        }
    }
}
