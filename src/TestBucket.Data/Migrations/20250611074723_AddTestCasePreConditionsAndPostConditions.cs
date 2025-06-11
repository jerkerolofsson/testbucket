using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTestCasePreConditionsAndPostConditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Postconditions",
                table: "testcases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Preconditions",
                table: "testcases",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Postconditions",
                table: "testcases");

            migrationBuilder.DropColumn(
                name: "Preconditions",
                table: "testcases");
        }
    }
}
