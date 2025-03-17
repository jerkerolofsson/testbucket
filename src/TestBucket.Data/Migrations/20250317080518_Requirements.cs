using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class Requirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RequirementId",
                table: "test_case_fields",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "spec",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileResourceId = table.Column<long>(type: "bigint", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spec", x => x.Id);
                    table.ForeignKey(
                        name: "FK_spec_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_spec_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_spec_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "spec__folders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: true),
                    PathIds = table.Column<long[]>(type: "bigint[]", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true),
                    RequirementSpecificationId = table.Column<long>(type: "bigint", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spec__folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_spec__folders_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_spec__folders_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_spec__folders_spec_RequirementSpecificationId",
                        column: x => x.RequirementSpecificationId,
                        principalTable: "spec",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_spec__folders_spec__folders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "spec__folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "requirements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: false),
                    PathIds = table.Column<long[]>(type: "bigint[]", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true),
                    RequirementSpecificationId = table.Column<long>(type: "bigint", nullable: false),
                    RequirementSpecificationFolderId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requirements_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_requirements_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_requirements_spec_RequirementSpecificationId",
                        column: x => x.RequirementSpecificationId,
                        principalTable: "spec",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_requirements_spec__folders_RequirementSpecificationFolderId",
                        column: x => x.RequirementSpecificationFolderId,
                        principalTable: "spec__folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_test_case_fields_RequirementId",
                table: "test_case_fields",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_Created",
                table: "requirements",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_Name",
                table: "requirements",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_RequirementSpecificationFolderId",
                table: "requirements",
                column: "RequirementSpecificationFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_RequirementSpecificationId",
                table: "requirements",
                column: "RequirementSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_TenantId",
                table: "requirements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_requirements_TestProjectId",
                table: "requirements",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_spec_TeamId",
                table: "spec",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_spec_TenantId_Created",
                table: "spec",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_spec_TestProjectId",
                table: "spec",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_spec__folders_ParentId",
                table: "spec__folders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_spec__folders_RequirementSpecificationId",
                table: "spec__folders",
                column: "RequirementSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_spec__folders_TenantId_Created",
                table: "spec__folders",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_spec__folders_TestProjectId",
                table: "spec__folders",
                column: "TestProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_case_fields_requirements_RequirementId",
                table: "test_case_fields",
                column: "RequirementId",
                principalTable: "requirements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_case_fields_requirements_RequirementId",
                table: "test_case_fields");

            migrationBuilder.DropTable(
                name: "requirements");

            migrationBuilder.DropTable(
                name: "spec__folders");

            migrationBuilder.DropTable(
                name: "spec");

            migrationBuilder.DropIndex(
                name: "IX_test_case_fields_RequirementId",
                table: "test_case_fields");

            migrationBuilder.DropColumn(
                name: "RequirementId",
                table: "test_case_fields");
        }
    }
}
