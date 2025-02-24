using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProjectOptionalForTestSuites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_testsuite__folders_projects_TestProjectId",
                table: "testsuite__folders");

            migrationBuilder.DropForeignKey(
                name: "FK_testsuites_projects_TestProjectId",
                table: "testsuites");

            migrationBuilder.AlterColumn<long>(
                name: "TestProjectId",
                table: "testsuites",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "TestProjectId",
                table: "testsuite__folders",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_testsuite__folders_projects_TestProjectId",
                table: "testsuite__folders",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testsuites_projects_TestProjectId",
                table: "testsuites",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_testsuite__folders_projects_TestProjectId",
                table: "testsuite__folders");

            migrationBuilder.DropForeignKey(
                name: "FK_testsuites_projects_TestProjectId",
                table: "testsuites");

            migrationBuilder.AlterColumn<long>(
                name: "TestProjectId",
                table: "testsuites",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TestProjectId",
                table: "testsuite__folders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_testsuite__folders_projects_TestProjectId",
                table: "testsuite__folders",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_testsuites_projects_TestProjectId",
                table: "testsuites",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
