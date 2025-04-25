using Microsoft.EntityFrameworkCore.Migrations;
using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class EntityStatesForRequirementsTestsIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MappedState",
                table: "testcaseruns",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MappedState",
                table: "requirements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MappedType",
                table: "requirements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<IssueStates[]>(
                name: "IssueStates",
                table: "projects",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<RequirementState[]>(
                name: "RequirementStates",
                table: "projects",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MappedState",
                table: "testcaseruns");

            migrationBuilder.DropColumn(
                name: "MappedState",
                table: "requirements");

            migrationBuilder.DropColumn(
                name: "MappedType",
                table: "requirements");

            migrationBuilder.DropColumn(
                name: "IssueStates",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "RequirementStates",
                table: "projects");
        }
    }
}
