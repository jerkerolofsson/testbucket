using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequirementAsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_requirements_Tenants_TenantId",
                table: "requirements");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "requirements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "requirements",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_requirements_TeamId",
                table: "requirements",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_requirements_Tenants_TenantId",
                table: "requirements",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_requirements_teams_TeamId",
                table: "requirements",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_requirements_Tenants_TenantId",
                table: "requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_requirements_teams_TeamId",
                table: "requirements");

            migrationBuilder.DropIndex(
                name: "IX_requirements_TeamId",
                table: "requirements");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "requirements");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "requirements",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_requirements_Tenants_TenantId",
                table: "requirements",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
