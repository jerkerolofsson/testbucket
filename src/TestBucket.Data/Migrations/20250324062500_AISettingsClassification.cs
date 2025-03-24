using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AISettingsClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LlmClassificationModel",
                table: "GlobalSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LlmTestGenerationModel",
                table: "GlobalSettings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LlmClassificationModel",
                table: "GlobalSettings");

            migrationBuilder.DropColumn(
                name: "LlmTestGenerationModel",
                table: "GlobalSettings");
        }
    }
}
