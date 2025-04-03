using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class CiCd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultCiCdRef",
                table: "testsuites",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Dictionary<string, string>>(
                name: "Variables",
                table: "testsuites",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCiCdRef",
                table: "testsuites");

            migrationBuilder.DropColumn(
                name: "Variables",
                table: "testsuites");
        }
    }
}
