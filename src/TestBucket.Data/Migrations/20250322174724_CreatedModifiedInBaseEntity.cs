using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatedModifiedInBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "testsuite__folders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "testsuite__folders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "testsuite__folders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "test_run_fields",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "test_run_fields",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "test_run_fields",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "test_run_fields",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "test_case_run_fields",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "test_case_run_fields",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "test_case_run_fields",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "test_case_run_fields",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "test_case_fields",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "test_case_fields",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "test_case_fields",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "test_case_fields",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "steps",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "steps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "steps",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "steps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "spec__folders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "spec__folders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "spec__folders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "requirement_test_links",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "requirement_test_links",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "requirement_test_links",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "requirement_test_links",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "field_definitions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "field_definitions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "field_definitions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "field_definitions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "testsuite__folders");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "testsuite__folders");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "testsuite__folders");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "test_run_fields");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "test_run_fields");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "test_run_fields");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "test_run_fields");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "test_case_run_fields");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "test_case_run_fields");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "test_case_run_fields");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "test_case_run_fields");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "test_case_fields");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "test_case_fields");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "test_case_fields");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "test_case_fields");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "steps");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "steps");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "steps");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "steps");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "spec__folders");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "spec__folders");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "spec__folders");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "requirement_test_links");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "requirement_test_links");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "requirement_test_links");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "requirement_test_links");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "field_definitions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "field_definitions");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "field_definitions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "field_definitions");
        }
    }
}
