using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestBucket.Domain.Code.Yaml.Models;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProductSystemAndDisplayOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DevResponsible",
                table: "Features",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DisplayOptions>(
                name: "Display",
                table: "Features",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DevResponsible",
                table: "Component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DisplayOptions>(
                name: "Display",
                table: "Component",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<DisplayOptions>(
                name: "Display",
                table: "ArchitecturalLayers",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductSystems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    GlobPatterns = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Display = table.Column<DisplayOptions>(type: "jsonb", nullable: true),
                    DevResponsible = table.Column<string>(type: "text", nullable: true),
                    DevLead = table.Column<string>(type: "text", nullable: true),
                    TestLead = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSystems_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductSystems_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductSystems_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSystems_TeamId",
                table: "ProductSystems",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSystems_TenantId",
                table: "ProductSystems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSystems_TestProjectId",
                table: "ProductSystems",
                column: "TestProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSystems");

            migrationBuilder.DropColumn(
                name: "DevResponsible",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "Display",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "DevResponsible",
                table: "Component");

            migrationBuilder.DropColumn(
                name: "Display",
                table: "Component");

            migrationBuilder.DropColumn(
                name: "Display",
                table: "ArchitecturalLayers");
        }
    }
}
