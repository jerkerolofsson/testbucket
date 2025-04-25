using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestCaseAndSuiteSlugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_testcases_TenantId",
                table: "testcases");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "testsuites",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_testsuites_TenantId_Slug",
                table: "testsuites",
                columns: new[] { "TenantId", "Slug" });

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TenantId_Slug",
                table: "testcases",
                columns: new[] { "TenantId", "Slug" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_testsuites_TenantId_Slug",
                table: "testsuites");

            migrationBuilder.DropIndex(
                name: "IX_testcases_TenantId_Slug",
                table: "testcases");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "testsuites");

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TenantId",
                table: "testcases",
                column: "TenantId");
        }
    }
}
