using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingVectorsOnTestAndArtifactComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "testcases",
                type: "vector(384)",
                nullable: true);

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "ProductSystems",
                type: "vector(384)",
                nullable: true);

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "Features",
                type: "vector(384)",
                nullable: true);

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "Components",
                type: "vector(384)",
                nullable: true);

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "ArchitecturalLayers",
                type: "vector(384)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "testcases");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "ProductSystems");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "ArchitecturalLayers");
        }
    }
}
