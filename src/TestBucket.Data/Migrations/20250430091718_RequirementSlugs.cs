using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequirementSlugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_requirements_TenantId",
                table: "requirements");

            migrationBuilder.DropIndex(
                name: "IX_projects_TenantId",
                table: "projects");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "spec",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_spec_TenantId_Slug",
                table: "spec",
                columns: new[] { "TenantId", "Slug" });

            migrationBuilder.CreateIndex(
                name: "IX_requirements_TenantId_Slug",
                table: "requirements",
                columns: new[] { "TenantId", "Slug" });

            migrationBuilder.CreateIndex(
                name: "IX_projects_TenantId_Slug",
                table: "projects",
                columns: new[] { "TenantId", "Slug" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_spec_TenantId_Slug",
                table: "spec");

            migrationBuilder.DropIndex(
                name: "IX_requirements_TenantId_Slug",
                table: "requirements");

            migrationBuilder.DropIndex(
                name: "IX_projects_TenantId_Slug",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "spec");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_TenantId",
                table: "requirements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_projects_TenantId",
                table: "projects",
                column: "TenantId");
        }
    }
}
