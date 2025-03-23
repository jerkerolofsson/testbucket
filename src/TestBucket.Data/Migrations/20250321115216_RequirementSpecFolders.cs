using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequirementSpecFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_spec__folders_Tenants_TenantId",
                table: "spec__folders");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "spec__folders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "spec__folders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_spec__folders_TeamId",
                table: "spec__folders",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_spec__folders_Tenants_TenantId",
                table: "spec__folders",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_spec__folders_teams_TeamId",
                table: "spec__folders",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_spec__folders_Tenants_TenantId",
                table: "spec__folders");

            migrationBuilder.DropForeignKey(
                name: "FK_spec__folders_teams_TeamId",
                table: "spec__folders");

            migrationBuilder.DropIndex(
                name: "IX_spec__folders_TeamId",
                table: "spec__folders");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "spec__folders");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "spec__folders",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_spec__folders_Tenants_TenantId",
                table: "spec__folders",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
