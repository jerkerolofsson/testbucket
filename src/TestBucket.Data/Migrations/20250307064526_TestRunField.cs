using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestRunField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AutomationAssembly",
                table: "testcases",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "test_case_run_fields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRunCaseId = table.Column<long>(type: "bigint", nullable: false),
                    TestCaseRunId = table.Column<long>(type: "bigint", nullable: true),
                    TestRunId = table.Column<long>(type: "bigint", nullable: false),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: true),
                    LongValue = table.Column<long>(type: "bigint", nullable: true),
                    DoubleValue = table.Column<double>(type: "double precision", nullable: true),
                    StringValue = table.Column<string>(type: "text", nullable: true),
                    StringArrayValue = table.Column<string[]>(type: "text[]", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    FieldDefinitionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_case_run_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_case_run_fields_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_test_case_run_fields_field_definitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "field_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_test_case_run_fields_runs_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_test_case_run_fields_testcaseruns_TestCaseRunId",
                        column: x => x.TestCaseRunId,
                        principalTable: "testcaseruns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "test_run_fields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRunId = table.Column<long>(type: "bigint", nullable: false),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: true),
                    LongValue = table.Column<long>(type: "bigint", nullable: true),
                    DoubleValue = table.Column<double>(type: "double precision", nullable: true),
                    StringValue = table.Column<string>(type: "text", nullable: true),
                    StringArrayValue = table.Column<string[]>(type: "text[]", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    FieldDefinitionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_run_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_run_fields_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_test_run_fields_field_definitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "field_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_test_run_fields_runs_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_test_case_run_fields_FieldDefinitionId",
                table: "test_case_run_fields",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_run_fields_TenantId",
                table: "test_case_run_fields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_run_fields_TestCaseRunId",
                table: "test_case_run_fields",
                column: "TestCaseRunId");

            migrationBuilder.CreateIndex(
                name: "IX_test_case_run_fields_TestRunId",
                table: "test_case_run_fields",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_test_run_fields_FieldDefinitionId",
                table: "test_run_fields",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_test_run_fields_TenantId",
                table: "test_run_fields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_test_run_fields_TestRunId",
                table: "test_run_fields",
                column: "TestRunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "test_case_run_fields");

            migrationBuilder.DropTable(
                name: "test_run_fields");

            migrationBuilder.DropColumn(
                name: "AutomationAssembly",
                table: "testcases");
        }
    }
}
