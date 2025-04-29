using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class CommitWithNamesForImpact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArchitecturalLayers_Commits_CommitId",
                table: "ArchitecturalLayers");

            migrationBuilder.DropForeignKey(
                name: "FK_Components_Commits_CommitId",
                table: "Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Features_Commits_CommitId",
                table: "Features");

            migrationBuilder.DropIndex(
                name: "IX_Features_CommitId",
                table: "Features");

            migrationBuilder.DropIndex(
                name: "IX_Components_CommitId",
                table: "Components");

            migrationBuilder.DropIndex(
                name: "IX_ArchitecturalLayers_CommitId",
                table: "ArchitecturalLayers");

            migrationBuilder.DropColumn(
                name: "CommitId",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "CommitId",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "CommitId",
                table: "ArchitecturalLayers");

            migrationBuilder.AddColumn<List<string>>(
                name: "ComponentNames",
                table: "Commits",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "FeatureNames",
                table: "Commits",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "LayerNames",
                table: "Commits",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "SystemNames",
                table: "Commits",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComponentNames",
                table: "Commits");

            migrationBuilder.DropColumn(
                name: "FeatureNames",
                table: "Commits");

            migrationBuilder.DropColumn(
                name: "LayerNames",
                table: "Commits");

            migrationBuilder.DropColumn(
                name: "SystemNames",
                table: "Commits");

            migrationBuilder.AddColumn<long>(
                name: "CommitId",
                table: "Features",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CommitId",
                table: "Components",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CommitId",
                table: "ArchitecturalLayers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Features_CommitId",
                table: "Features",
                column: "CommitId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_CommitId",
                table: "Components",
                column: "CommitId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchitecturalLayers_CommitId",
                table: "ArchitecturalLayers",
                column: "CommitId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchitecturalLayers_Commits_CommitId",
                table: "ArchitecturalLayers",
                column: "CommitId",
                principalTable: "Commits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Commits_CommitId",
                table: "Components",
                column: "CommitId",
                principalTable: "Commits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Commits_CommitId",
                table: "Features",
                column: "CommitId",
                principalTable: "Commits",
                principalColumn: "Id");
        }
    }
}
