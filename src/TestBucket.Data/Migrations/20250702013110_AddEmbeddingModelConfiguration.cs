using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingModelConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LlmTestGenerationModel",
                table: "GlobalSettings",
                newName: "LlmEmbeddingModel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LlmEmbeddingModel",
                table: "GlobalSettings",
                newName: "LlmTestGenerationModel");
        }
    }
}
