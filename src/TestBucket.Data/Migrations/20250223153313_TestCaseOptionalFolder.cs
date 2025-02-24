using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestCaseOptionalFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_testcases_projects_TestProjectId",
                table: "testcases");

            migrationBuilder.DropForeignKey(
                name: "FK_testcases_testsuite__folders_TestSuiteFolderId",
                table: "testcases");

            migrationBuilder.AlterColumn<long>(
                name: "TestSuiteFolderId",
                table: "testcases",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "TestProjectId",
                table: "testcases",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_testcases_projects_TestProjectId",
                table: "testcases",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testcases_testsuite__folders_TestSuiteFolderId",
                table: "testcases",
                column: "TestSuiteFolderId",
                principalTable: "testsuite__folders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_testcases_projects_TestProjectId",
                table: "testcases");

            migrationBuilder.DropForeignKey(
                name: "FK_testcases_testsuite__folders_TestSuiteFolderId",
                table: "testcases");

            migrationBuilder.AlterColumn<long>(
                name: "TestSuiteFolderId",
                table: "testcases",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TestProjectId",
                table: "testcases",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_testcases_projects_TestProjectId",
                table: "testcases",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_testcases_testsuite__folders_TestSuiteFolderId",
                table: "testcases",
                column: "TestSuiteFolderId",
                principalTable: "testsuite__folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
