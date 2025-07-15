using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingAiProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmbeddingAiProvider",
                table: "GlobalSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmbeddingAiProviderUrl",
                table: "GlobalSettings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmbeddingAiProvider",
                table: "GlobalSettings");

            migrationBuilder.DropColumn(
                name: "EmbeddingAiProviderUrl",
                table: "GlobalSettings");
        }
    }
}
