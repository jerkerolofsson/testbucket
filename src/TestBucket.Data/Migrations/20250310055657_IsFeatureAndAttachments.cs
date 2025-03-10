using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsFeatureAndAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeatureDescription",
                table: "testsuite__folders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeature",
                table: "testsuite__folders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "TestCaseRunId",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestProjectId",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestSuiteFolderId",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestSuiteId",
                table: "Files",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeatureDescription",
                table: "testsuite__folders");

            migrationBuilder.DropColumn(
                name: "IsFeature",
                table: "testsuite__folders");

            migrationBuilder.DropColumn(
                name: "TestCaseRunId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "TestProjectId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "TestSuiteFolderId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "TestSuiteId",
                table: "Files");
        }
    }
}
