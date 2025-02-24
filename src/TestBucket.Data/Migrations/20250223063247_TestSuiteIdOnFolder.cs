using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestSuiteIdOnFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TestSuiteId",
                table: "testsuite__folders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_testsuite__folders_TestSuiteId",
                table: "testsuite__folders",
                column: "TestSuiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_testsuite__folders_testsuites_TestSuiteId",
                table: "testsuite__folders",
                column: "TestSuiteId",
                principalTable: "testsuites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_testsuite__folders_testsuites_TestSuiteId",
                table: "testsuite__folders");

            migrationBuilder.DropIndex(
                name: "IX_testsuite__folders_TestSuiteId",
                table: "testsuite__folders");

            migrationBuilder.DropColumn(
                name: "TestSuiteId",
                table: "testsuite__folders");
        }
    }
}
