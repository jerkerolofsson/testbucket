using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using TestBucket.Contracts.Testing.Models;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class DependenciesOnSuite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<TestCaseDependency>>(
                name: "Dependencies",
                table: "testsuites",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dependencies",
                table: "testsuites");
        }
    }
}
