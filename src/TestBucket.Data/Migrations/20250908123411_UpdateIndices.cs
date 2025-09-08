using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_testcases_Name",
                table: "testcases");

            migrationBuilder.DropIndex(
                name: "IX_testcases_TenantId_TestProjectId_AutomationAssembly_ClassNa~",
                table: "testcases");

            migrationBuilder.DropIndex(
                name: "IX_testcases_TenantId_TestProjectId_ExternalId",
                table: "testcases");

            migrationBuilder.CreateIndex(
                name: "IX_testcases_Name_Created",
                table: "testcases",
                columns: new[] { "Name", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TenantId_TestProjectId_ExternalId_Created",
                table: "testcases",
                columns: new[] { "TenantId", "TestProjectId", "ExternalId", "Created" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_testcases_Name_Created",
                table: "testcases");

            migrationBuilder.DropIndex(
                name: "IX_testcases_TenantId_TestProjectId_ExternalId_Created",
                table: "testcases");

            migrationBuilder.CreateIndex(
                name: "IX_testcases_Name",
                table: "testcases",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TenantId_TestProjectId_AutomationAssembly_ClassNa~",
                table: "testcases",
                columns: new[] { "TenantId", "TestProjectId", "AutomationAssembly", "ClassName", "Module", "Method" });

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TenantId_TestProjectId_ExternalId",
                table: "testcases",
                columns: new[] { "TenantId", "TestProjectId", "ExternalId" });
        }
    }
}
