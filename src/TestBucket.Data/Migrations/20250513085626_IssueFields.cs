using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class IssueFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MilestoneName",
                table: "LocalIssues");

            migrationBuilder.CreateTable(
                name: "issue_fields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocalIssueId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Inherited = table.Column<bool>(type: "boolean", nullable: true),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: true),
                    LongValue = table.Column<long>(type: "bigint", nullable: true),
                    DoubleValue = table.Column<double>(type: "double precision", nullable: true),
                    StringValue = table.Column<string>(type: "text", nullable: true),
                    DateValue = table.Column<DateOnly>(type: "date", nullable: true),
                    TimeSpanValue = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DateTimeOffsetValue = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StringValuesList = table.Column<List<string>>(type: "jsonb", nullable: true),
                    FieldDefinitionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_issue_fields_LocalIssues_LocalIssueId",
                        column: x => x.LocalIssueId,
                        principalTable: "LocalIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_issue_fields_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_issue_fields_field_definitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "field_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalIssues_ExternalSystemId_Created",
                table: "LocalIssues",
                columns: new[] { "ExternalSystemId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalIssues_State_Created",
                table: "LocalIssues",
                columns: new[] { "State", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_issue_fields_FieldDefinitionId",
                table: "issue_fields",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_issue_fields_LocalIssueId",
                table: "issue_fields",
                column: "LocalIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_issue_fields_TenantId",
                table: "issue_fields",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "issue_fields");

            migrationBuilder.DropIndex(
                name: "IX_LocalIssues_ExternalSystemId_Created",
                table: "LocalIssues");

            migrationBuilder.DropIndex(
                name: "IX_LocalIssues_State_Created",
                table: "LocalIssues");

            migrationBuilder.AddColumn<string>(
                name: "MilestoneName",
                table: "LocalIssues",
                type: "text",
                nullable: true);
        }
    }
}
