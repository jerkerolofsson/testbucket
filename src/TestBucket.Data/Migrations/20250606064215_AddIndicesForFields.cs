using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndicesForFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_test_case_run_fields_TenantId",
                table: "test_case_run_fields");

            migrationBuilder.DropIndex(
                name: "IX_test_case_fields_TenantId",
                table: "test_case_fields");

            migrationBuilder.DropIndex(
                name: "IX_requirement_fields_TenantId",
                table: "requirement_fields");

            migrationBuilder.DropIndex(
                name: "IX_issue_fields_TenantId",
                table: "issue_fields");

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TenantId_TestProjectId_AutomationAssembly_ClassNa~",
                table: "testcases",
                columns: new[] { "TenantId", "TestProjectId", "AutomationAssembly", "ClassName", "Module", "Method" });

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TenantId_TestProjectId_ExternalId",
                table: "testcases",
                columns: new[] { "TenantId", "TestProjectId", "ExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_test_case_run_fields_TenantId_TestCaseRunId",
                table: "test_case_run_fields",
                columns: new[] { "TenantId", "TestCaseRunId" });

            migrationBuilder.CreateIndex(
                name: "IX_test_case_fields_TenantId_TestCaseId",
                table: "test_case_fields",
                columns: new[] { "TenantId", "TestCaseId" });

            migrationBuilder.CreateIndex(
                name: "IX_requirement_fields_TenantId_RequirementId",
                table: "requirement_fields",
                columns: new[] { "TenantId", "RequirementId" });

            migrationBuilder.CreateIndex(
                name: "IX_issue_fields_TenantId_LocalIssueId",
                table: "issue_fields",
                columns: new[] { "TenantId", "LocalIssueId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_testcases_TenantId_TestProjectId_AutomationAssembly_ClassNa~",
                table: "testcases");

            migrationBuilder.DropIndex(
                name: "IX_testcases_TenantId_TestProjectId_ExternalId",
                table: "testcases");

            migrationBuilder.DropIndex(
                name: "IX_test_case_run_fields_TenantId_TestCaseRunId",
                table: "test_case_run_fields");

            migrationBuilder.DropIndex(
                name: "IX_test_case_fields_TenantId_TestCaseId",
                table: "test_case_fields");

            migrationBuilder.DropIndex(
                name: "IX_requirement_fields_TenantId_RequirementId",
                table: "requirement_fields");

            migrationBuilder.DropIndex(
                name: "IX_issue_fields_TenantId_LocalIssueId",
                table: "issue_fields");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_run_fields_TenantId",
                table: "test_case_run_fields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_fields_TenantId",
                table: "test_case_fields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_fields_TenantId",
                table: "requirement_fields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_issue_fields_TenantId",
                table: "issue_fields",
                column: "TenantId");
        }
    }
}
