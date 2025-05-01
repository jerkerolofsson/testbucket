using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class FieldDateTimeValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateTimeOffsetValue",
                table: "test_run_fields",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateValue",
                table: "test_run_fields",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeSpanValue",
                table: "test_run_fields",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateTimeOffsetValue",
                table: "test_case_run_fields",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateValue",
                table: "test_case_run_fields",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeSpanValue",
                table: "test_case_run_fields",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateTimeOffsetValue",
                table: "test_case_fields",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateValue",
                table: "test_case_fields",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeSpanValue",
                table: "test_case_fields",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateTimeOffsetValue",
                table: "requirement_fields",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateValue",
                table: "requirement_fields",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeSpanValue",
                table: "requirement_fields",
                type: "interval",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTimeOffsetValue",
                table: "test_run_fields");

            migrationBuilder.DropColumn(
                name: "DateValue",
                table: "test_run_fields");

            migrationBuilder.DropColumn(
                name: "TimeSpanValue",
                table: "test_run_fields");

            migrationBuilder.DropColumn(
                name: "DateTimeOffsetValue",
                table: "test_case_run_fields");

            migrationBuilder.DropColumn(
                name: "DateValue",
                table: "test_case_run_fields");

            migrationBuilder.DropColumn(
                name: "TimeSpanValue",
                table: "test_case_run_fields");

            migrationBuilder.DropColumn(
                name: "DateTimeOffsetValue",
                table: "test_case_fields");

            migrationBuilder.DropColumn(
                name: "DateValue",
                table: "test_case_fields");

            migrationBuilder.DropColumn(
                name: "TimeSpanValue",
                table: "test_case_fields");

            migrationBuilder.DropColumn(
                name: "DateTimeOffsetValue",
                table: "requirement_fields");

            migrationBuilder.DropColumn(
                name: "DateValue",
                table: "requirement_fields");

            migrationBuilder.DropColumn(
                name: "TimeSpanValue",
                table: "requirement_fields");
        }
    }
}
