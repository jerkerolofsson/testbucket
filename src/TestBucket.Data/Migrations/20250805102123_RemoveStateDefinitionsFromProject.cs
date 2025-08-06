using Microsoft.EntityFrameworkCore.Migrations;
using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Testing.States;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStateDefinitionsFromProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueStates",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "RequirementStates",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "TestStates",
                table: "projects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<IssueState[]>(
                name: "IssueStates",
                table: "projects",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<RequirementState[]>(
                name: "RequirementStates",
                table: "projects",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<TestState[]>(
                name: "TestStates",
                table: "projects",
                type: "jsonb",
                nullable: true);
        }
    }
}
