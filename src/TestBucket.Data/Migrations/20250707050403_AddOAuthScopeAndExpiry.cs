using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOAuthScopeAndExpiry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "external_systems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "external_systems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TokenExpiry",
                table: "external_systems",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "external_systems");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "external_systems");

            migrationBuilder.DropColumn(
                name: "TokenExpiry",
                table: "external_systems");
        }
    }
}
