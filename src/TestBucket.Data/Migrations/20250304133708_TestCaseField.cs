using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestCaseField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "test_case_fields",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_test_case_fields_TenantId",
                table: "test_case_fields",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_case_fields_Tenants_TenantId",
                table: "test_case_fields",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_case_fields_Tenants_TenantId",
                table: "test_case_fields");

            migrationBuilder.DropIndex(
                name: "IX_test_case_fields_TenantId",
                table: "test_case_fields");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "test_case_fields");
        }
    }
}
