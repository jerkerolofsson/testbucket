using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class CommitProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Commited",
                table: "Commits",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommitedBy",
                table: "Commits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Commits",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Commited",
                table: "Commits");

            migrationBuilder.DropColumn(
                name: "CommitedBy",
                table: "Commits");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Commits");
        }
    }
}
