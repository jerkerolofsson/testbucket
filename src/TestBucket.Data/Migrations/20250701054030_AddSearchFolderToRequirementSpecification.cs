using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using TestBucket.Contracts;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchFolderToRequirementSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<SearchFolder>>(
                name: "SearchFolders",
                table: "spec",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchFolders",
                table: "spec");
        }
    }
}
