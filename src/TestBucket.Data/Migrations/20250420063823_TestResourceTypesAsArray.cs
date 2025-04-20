using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestResourceTypesAsArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceTypes",
                table: "TestResources");

            migrationBuilder.AddColumn<string[]>(
                name: "Types",
                table: "TestResources",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Types",
                table: "TestResources");

            migrationBuilder.AddColumn<List<string>>(
                name: "ResourceTypes",
                table: "TestResources",
                type: "jsonb",
                nullable: true);
        }
    }
}
