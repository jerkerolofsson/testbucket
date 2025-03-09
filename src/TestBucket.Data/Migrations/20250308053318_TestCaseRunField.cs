using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestCaseRunField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_case_run_fields_testcaseruns_TestCaseRunId",
                table: "test_case_run_fields");

            migrationBuilder.DropColumn(
                name: "TestRunCaseId",
                table: "test_case_run_fields");

            migrationBuilder.AlterColumn<long>(
                name: "TestCaseRunId",
                table: "test_case_run_fields",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_test_case_run_fields_testcaseruns_TestCaseRunId",
                table: "test_case_run_fields",
                column: "TestCaseRunId",
                principalTable: "testcaseruns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_case_run_fields_testcaseruns_TestCaseRunId",
                table: "test_case_run_fields");

            migrationBuilder.AlterColumn<long>(
                name: "TestCaseRunId",
                table: "test_case_run_fields",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "TestRunCaseId",
                table: "test_case_run_fields",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_test_case_run_fields_testcaseruns_TestCaseRunId",
                table: "test_case_run_fields",
                column: "TestCaseRunId",
                principalTable: "testcaseruns",
                principalColumn: "Id");
        }
    }
}
