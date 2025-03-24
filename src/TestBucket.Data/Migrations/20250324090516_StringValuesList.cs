using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class StringValuesList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "StringValues",
                table: "test_run_fields",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "StringValuesList",
                table: "test_run_fields",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AlterColumn<string[]>(
                name: "StringValues",
                table: "test_case_run_fields",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "StringValuesList",
                table: "test_case_run_fields",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AlterColumn<string[]>(
                name: "StringValues",
                table: "test_case_fields",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "StringValuesList",
                table: "test_case_fields",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StringValuesList",
                table: "test_run_fields");

            migrationBuilder.DropColumn(
                name: "StringValuesList",
                table: "test_case_run_fields");

            migrationBuilder.DropColumn(
                name: "StringValuesList",
                table: "test_case_fields");

            migrationBuilder.AlterColumn<List<string>>(
                name: "StringValues",
                table: "test_run_fields",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "StringValues",
                table: "test_case_run_fields",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "StringValues",
                table: "test_case_fields",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);
        }
    }
}
