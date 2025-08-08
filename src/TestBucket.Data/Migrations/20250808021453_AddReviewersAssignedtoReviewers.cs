using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using TestBucket.Domain.Shared;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewersAssignedtoReviewers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Reviewers",
                table: "testsuites",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "testcases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<AssignedReviewer>>(
                name: "ReviewAssignedTo",
                table: "testcases",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<List<AssignedReviewer>>(
                name: "ReviewAssignedTo",
                table: "requirements",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reviewers",
                table: "testsuites");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "testcases");

            migrationBuilder.DropColumn(
                name: "ReviewAssignedTo",
                table: "testcases");

            migrationBuilder.DropColumn(
                name: "ReviewAssignedTo",
                table: "requirements");
        }
    }
}
