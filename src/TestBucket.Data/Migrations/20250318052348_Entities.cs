using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_runs_Tenants_TenantId",
                table: "runs");

            migrationBuilder.DropForeignKey(
                name: "FK_testcaseruns_Tenants_TenantId",
                table: "testcaseruns");

            migrationBuilder.DropForeignKey(
                name: "FK_testcaseruns_projects_TestProjectId",
                table: "testcaseruns");

            migrationBuilder.DropForeignKey(
                name: "FK_testcases_Tenants_TenantId",
                table: "testcases");

            migrationBuilder.DropForeignKey(
                name: "FK_testcases_testsuites_TestSuiteId",
                table: "testcases");

            migrationBuilder.DropForeignKey(
                name: "FK_testsuites_Tenants_TenantId",
                table: "testsuites");

            migrationBuilder.DropIndex(
                name: "IX_testcases_TestSuiteId",
                table: "testcases");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "testsuites",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "testsuite__folders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "testcases",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "testcases",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TestProjectId",
                table: "testcaseruns",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "testcaseruns",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "testcaseruns",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "steps",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "steps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestProjectId",
                table: "steps",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "runs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "field_definitions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_testsuite__folders_TeamId",
                table: "testsuite__folders",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TeamId",
                table: "testcases",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_testcaseruns_TeamId",
                table: "testcaseruns",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_steps_TeamId",
                table: "steps",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_steps_TenantId",
                table: "steps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_steps_TestProjectId",
                table: "steps",
                column: "TestProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_field_definitions_TeamId",
                table: "field_definitions",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_field_definitions_teams_TeamId",
                table: "field_definitions",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_runs_Tenants_TenantId",
                table: "runs",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_steps_Tenants_TenantId",
                table: "steps",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_steps_projects_TestProjectId",
                table: "steps",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_steps_teams_TeamId",
                table: "steps",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testcaseruns_Tenants_TenantId",
                table: "testcaseruns",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testcaseruns_projects_TestProjectId",
                table: "testcaseruns",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testcaseruns_teams_TeamId",
                table: "testcaseruns",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testcases_Tenants_TenantId",
                table: "testcases",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testcases_teams_TeamId",
                table: "testcases",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testsuite__folders_teams_TeamId",
                table: "testsuite__folders",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testsuites_Tenants_TenantId",
                table: "testsuites",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_field_definitions_teams_TeamId",
                table: "field_definitions");

            migrationBuilder.DropForeignKey(
                name: "FK_runs_Tenants_TenantId",
                table: "runs");

            migrationBuilder.DropForeignKey(
                name: "FK_steps_Tenants_TenantId",
                table: "steps");

            migrationBuilder.DropForeignKey(
                name: "FK_steps_projects_TestProjectId",
                table: "steps");

            migrationBuilder.DropForeignKey(
                name: "FK_steps_teams_TeamId",
                table: "steps");

            migrationBuilder.DropForeignKey(
                name: "FK_testcaseruns_Tenants_TenantId",
                table: "testcaseruns");

            migrationBuilder.DropForeignKey(
                name: "FK_testcaseruns_projects_TestProjectId",
                table: "testcaseruns");

            migrationBuilder.DropForeignKey(
                name: "FK_testcaseruns_teams_TeamId",
                table: "testcaseruns");

            migrationBuilder.DropForeignKey(
                name: "FK_testcases_Tenants_TenantId",
                table: "testcases");

            migrationBuilder.DropForeignKey(
                name: "FK_testcases_teams_TeamId",
                table: "testcases");

            migrationBuilder.DropForeignKey(
                name: "FK_testsuite__folders_teams_TeamId",
                table: "testsuite__folders");

            migrationBuilder.DropForeignKey(
                name: "FK_testsuites_Tenants_TenantId",
                table: "testsuites");

            migrationBuilder.DropIndex(
                name: "IX_testsuite__folders_TeamId",
                table: "testsuite__folders");

            migrationBuilder.DropIndex(
                name: "IX_testcases_TeamId",
                table: "testcases");

            migrationBuilder.DropIndex(
                name: "IX_testcaseruns_TeamId",
                table: "testcaseruns");

            migrationBuilder.DropIndex(
                name: "IX_steps_TeamId",
                table: "steps");

            migrationBuilder.DropIndex(
                name: "IX_steps_TenantId",
                table: "steps");

            migrationBuilder.DropIndex(
                name: "IX_steps_TestProjectId",
                table: "steps");

            migrationBuilder.DropIndex(
                name: "IX_field_definitions_TeamId",
                table: "field_definitions");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "testsuite__folders");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "testcases");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "testcaseruns");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "steps");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "steps");

            migrationBuilder.DropColumn(
                name: "TestProjectId",
                table: "steps");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "field_definitions");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "testsuites",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "testcases",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TestProjectId",
                table: "testcaseruns",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "testcaseruns",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "runs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_testcases_TestSuiteId",
                table: "testcases",
                column: "TestSuiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_runs_Tenants_TenantId",
                table: "runs",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_testcaseruns_Tenants_TenantId",
                table: "testcaseruns",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_testcaseruns_projects_TestProjectId",
                table: "testcaseruns",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_testcases_Tenants_TenantId",
                table: "testcases",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_testcases_testsuites_TestSuiteId",
                table: "testcases",
                column: "TestSuiteId",
                principalTable: "testsuites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_testsuites_Tenants_TenantId",
                table: "testsuites",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
