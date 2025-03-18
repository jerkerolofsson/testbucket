using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatedByModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_spec_Tenants_TenantId",
                table: "spec");

            migrationBuilder.RenameColumn(
                name: "Creator",
                table: "runs",
                newName: "ModifiedBy");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "testsuites",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "testsuites",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "testsuites",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "testcases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "testcases",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "testcases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "testcaseruns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "testcaseruns",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "testcaseruns",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "spec",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "spec",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "spec",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "spec",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "runs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "runs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "requirements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "requirements",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "requirements",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_spec_Tenants_TenantId",
                table: "spec",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_spec_Tenants_TenantId",
                table: "spec");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "testsuites");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "testsuites");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "testsuites");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "testcases");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "testcases");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "testcases");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "testcaseruns");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "testcaseruns");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "testcaseruns");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "spec");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "spec");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "spec");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "runs");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "runs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "requirements");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "requirements");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "requirements");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "runs",
                newName: "Creator");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "spec",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_spec_Tenants_TenantId",
                table: "spec",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
