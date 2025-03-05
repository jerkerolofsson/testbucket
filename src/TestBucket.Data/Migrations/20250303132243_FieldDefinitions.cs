using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class FieldDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "field_definitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Trait = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Options = table.Column<List<string>>(type: "jsonb", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_field_definitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_field_definitions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_field_definitions_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "test_case_fields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: true),
                    LongValue = table.Column<long>(type: "bigint", nullable: true),
                    DoubleValue = table.Column<double>(type: "double precision", nullable: true),
                    StringValue = table.Column<string>(type: "text", nullable: true),
                    FieldDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    TestCaseId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_case_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_case_fields_field_definitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "field_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_test_case_fields_testcases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "testcases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_field_definitions_TenantId_IsDeleted",
                table: "field_definitions",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_field_definitions_TestProjectId",
                table: "field_definitions",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_fields_FieldDefinitionId",
                table: "test_case_fields",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_fields_TestCaseId",
                table: "test_case_fields",
                column: "TestCaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "test_case_fields");

            migrationBuilder.DropTable(
                name: "field_definitions");
        }
    }
}
