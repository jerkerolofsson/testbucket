using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApiKeysUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "ApiKeys",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ApiKeys",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Modified",
                table: "ApiKeys",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "ApiKeys",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ApiKeys",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_TenantId",
                table: "ApiKeys",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_Tenants_TenantId",
                table: "ApiKeys",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_Tenants_TenantId",
                table: "ApiKeys");

            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_TenantId",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ApiKeys");
        }
    }
}
