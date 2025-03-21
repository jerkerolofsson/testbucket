using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequirementTestLinkEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "requirement_test_links",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequirementId = table.Column<long>(type: "bigint", nullable: false),
                    TestCaseId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requirement_test_links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requirement_test_links_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_requirement_test_links_requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_requirement_test_links_testcases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "testcases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_requirement_test_links_RequirementId",
                table: "requirement_test_links",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_test_links_TenantId",
                table: "requirement_test_links",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_test_links_TestCaseId",
                table: "requirement_test_links",
                column: "TestCaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "requirement_test_links");
        }
    }
}
