using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AISettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiProvider",
                table: "GlobalSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AiProviderUrl",
                table: "GlobalSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AzureAiProductionKey",
                table: "GlobalSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GithubModelsDeveloperKey",
                table: "GlobalSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LlmModel",
                table: "GlobalSettings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiProvider",
                table: "GlobalSettings");

            migrationBuilder.DropColumn(
                name: "AiProviderUrl",
                table: "GlobalSettings");

            migrationBuilder.DropColumn(
                name: "AzureAiProductionKey",
                table: "GlobalSettings");

            migrationBuilder.DropColumn(
                name: "GithubModelsDeveloperKey",
                table: "GlobalSettings");

            migrationBuilder.DropColumn(
                name: "LlmModel",
                table: "GlobalSettings");
        }
    }
}
